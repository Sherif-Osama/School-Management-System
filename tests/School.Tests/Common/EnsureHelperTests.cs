using School.BLL.Common;
using Xunit;

namespace School.Tests.Common
{
    public class EnsureHelperTests
    {
        #region EnsureExistsAsync
        [Fact]
        public async Task EnsureExistsAsync_Throws_WhenEntityDoesNotExist()
        {
            Func<int, Task<bool>> existsFunc = _ => Task.FromResult(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => EnsureHelper.EnsureExistsAsync(existsFunc, 1, "Student"));
        }

        [Fact]
        public async Task EnsureExistsAsync_DoesNotThrow_WhenEntityExists()
        {
            Func<int, Task<bool>> existsFunc = _ => Task.FromResult(true);

            Assert.Null(await Record.ExceptionAsync(() => EnsureHelper.EnsureExistsAsync(existsFunc, 1, "Student")));
        }
        #endregion

        #region EnsureUniqueAsync
        [Fact]
        public async Task EnsureUniqueAsync_DoesNotThrow_WhenNoExistingRecordFound()
        {
            Func<string, Task<string?>> getExisting = _ => Task.FromResult<string?>(null);

            Assert.Null(await Record.ExceptionAsync(() => EnsureHelper.EnsureUniqueAsync(getExisting, "unique-key")));
        }

        [Fact]
        public async Task EnsureUniqueAsync_Throws_WhenExistingRecordBelongsToAnotherEntity()
        {
            Func<string, Task<string?>> getExisting = _ => Task.FromResult<string?>("existing");

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => EnsureHelper.EnsureUniqueAsync(getExisting, "unique-key", s => 5, currentId: 1));
        }

        [Fact]
        public async Task EnsureUniqueAsync_DoesNotThrow_WhenExistingRecordIsTheCurrentEntity()
        {
            Func<string, Task<string?>> getExisting = _ => Task.FromResult<string?>("existing");

            Assert.Null(await Record.ExceptionAsync(
                () => EnsureHelper.EnsureUniqueAsync(getExisting, "unique-key", s => 5, currentId: 5)));
        }
        #endregion
    }
}
