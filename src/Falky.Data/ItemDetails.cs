using System.ComponentModel.DataAnnotations;

namespace Flaky.Data
{
    public class ItemDetails
    {
        public int ItemNumber { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public bool Taxable { get; set; }

        public float? Discount { get; set; }

        public decimal LineTotal { get; set; }
    }
}