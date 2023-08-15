namespace userMS.Application.Services
{
    public interface IRedisCacheService
    {
        Task<T> GetAsync<T>(string key);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        Task<bool> DeleteAsync(string key);

        Task<bool> HasKeyValue(string key);
    }
}
