using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Caching;

public class NotificationCacheService(IConnectionMultiplexer redis) : INotificationCacheService
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
}
