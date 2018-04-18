using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Colin.Lottery.Models;
using Microsoft.AspNet.SignalR;

namespace Colin.Lottery.StrategyService
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

        public async override Task OnConnected()
        {
            Interlocked.Increment(ref _usersCount);
            _users[Context.ConnectionId] = new UserData(Context.ConnectionId, $"user{_usersCount}");

            await base.OnConnected();
        }

        public async override Task OnDisconnected(bool stopCalled)
        {
            Interlocked.Decrement(ref _usersCount);
            _users.TryRemove(Context.ConnectionId, out object user);

            await base.OnDisconnected(stopCalled);
        }
    }
}
