using Application.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers
{
    public class SubscriptionController : BaseController
    {
        private readonly ISubcriptionService _subscriptionService;
        public SubscriptionController(ISubcriptionService subcriptionService)
        {
            _subscriptionService = subcriptionService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SubscriptionPlan()
        {
            var listPlan=await _subscriptionService.GetAllSubscriptionAsync();
            return Ok(listPlan);
        }
        [HttpPatch]
        public async Task<IActionResult> ExtendSubscription()
        {
            var isUpdated = await _subscriptionService.ExtendSubscription();
            if(isUpdated)
            {
                return Ok(isUpdated);
            }
            return BadRequest(isUpdated);
        }
    }
}
