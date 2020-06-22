using System.Collections.Generic;
using System.Net.Sockets;

namespace Flaky.Data
{
    public class ChargeDetails
    {
        public int ID { get; set; }

        public string ClientName { get; set; }

        public Address Address { get; set; }

        public List<ItemDetails> Items { get; set; }

        public List<TaxLine> Taxes { get; set; }

        public decimal TotalAmmount { get; set; }

        public string Currency { get; set; }

        public PaymentDetails PaymentDetails { get; set; }
    }
}