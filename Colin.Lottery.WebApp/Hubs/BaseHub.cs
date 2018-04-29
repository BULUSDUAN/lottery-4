using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.Models;


namespace Colin.Lottery.WebApp.Hubs
{
    public abstract class BaseHub<T>
    : Hub
    where T : BaseHub<T>
    {
        /// <summary>
        /// 在线用户统计
        /// </summary>
        protected static int _usersCount = 0;

        /// <summary>
        /// 用户配置
        /// </summary>

        public static ConcurrentDictionary<string, object> UserSettings { get; set; } = new ConcurrentDictionary<string, object>();

        public async override Task OnConnectedAsync()
        {
            Interlocked.Increment(ref _usersCount);
            await Groups.AddAsync(Context.ConnectionId, typeof(T).Name);

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            Interlocked.Decrement(ref _usersCount);
            await Groups.RemoveAsync(Context.ConnectionId, typeof(T).Name);
            UserSettings.TryRemove(Context.ConnectionId, out object settings);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
