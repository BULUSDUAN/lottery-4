using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.Models;


namespace Colin.Lottery.WebApp.Hubs
{
    public abstract class BaseHub : Hub
    {
        /// <summary>
        /// 用户统计
        /// </summary>
        protected static int _usersCount = 0;

        /// <summary>
        /// 在线用户
        /// </summary>
        protected static ConcurrentDictionary<string, object> _users = new ConcurrentDictionary<string, object>();

        public async override Task OnConnectedAsync()
        {
            Interlocked.Increment(ref _usersCount);
            _users[Context.ConnectionId] = new UserData(Context.ConnectionId, $"user{_usersCount}");

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            Interlocked.Decrement(ref _usersCount);
            _users.TryRemove(Context.ConnectionId, out object user);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
