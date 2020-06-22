namespace Flaky.Data
{
    public class TaxLine
    {
        public string TaxCode { get; set; }

        public string Description { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount { get; set; }      
    }
}