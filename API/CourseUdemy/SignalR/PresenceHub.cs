using CourseUdemy.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;

namespace CourseUdemy.SignalR
{
    [Authorize]
    public class PresenceHub :Hub
    {
        public presenceTracer _tracer { get; }
        public PresenceHub(presenceTracer tracer)
        {
            _tracer = tracer;
        }


        public override  async Task OnConnectedAsync ( )
        {
            await _tracer.UserConnected (Context.User.GetUsername (), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOnline",Context.User.GetUsername());
            var currentUsers=await _tracer.GetOnlineUser();
            await Clients.All.SendAsync ("GetOnlineUsers", currentUsers);
        }
        public override async Task OnDisconnectedAsync ( Exception? exception )
        {
            await _tracer.UserDisConnected (Context.User.GetUsername (), Context.ConnectionId);
            await Clients.Others.SendAsync ("UserIsOffline", Context.User.GetUsername ());
            var currentUsers=await _tracer.GetOnlineUser();
            await Clients.All.SendAsync ("GetOnlineUsers", currentUsers);
            await base.OnDisconnectedAsync (exception);
        }
    }
}
