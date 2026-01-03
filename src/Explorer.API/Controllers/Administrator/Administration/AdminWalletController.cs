using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administrator/wallet")]
    public class AdminWalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly INotificationService _notificationService;

        public AdminWalletController(IWalletService walletService, INotificationService notificationService)
        {
            _walletService = walletService;
            _notificationService = notificationService;
        }

        [HttpPost("topup")]
        public ActionResult<WalletDto> TopUp([FromBody] WalletTopUpDto dto)
        {
            var adminPersonId = GetPersonId();
            var result = _walletService.TopUp(adminPersonId, dto.TouristUserId, dto.AmountAc);

            _notificationService.CreateWalletTopUpNotification(result.PersonId, dto.AmountAc);

            return Ok(result);
        }

        private long GetPersonId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);
            if (claim == null || !long.TryParse(claim.Value, out var id))
                throw new UnauthorizedAccessException("personId claim is missing.");
            return id;
        }
    }
}
