using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VueCore.Events;
using VueCore.Models;
using VueCore.Models.Domain;
using VueCore.Services;

namespace VueCore.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IMediator _mediatr;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ILogger<PaymentController> logger, IMediator mediatr, IPaymentService paymentService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediatr = mediatr;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("payment/schedule")]
        public async Task<IActionResult> SchedulePayment([FromBody] PaymentViewModel model, CancellationToken token)
        {
            if(model == null)
            {
                return Ok(new {Success = false});
            }
            // get user model
            var appUser = await _userManager.GetUserAsync(User);
            // save payment 
            var payment = await _paymentService.SchedulePaymentAsync(appUser.Id, model.Amount, model.ScheduledAt, token);
            // Publish event
            await _mediatr.Publish(new SchedulePayment(payment, appUser), token);

            return Ok(new {Success = true});
        }
    }
}
