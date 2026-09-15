using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace AutoClicker.Services
{
    public enum PixelWatchMode
    {
        WaitForColor,
        WaitForChange,
        ContinuousMonitor
    }

    public class PixelWatchCondition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public int X { get; set; }
        public int Y { get; set; }
        public Color TargetColor { get; set; } = Color.White;
        public int Tolerance { get; set; } = 10;
        public PixelWatchMode Mode { get; set; } = PixelWatchMode.WaitForColor;
        public Color InitialColor { get; set; } = Color.Empty;
    }

    public class PixelWatchResult
    {
        public string WatchId { get; set; }
        public Color DetectedColor { get; set; }
        public DateTime DetectedAt { get; set; } = DateTime.Now;
        public bool ConditionMet { get; set; }
    }

    public class PixelWatcherService : IDisposable
    {
        private readonly List<PixelWatchCondition> _conditions = new List<PixelWatchCondition>();
        private readonly object _lock = new object();
        private readonly Func<int, int, Color> _getPixelColorFunc;
        private CancellationTokenSource _cts;
        private Task _watchTask;
        private bool _disposed;

        public event Action<PixelWatchResult> PixelConditionMet;
        public event Action<string> StatusChanged;

        public int PollIntervalMs { get; set; } = 50;
        public bool IsWatching { get; private set; }

        public IReadOnlyList<PixelWatchCondition> Conditions
        {
            get
            {
                lock (_lock)
                {
                    return new List<PixelWatchCondition>(_conditions);
                }
            }
        }

        public PixelWatcherService(Func<int, int, Color> getPixelColorFunc = null)
        {
            _getPixelColorFunc = getPixelColorFunc ?? ScreenCaptureService.GetPixelColor;
        }

        public void AddWatch(PixelWatchCondition condition)
        {
            if (condition == null) return;
            lock (_lock)
            {
                if (condition.InitialColor == Color.Empty)
                {
                    condition.InitialColor = _getPixelColorFunc(condition.X, condition.Y);
                }
                _conditions.Add(condition);
            }
        }

        public bool RemoveWatch(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            lock (_lock)
            {
                return _conditions.RemoveAll(c => c.Id == id) > 0;
            }
        }

        public void ClearWatches()
        {
            lock (_lock)
            {
                _conditions.Clear();
            }
        }

        public void StartWatching(CancellationToken externalToken = default)
        {
            if (IsWatching) return;

            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var token = _cts.Token;

            IsWatching = true;
            StatusChanged?.Invoke("Vision Watcher Started");

            _watchTask = Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        List<PixelWatchCondition> currentConditions;
                        lock (_lock)
                        {
                            currentConditions = new List<PixelWatchCondition>(_conditions);
                        }

                        foreach (var cond in currentConditions)
                        {
                            if (token.IsCancellationRequested) break;

                            Color currentColor = _getPixelColorFunc(cond.X, cond.Y);
                            bool met = false;

                            switch (cond.Mode)
                            {
                                case PixelWatchMode.WaitForColor:
                                    met = ScreenCaptureService.ColorsMatch(currentColor, cond.TargetColor, cond.Tolerance);
                                    break;

                                case PixelWatchMode.WaitForChange:
                                    met = !ScreenCaptureService.ColorsMatch(currentColor, cond.InitialColor, cond.Tolerance);
                                    break;

                                case PixelWatchMode.ContinuousMonitor:
                                    met = ScreenCaptureService.ColorsMatch(currentColor, cond.TargetColor, cond.Tolerance);
                                    break;
                            }

                            if (met)
                            {
                                var result = new PixelWatchResult
                                {
                                    WatchId = cond.Id,
                                    DetectedColor = currentColor,
                                    DetectedAt = DateTime.Now,
                                    ConditionMet = true
                                };
                                PixelConditionMet?.Invoke(result);
                            }
                        }

                        int delay = Math.Max(10, PollIntervalMs);
                        await Task.Delay(delay, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Normal stop
                }
                finally
                {
                    IsWatching = false;
                    StatusChanged?.Invoke("Vision Watcher Stopped");
                }
            }, token);
        }

        public void StopWatching()
        {
            if (!IsWatching) return;

            _cts?.Cancel();
            try
            {
                _watchTask?.Wait(500);
            }
            catch { }

            IsWatching = false;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            StopWatching();
            _cts?.Dispose();
        }
    }
}
