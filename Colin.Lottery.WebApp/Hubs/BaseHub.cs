using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;


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

        protected static ConcurrentDictionary<string, object> UserSettings { get; set; } = new ConcurrentDictionary<string, object>();

        public override async Task OnConnectedAsync()
        {
            Interlocked.Increment(ref _usersCount);
            await Groups.AddToGroupAsync(Context.ConnectionId, typeof(T).Name);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Interlocked.Decrement(ref _usersCount);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, typeof(T).Name);
            UserSettings.TryRemove(Context.ConnectionId, out var settings);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
