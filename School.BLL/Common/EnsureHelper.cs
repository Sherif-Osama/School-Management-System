namespace School.BLL.Common
{
    public static class EnsureHelper
    {

        public static async Task EnsureUniqueAsync<TKey, T>(Func<TKey, Task<T?>> getExisting, TKey key, Func<T, int>? getId = null, int? currentId = null)
        {
            T? existing = await getExisting(key);

            if (existing == null)
                return;

            if (currentId.HasValue && getId?.Invoke(existing) == currentId)
                return;

            throw new InvalidOperationException($"'{key}' already exists.");
        }

        public static async Task EnsureExistsAsync<TId>(Func<TId, Task<bool>> existsFunc, TId id, string entityName)
        {
            if (!await existsFunc(id))
                throw new KeyNotFoundException($"{entityName} with ID {id} does not exist.");
        }
    }
}