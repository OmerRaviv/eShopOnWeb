using System.ComponentModel.DataAnnotations;

namespace Flaky.Data
{
    public class PaymentDetails
    {
        [Required]
        public string Brand { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public int ExperationMonth { get; set; }

        [Required]
        public int ExperationYear { get; set; }

        [Required]
        public string CVC { get; set; }

        [Required]
        public string CardHolderName { get; set; }
    }
}