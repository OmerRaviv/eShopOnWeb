using System;

namespace Flaky.Data
{
    public class TaxRate
    {
        public string Description { get; set; }
        public decimal Rate { get; set; }

        public string TaxCode { get; set; }

        public TaxRate(string description, decimal rate) : this(description, rate, description)
        {
        }

        public TaxRate(string description, decimal rate, string taxCode)
        {
            Description = description;
            TaxCode = taxCode;
            Rate = rate;
        }

        public TaxLine Calculate(decimal amount)
        {
            return new TaxLine()
            {
                Rate = Rate,
                Amount = Math.Round(amount * Rate / 100M, 2, MidpointRounding.ToEven),
                Description = Description,
                TaxCode = TaxCode
            };
        }
    }
}