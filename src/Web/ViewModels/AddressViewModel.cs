using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopWeb.Web.ViewModels
{
    public class AddressViewModel
    {
        [Required]
        public String Street { get; set; }

        [Required]
        public String City { get; set; }

        [Required]
        public String State { get; set; }

        [Required]
        public String Country { get; set; }

        [Required]
        public String ZipCode { get; set; }
    }
}