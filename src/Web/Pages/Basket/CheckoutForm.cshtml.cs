using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Pages.Basket
{
    public class CheckoutFormModel : PageModel
    {
        private IBasketService _basketService;
        private SignInManager<ApplicationUser> _signInManager;
        private IOrderService _orderService;
        private IBillingService _billingService;
        private IBasketViewModelService _basketViewModelService;
        private string _username;

        public CheckoutFormModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            SignInManager<ApplicationUser> signInManager,
            IOrderService orderService,
            IBillingService billingService)
        {
            _basketService = basketService;
            _signInManager = signInManager;
            _orderService = orderService;
            _billingService = billingService;
            _basketViewModelService = basketViewModelService;
        }

        public BasketViewModel BasketModel { get; set; } = new BasketViewModel();

        [BindProperty]
        public AddressViewModel ShippingAddress { get; set; }

        [BindProperty]
        public PaymentDetailsViewModel PaymentDetails { get; set; }

        public PaymentConfirmation PaymentConfirmation { get; set; } = null;

        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem> {
            new SelectListItem("American Express","amex"),
            new SelectListItem("Diners","diners"),
            new SelectListItem("Discover","discover"),
            new SelectListItem("MasterCard","mastercard"),
            new SelectListItem("Visa","visa")
        };

        public async Task OnGet()
        {
            await SetBasketModelAsync();
        }

        private async Task SetBasketModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetBasketCookieAndUserName();
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            }
        }

        public async Task<IActionResult> OnPost(Dictionary<string, int> items)
        {
            await SetBasketModelAsync();

            await _basketService.SetQuantities(BasketModel.Id, items);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostComplete()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await SetBasketModelAsync();

            var order = await _orderService.CreateOrderAsync(BasketModel.Id, new Address(ShippingAddress.Street, ShippingAddress.City, ShippingAddress.State, ShippingAddress.Country, ShippingAddress.ZipCode));

            await _basketService.DeleteBasketAsync(BasketModel.Id);

            var transcation = await AuthorizePayment(order);

            if (transcation.Status == Flaky.Data.TranscationStatus.Completed)
            {
                order = await _orderService.AddPaymentConfirmation(order.Id, new PaymentConfirmation(transcation.Timestamp, "card", transcation.ID.ToString()));

                PaymentConfirmation = order.PaymentConfirmation;
            } 
            else
            {
                ModelState.AddModelError("Payment", "Payment failed");
            }

            return Page();
        }

        private async Task<Flaky.Data.Transcation> AuthorizePayment(Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Order order)
        {
            var chardeDetails = await _billingService.PrepareOrderForPayment(order, ShippingAddress, PaymentDetails);

            var transaction = await _billingService.CompleteCharge(chardeDetails);

            return transaction;
        }

        private void GetOrSetBasketCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                _username = Request.Cookies[Constants.BASKET_COOKIENAME];
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
        }
    }
}