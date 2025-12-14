using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Explorer.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
	public override async Task OnConnectedAsync()
	{
		var userId = Context.User?
			.Claims
			.FirstOrDefault(c =>
				c.Type == "personId" ||
				c.Type == ClaimTypes.NameIdentifier
			)?.Value;

		if (!string.IsNullOrEmpty(userId))
		{
			await Groups.AddToGroupAsync(
				Context.ConnectionId,
				userId
			);
		}

		await base.OnConnectedAsync();
	}
}
