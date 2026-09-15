using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using AutoClicker.Services;
using Xunit;

namespace AutoClicker.Tests.Services
{
    public class PixelWatcherServiceTests : IDisposable
    {
        private readonly PixelWatcherService _service;
        private Color _mockCurrentColor = Color.Black;

        public PixelWatcherServiceTests()
        {
            _service = new PixelWatcherService((x, y) => _mockCurrentColor);
        }

        public void Dispose()
        {
            _service.Dispose();
        }

        [Fact]
        public void AddWatch_AddsConditionToList()
        {
            var cond = new PixelWatchCondition
            {
                X = 100,
                Y = 200,
                TargetColor = Color.Red,
                Mode = PixelWatchMode.WaitForColor
            };

            _service.AddWatch(cond);

            Assert.Single(_service.Conditions);
            Assert.Equal(100, _service.Conditions[0].X);
            Assert.Equal(200, _service.Conditions[0].Y);
        }

        [Fact]
        public void RemoveWatch_RemovesConditionSuccessfully()
        {
            var cond = new PixelWatchCondition
            {
                Id = "test_watch_1",
                X = 100,
                Y = 200
            };

            _service.AddWatch(cond);
            Assert.Single(_service.Conditions);

            bool removed = _service.RemoveWatch("test_watch_1");
            Assert.True(removed);
            Assert.Empty(_service.Conditions);
        }

        [Fact]
        public void ClearWatches_ClearsAllConditions()
        {
            _service.AddWatch(new PixelWatchCondition { X = 1, Y = 1 });
            _service.AddWatch(new PixelWatchCondition { X = 2, Y = 2 });

            Assert.Equal(2, _service.Conditions.Count);

            _service.ClearWatches();
            Assert.Empty(_service.Conditions);
        }

        [Fact]
        public async Task StartWatching_WaitForColor_FiresEventWhenTargetColorAppears()
        {
            _mockCurrentColor = Color.Blue;

            var cond = new PixelWatchCondition
            {
                Id = "color_match",
                X = 50,
                Y = 50,
                TargetColor = Color.Red,
                Tolerance = 5,
                Mode = PixelWatchMode.WaitForColor
            };

            _service.AddWatch(cond);
            _service.PollIntervalMs = 20;

            PixelWatchResult receivedResult = null;
            var tcs = new TaskCompletionSource<bool>();

            _service.PixelConditionMet += res =>
            {
                receivedResult = res;
                tcs.TrySetResult(true);
            };

            _service.StartWatching();
            Assert.True(_service.IsWatching);

            // Give it 50ms with non-matching color
            await Task.Delay(50);
            Assert.Null(receivedResult);

            // Change color to match target
            _mockCurrentColor = Color.Red;

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(1000));
            Assert.Equal(tcs.Task, completedTask);
            Assert.NotNull(receivedResult);
            Assert.Equal("color_match", receivedResult.WatchId);
            Assert.True(receivedResult.ConditionMet);

            _service.StopWatching();
            Assert.False(_service.IsWatching);
        }

        [Fact]
        public async Task StartWatching_WaitForChange_FiresEventWhenPixelChanges()
        {
            _mockCurrentColor = Color.White;

            var cond = new PixelWatchCondition
            {
                Id = "change_watch",
                X = 10,
                Y = 10,
                InitialColor = Color.White,
                Mode = PixelWatchMode.WaitForChange,
                Tolerance = 0
            };

            _service.AddWatch(cond);
            _service.PollIntervalMs = 20;

            PixelWatchResult receivedResult = null;
            var tcs = new TaskCompletionSource<bool>();

            _service.PixelConditionMet += res =>
            {
                receivedResult = res;
                tcs.TrySetResult(true);
            };

            _service.StartWatching();

            await Task.Delay(50);
            Assert.Null(receivedResult);

            // Change pixel
            _mockCurrentColor = Color.Green;

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(1000));
            Assert.Equal(tcs.Task, completedTask);
            Assert.NotNull(receivedResult);
            Assert.Equal("change_watch", receivedResult.WatchId);

            _service.StopWatching();
        }
    }
}
