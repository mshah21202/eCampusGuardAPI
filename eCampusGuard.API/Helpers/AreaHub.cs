using System;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace eCampusGuard.API.Helpers
{
    [Authorize(Policy = "RequireGateStaffRole")]
    public class AreaHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var idValue = Context.GetHttpContext().GetRouteValue("id");

            if (idValue == null)
            {
                Context.Abort();
                await base.OnConnectedAsync();
            }

            string id = idValue as string;

            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, id);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var idValue = Context.GetHttpContext().GetRouteValue("id");
            string id = idValue as string;

            var connectionId = Context.ConnectionId;
            await Groups.RemoveFromGroupAsync(connectionId, id);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

