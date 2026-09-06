using AutoClicker.Services;
using Xunit;

namespace AutoClicker.Tests.Services
{
    public class InputRecorderServiceTests
    {
        [Theory]
        [InlineData(0x41, "A")]
        [InlineData(0x5A, "Z")]
        [InlineData(0x30, "0")]
        [InlineData(0x39, "9")]
        [InlineData(0x70, "F1")]
        [InlineData(0x76, "F7")]
        [InlineData(0x7B, "F12")]
        [InlineData(0x0D, "ENTER")]
        [InlineData(0x20, "SPACE")]
        [InlineData(0x09, "TAB")]
        [InlineData(0x1B, "ESCAPE")]
        [InlineData(0x60, "NUMPAD0")]
        [InlineData(0x61, "NUMPAD1")]
        [InlineData(0x65, "NUMPAD5")]
        [InlineData(0x69, "NUMPAD9")]
        [InlineData(0x6A, "*")]
        [InlineData(0x6B, "+")]
        [InlineData(0x6D, "-")]
        [InlineData(0x6E, ".")]
        [InlineData(0x6F, "/")]
        [InlineData(0x90, "NUMLOCK")]
        public void ConvertVkCodeToKeyName_KnownVkCodes_ReturnsExpectedString(ushort vkCode, string expectedName)
        {
            string actualName = InputRecorderService.ConvertVkCodeToKeyName(vkCode);
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void RecordingState_StartsAndStopsCorrectly()
        {
            Assert.False(InputRecorderService.IsRecording);

            InputRecorderService.StartRecording();
            Assert.True(InputRecorderService.IsRecording);

            InputRecorderService.StopRecording();
            Assert.False(InputRecorderService.IsRecording);
        }
    }
}
