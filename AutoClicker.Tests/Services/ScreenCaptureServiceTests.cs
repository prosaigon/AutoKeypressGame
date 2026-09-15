using System.Drawing;
using AutoClicker.Services;
using Xunit;

namespace AutoClicker.Tests.Services
{
    public class ScreenCaptureServiceTests
    {
        [Fact]
        public void ColorsMatch_ExactMatch_ReturnsTrue()
        {
            var c1 = Color.FromArgb(255, 100, 50);
            var c2 = Color.FromArgb(255, 100, 50);

            Assert.True(ScreenCaptureService.ColorsMatch(c1, c2, 0));
        }

        [Fact]
        public void ColorsMatch_WithinTolerance_ReturnsTrue()
        {
            var c1 = Color.FromArgb(250, 105, 52);
            var c2 = Color.FromArgb(255, 100, 50);

            Assert.True(ScreenCaptureService.ColorsMatch(c1, c2, 10));
        }

        [Fact]
        public void ColorsMatch_ExceedsTolerance_ReturnsFalse()
        {
            var c1 = Color.FromArgb(200, 100, 50);
            var c2 = Color.FromArgb(255, 100, 50);

            Assert.False(ScreenCaptureService.ColorsMatch(c1, c2, 10));
        }

        [Fact]
        public void ColorToHex_And_HexToColor_RoundTrip()
        {
            var original = Color.FromArgb(18, 52, 86);
            string hex = ScreenCaptureService.ColorToHex(original);
            Assert.Equal("#123456", hex);

            var parsed = ScreenCaptureService.HexToColor(hex);
            Assert.Equal(original.R, parsed.R);
            Assert.Equal(original.G, parsed.G);
            Assert.Equal(original.B, parsed.B);
        }

        [Fact]
        public void HexToColor_InvalidInput_ReturnsWhite()
        {
            var result = ScreenCaptureService.HexToColor("INVALID");
            Assert.Equal(Color.White.R, result.R);
            Assert.Equal(Color.White.G, result.G);
            Assert.Equal(Color.White.B, result.B);
        }

        [Fact]
        public void CompareImages_IdenticalBitmaps_ReturnsOne()
        {
            using (var bmp1 = new Bitmap(10, 10))
            using (var bmp2 = new Bitmap(10, 10))
            {
                for (int x = 0; x < 10; x++)
                    for (int y = 0; y < 10; y++)
                    {
                        bmp1.SetPixel(x, y, Color.Red);
                        bmp2.SetPixel(x, y, Color.Red);
                    }

                double similarity = ScreenCaptureService.CompareImages(bmp1, bmp2);
                Assert.Equal(1.0, similarity);
            }
        }

        [Fact]
        public void CompareImages_CompletelyDifferent_ReturnsZero()
        {
            using (var bmp1 = new Bitmap(10, 10))
            using (var bmp2 = new Bitmap(10, 10))
            {
                for (int x = 0; x < 10; x++)
                    for (int y = 0; y < 10; y++)
                    {
                        bmp1.SetPixel(x, y, Color.Red);
                        bmp2.SetPixel(x, y, Color.Blue);
                    }

                double similarity = ScreenCaptureService.CompareImages(bmp1, bmp2);
                Assert.Equal(0.0, similarity);
            }
        }

        [Fact]
        public void FindPixelByColor_PixelExists_ReturnsCorrectCoordinate()
        {
            using (var bmp = new Bitmap(20, 20))
            {
                for (int x = 0; x < 20; x++)
                    for (int y = 0; y < 20; y++)
                        bmp.SetPixel(x, y, Color.Black);

                bmp.SetPixel(7, 13, Color.Yellow);

                var point = ScreenCaptureService.FindPixelByColor(bmp, Color.Yellow, 0);
                Assert.NotNull(point);
                Assert.Equal(7, point.Value.X);
                Assert.Equal(13, point.Value.Y);
            }
        }

        [Fact]
        public void FindPixelByColor_PixelDoesNotExist_ReturnsNull()
        {
            using (var bmp = new Bitmap(10, 10))
            {
                for (int x = 0; x < 10; x++)
                    for (int y = 0; y < 10; y++)
                        bmp.SetPixel(x, y, Color.Black);

                var point = ScreenCaptureService.FindPixelByColor(bmp, Color.Magenta, 0);
                Assert.Null(point);
            }
        }
    }
}
