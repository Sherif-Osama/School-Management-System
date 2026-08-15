using School.BLL.Common;
using Xunit;

namespace School.Tests.Common
{
    public class ValidationHelperTests
    {
        #region ValidateId
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ValidateId_Throws_WhenIdIsZeroOrNegative(int id)
        {
            Assert.Throws<ArgumentException>(() => ValidationHelper.ValidateId(id));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(66)]
        public void ValidateId_DoesNotThrow_WhenIdIsPositive(int id)
        {
            Assert.Null(Record.Exception(() => ValidationHelper.ValidateId(id)));
        }
        #endregion

        #region ValidateString
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ValidateString_Throws_WhenNullOrWhitespace(string? value)
        {
            Assert.Throws<ArgumentException>(() => ValidationHelper.ValidateString(value!, "Name"));
        }

        [Fact]
        public void ValidateString_Throws_WhenShorterThanMinLength()
        {
            Assert.Throws<ArgumentException>(() => ValidationHelper.ValidateString("ab", "Name", minLength: 3, maxLength: 10));
        }

        [Fact]
        public void ValidateString_Throws_WhenLongerThanMaxLength()
        {
            string tooLong = new string('a', 101);

            Assert.Throws<ArgumentException>(() => ValidationHelper.ValidateString(tooLong, "Name"));
        }

        [Fact]
        public void ValidateString_TrimsAndReturnsValue_WhenValid()
        {
            string result = ValidationHelper.ValidateString("  John  ", "Name");

            Assert.Equal("John", result);
        }
        #endregion
    }
}
