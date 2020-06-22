using Flaky.Data;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Pages.Basket;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Interfaces
{
    public interface IBillingService
    {
        Task<List<string>> GetAvailalbeShippingStates();

        Task<ChargeDetails> PrepareOrderForPayment(Order order, AddressViewModel address, PaymentDetailsViewModel payment);

        Task<Transcation> CompleteCharge(ChargeDetails detalis);
    }
}
