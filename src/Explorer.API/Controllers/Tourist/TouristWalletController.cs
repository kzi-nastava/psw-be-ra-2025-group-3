using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/wallet")]
    public class TouristWalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public TouristWalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("my")]
        public ActionResult<WalletDto> GetMyWallet()
            => Ok(_walletService.GetMyWallet(GetPersonId()));

        private long GetPersonId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);
            if (claim == null || !long.TryParse(claim.Value, out var id))
                throw new UnauthorizedAccessException("personId claim is missing.");
            return id;
        }
    }
}
