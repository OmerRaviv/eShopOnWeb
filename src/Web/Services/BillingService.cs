using Flaky.Data;
using Flaky.SDK;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Services
{
    public class BillingService : IBillingService
    {
        private Billing _billingClient;

        public BillingService(FlakyBillingConfiguration configuration)
        {
            _billingClient = new Flaky.SDK.Billing(null, configuration);
        }

        public async Task<Transcation> CompleteCharge(ChargeDetails detalis)
        {
            return await _billingClient.CreateCharge(detalis);
        }

        public async Task<List<string>> GetAvailalbeShippingStates()
        {
            return await _billingClient.GetSupportedStates();
        }


        public async Task<ChargeDetails> PrepareOrderForPayment(Order order, AddressViewModel shippingAddress, PaymentDetailsViewModel paymentDetails)
        {
            var details = new ChargeDetails
            {
                ID = order.Id,
                Items = order.OrderItems.Select(item => new ItemDetails()
                {
                    Name = item.ItemOrdered.ProductName,
                    ItemNumber = item.ItemOrdered.CatalogItemId,
                    UnitPrice = item.UnitPrice,
                    Amount = item.Units,
                    LineTotal = item.UnitPrice * item.Units
                }).ToList(),
                Address = new Flaky.Data.Address() {
                    FirstLine = shippingAddress.Street,
                    City = shippingAddress.City,
                    State = shippingAddress.State,
                    Country = shippingAddress.Country,
                    ZipCode = shippingAddress.ZipCode
                },
                PaymentDetails = new Flaky.Data.PaymentDetails()
                {
                    Brand = paymentDetails.Brand,
                    CardNumber = paymentDetails.CardNumber,
                    CardHolderName = paymentDetails.CardHolderName,
                    CVC = paymentDetails.CVC,
                    ExperationMonth = paymentDetails.ExperationMonth,
                    ExperationYear = paymentDetails.ExperationYear
                }
            };

            details.Taxes = await _billingClient.CalculateTax(details);
            details.TotalAmmount = details.Items.Sum(item => item.LineTotal) + details.Taxes.Sum(tax => tax.Amount);

            return details;
        }
    }
}
