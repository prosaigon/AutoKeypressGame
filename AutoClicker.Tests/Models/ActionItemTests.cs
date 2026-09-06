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
        public void PropertyChanged_FiresOnKeyOrButtonChange()
        {
            var item = new ActionItem();
            bool eventFired = false;
            string changedPropName = null;

            item.PropertyChanged += (s, e) =>
            {
                eventFired = true;
                changedPropName = e.PropertyName;
            };

            item.KeyOrButton = "B";

            Assert.True(eventFired);
        }
    }
}
