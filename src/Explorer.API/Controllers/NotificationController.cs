using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("my")]
    public ActionResult<List<NotificationDto>> GetMyNotifications()
    {
        var userId = GetUserId();
        var result = _notificationService.GetMyNotifications(userId);
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public ActionResult<UnreadCountDto> GetUnreadCount()
    {
        var userId = GetUserId();
        var result = _notificationService.GetUnreadCount(userId);
        return Ok(result);
    }

    [HttpPut("{id}/mark-read")]
    public ActionResult<NotificationDto> MarkAsRead(long id)
    {
        var userId = GetUserId();

        try
        {
            var result = _notificationService.MarkAsRead(id, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }

    [HttpPut("mark-all-read")]
    public ActionResult<MarkAllReadResultDto> MarkAllAsRead()
    {
        var userId = GetUserId();
        var result = _notificationService.MarkAllAsRead(userId);
        return Ok(result);
    }

    // Helper metoda za ekstrakciju User ID iz JWT tokena
    private long GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);
        if (claim == null || !long.TryParse(claim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or personId claim is missing.");
        }
        return userId;
    }
}