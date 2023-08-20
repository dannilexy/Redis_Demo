using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisDemoAPI.Data.Extensions;

public static class DistributedCacheExtension
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache,
        string recordId, T record,
        TimeSpan? absoluteExpiredTime = null,
        TimeSpan? unUsedExpiredTime = null)
    {
        var options = new DistributedCacheEntryOptions();

        options.AbsoluteExpirationRelativeToNow = absoluteExpiredTime ?? TimeSpan.FromMinutes(2);
        options.SlidingExpiration = unUsedExpiredTime;

        var jsonData = JsonSerializer.Serialize(record);
        await cache.SetStringAsync(recordId, jsonData, options);
    }

    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);

        if (jsonData == null)
            return default(T);
        return JsonSerializer.Deserialize<T>(jsonData);
    }
}
