using AutoClicker.Models;
using Xunit;

namespace AutoClicker.Tests.Models
{
    public class ActionItemTests
    {
        [Fact]
        public void DisplayValue_KeyboardAction_ReturnsFormattedKeyString()
        {
            var item = new ActionItem
            {
                ActionType = ActionType.Keyboard,
                KeyOrButton = "F6",
                Delay = 200
            };

            Assert.Equal("Key: F6", item.DisplayValue);
            Assert.Equal("Delay: 200ms", item.DisplayDelay);
        }

        [Fact]
        public void DisplayValue_MouseClickAction_ReturnsFormattedClickString()
        {
            var item = new ActionItem
            {
                ActionType = ActionType.MouseClick,
                MouseButton = MouseButtonType.Left,
                X = 100,
                Y = 200,
                Delay = 150
            };

            Assert.Equal("Click Left at (100, 200)", item.DisplayValue);
            Assert.Equal("Delay: 150ms", item.DisplayDelay);
        }

        [Fact]
        public void DisplayValue_WaitForPixelColor_ReturnsFormattedString()
        {
            var item = new ActionItem
            {
                ActionType = ActionType.WaitForPixelColor,
                X = 300,
                Y = 400,
                TargetColor = "#FF0000",
                ColorTolerance = 15
            };

            Assert.Equal("Wait Pixel (300, 400) == #FF0000 (Tol: 15)", item.DisplayValue);
        }

        [Fact]
        public void DisplayValue_WaitForPixelChange_ReturnsFormattedString()
        {
            var item = new ActionItem
            {
                ActionType = ActionType.WaitForPixelChange,
                X = 500,
                Y = 600,
                TargetColor = "#00FF00"
            };

            Assert.Equal("Wait Pixel (500, 600) changes from #00FF00", item.DisplayValue);
        }

        [Fact]
        public void DisplayValue_ConditionalPixelColor_ReturnsFormattedString()
        {
            var item = new ActionItem
            {
                ActionType = ActionType.ConditionalPixelColor,
                X = 150,
                Y = 250,
                TargetColor = "#0000FF",
                KeyOrButton = "SPACE"
            };

            Assert.Equal("If Pixel (150, 250) == #0000FF -> Press SPACE", item.DisplayValue);
        }

        [Fact]
        public void PropertyChanged_FiresOnKeyOrButtonChange()
        {
            var item = new ActionItem();
            var changedProps = new System.Collections.Generic.List<string>();

            item.PropertyChanged += (s, e) =>
            {
                changedProps.Add(e.PropertyName);
            };

            item.KeyOrButton = "B";

            Assert.Contains(nameof(ActionItem.KeyOrButton), changedProps);
            Assert.Contains(nameof(ActionItem.DisplayValue), changedProps);
        }
    }
}
