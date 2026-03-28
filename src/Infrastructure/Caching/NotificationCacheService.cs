using System.Text.Json;
using Application.Abstractions.Authentication;
using Application.DTOs;
using Application.Interfaces;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Infrastructure.Caching;

public class NotificationCacheService(IConnectionMultiplexer redis, IUserContext userContext) : INotificationCacheService
{
    private const int MaxNotificationsPerUser = 50;

    public async Task AddNotificationAsync(Guid userId, NotificationItem notification)
    {        
        IDatabase db = redis.GetDatabase();
        string key = $"notifications:user:{userId}";
        string json = JsonSerializer.Serialize(notification);

        await db.ListLeftPushAsync(key, json);

        await db.ListTrimAsync(key, 0, MaxNotificationsPerUser - 1);

        await db.KeyExpireAsync(key, TimeSpan.FromDays(7));
    }

    public async Task<List<NotificationItem>> GetNotificationsAsync(Guid userId)
    {
        IDatabase db = redis.GetDatabase();
        string key = $"notifications:user:{userId}";

        RedisValue[] redisValues = await db.ListRangeAsync(key, 0, -1);

        if (redisValues.Length == 0)
        {
            return new List<NotificationItem>();
        }            

        return redisValues
            .Select(val => JsonSerializer.Deserialize<NotificationItem>(val.ToString()) ?? new())
            .ToList();
    }

    public async Task MarkNoficationAsRead(Guid? notificationId = null)
    {
        Guid userId = userContext.UserId;

        IDatabase db = redis.GetDatabase();
        string key = $"notifications:user:{userId}";

        RedisValue[] redisValues = await db.ListRangeAsync(key, 0, -1);

        if (redisValues.Length == 0)
        {
            return;
        }

        if (notificationId.HasValue)
        {
            for (int i = 0; i < redisValues.Length; i++)
            {
                NotificationItem notification = JsonSerializer.Deserialize<NotificationItem>(redisValues[i].ToString());
                if (notification?.Id == notificationId)
                {
                    notification.IsRead = true;
                    await db.ListSetByIndexAsync(key, i, JsonSerializer.Serialize(notification));
                    break;
                }
            }
            return;
        }

        RedisValue[] updated = redisValues
           .Select(v => JsonSerializer.Deserialize<NotificationItem>(v.ToString()) ?? new())
           .Select(n => { n.IsRead = true; return n; })
           .Select(n => (RedisValue)JsonSerializer.Serialize(n))
           .ToArray();

        ITransaction tx = db.CreateTransaction();
        _ = tx.KeyDeleteAsync(key);
        _ = tx.ListRightPushAsync(key, updated);
        _ = tx.KeyExpireAsync(key, TimeSpan.FromDays(7));
        await tx.ExecuteAsync();
    }
}
