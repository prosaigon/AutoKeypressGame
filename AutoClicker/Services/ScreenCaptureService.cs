using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;

namespace AutoClicker.Services
{
    public static class ScreenCaptureService
    {
        #region Win32 P/Invoke

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        #endregion

        public static Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            try
            {
                uint pixel = GetPixel(hdc, x, y);
                // COLORREF format: 0x00bbggrr
                int r = (int)(pixel & 0x000000FF);
                int g = (int)((pixel & 0x0000FF00) >> 8);
                int b = (int)((pixel & 0x00FF0000) >> 16);
                return Color.FromArgb(r, g, b);
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        public static bool ColorsMatch(Color a, Color b, int tolerance = 0)
        {
            if (tolerance <= 0)
            {
                return a.R == b.R && a.G == b.G && a.B == b.B;
            }

            return Math.Abs(a.R - b.R) <= tolerance &&
                   Math.Abs(a.G - b.G) <= tolerance &&
                   Math.Abs(a.B - b.B) <= tolerance;
        }

        public static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Color.White;

            string clean = hex.Trim().TrimStart('#');
            if (clean.Length == 6 &&
                int.TryParse(clean.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int r) &&
                int.TryParse(clean.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int g) &&
                int.TryParse(clean.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int b))
            {
                return Color.FromArgb(r, g, b);
            }

            return Color.White;
        }

        public static Bitmap CaptureRegion(int x, int y, int width, int height)
        {
            if (width <= 0) width = 1;
            if (height <= 0) height = 1;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }

        public static double CompareImages(Bitmap a, Bitmap b)
        {
            if (a == null || b == null) return 0.0;
            if (a.Width != b.Width || a.Height != b.Height) return 0.0;

            int totalPixels = a.Width * a.Height;
            if (totalPixels == 0) return 1.0;

            int matchingPixels = 0;

            for (int y = 0; y < a.Height; y++)
            {
                for (int x = 0; x < a.Width; x++)
                {
                    Color colorA = a.GetPixel(x, y);
                    Color colorB = b.GetPixel(x, y);

                    if (ColorsMatch(colorA, colorB, 5))
                    {
                        matchingPixels++;
                    }
                }
            }

            return (double)matchingPixels / totalPixels;
        }

        public static Point? FindPixelByColor(Bitmap bmp, Color target, int tolerance = 0)
        {
            if (bmp == null) return null;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (ColorsMatch(bmp.GetPixel(x, y), target, tolerance))
                    {
                        return new Point(x, y);
                    }
                }
            }

            return null;
        }
    }
}
