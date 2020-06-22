using Flaky.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flaky.API.Services
{
    public static class StateTaxService
    {
        public class StateTaxDetails
        {
            public List<TaxRate> Taxes { get; set; }
            public StateTaxDetails(params TaxRate[] taxes)
            {
                Taxes = new List<TaxRate>(taxes);
            }
        }

        public const string StateTax = "State Sale Tax";
        public const string GST = "GST";
        public const string HST = "HST";
        public const string PST = "PST";

        private static Dictionary<string, StateTaxDetails> _statesDetails = new Dictionary<string, StateTaxDetails>()
        {
            { "Alabama", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "Alaska", new StateTaxDetails() },
            { "Arizona", new StateTaxDetails(new TaxRate(StateTax, 5.6M)) },
            { "Arkansas", new StateTaxDetails(new TaxRate(StateTax, 6.5M)) },
            { "California", new StateTaxDetails(new TaxRate(StateTax, 7.5M)) },
            { "Colorado", new StateTaxDetails(new TaxRate(StateTax, 2.9M)) },
            { "Connecticut", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "Delaware", new StateTaxDetails() },
            { "District of Columbia", new StateTaxDetails(new TaxRate(StateTax, 5.75M)) },
            { "Florida", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Georgia", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "Hawaii", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "Idaho", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Illinois", new StateTaxDetails(new TaxRate(StateTax, 6.25M)) },
            { "Indiana", new StateTaxDetails(new TaxRate(StateTax, 7M)) },
            { "Iowa", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Kansas", new StateTaxDetails(new TaxRate(StateTax, 6.5M)) },
            { "Kentucky", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Louisiana", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "Maine", new StateTaxDetails(new TaxRate(StateTax, 5.5M)) },
            { "Maryland", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Massachusetts", new StateTaxDetails(new TaxRate(StateTax, 6.25M)) },
            { "Michigan", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Minnesota", new StateTaxDetails(new TaxRate(StateTax, 6.88M)) },
            { "Mississippi", new StateTaxDetails(new TaxRate(StateTax, 7M)) },
            { "Missouri", new StateTaxDetails(new TaxRate(StateTax, 4.23M)) },
            { "Montana", new StateTaxDetails() },
            { "Nebraska", new StateTaxDetails(new TaxRate(StateTax, 5.5M)) },
            { "Nevada", new StateTaxDetails(new TaxRate(StateTax, 6.85M)) },
            { "New Hampshire", new StateTaxDetails() },
            { "New Jersey", new StateTaxDetails(new TaxRate(StateTax, 7M)) },
            { "New Mexico", new StateTaxDetails(new TaxRate(StateTax, 5.13M)) },
            { "New York", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "North Carolina", new StateTaxDetails(new TaxRate(StateTax, 4.75M)) },
            { "North Dakota", new StateTaxDetails(new TaxRate(StateTax, 5M)) },
            { "Ohio", new StateTaxDetails(new TaxRate(StateTax, 5.75M)) },
            { "Oklahoma", new StateTaxDetails(new TaxRate(StateTax, 4.5M)) },
            { "Oregon", new StateTaxDetails() },
            { "Pennsylvania", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Puerto Rico", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Rhode Island", new StateTaxDetails(new TaxRate(StateTax, 7M)) },
            { "South Carolina", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "South Dakota", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "Tennessee", new StateTaxDetails(new TaxRate(StateTax, 7M)) },
            { "Texas", new StateTaxDetails(new TaxRate(StateTax, 6.25M)) },
            { "Utah", new StateTaxDetails(new TaxRate(StateTax, 5.95M)) },
            { "Vermont", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Virginia", new StateTaxDetails(new TaxRate(StateTax, 5.3M)) },
            { "Washington", new StateTaxDetails(new TaxRate(StateTax, 6.5M)) },
            { "West Virginia", new StateTaxDetails(new TaxRate(StateTax, 6M)) },
            { "Wisconsin", new StateTaxDetails(new TaxRate(StateTax, 5M)) },
            { "Wyoming", new StateTaxDetails(new TaxRate(StateTax, 4M)) },
            { "Alberta", new StateTaxDetails(new TaxRate("GST", 5M)) },
            { "British Columbia", new StateTaxDetails(new TaxRate("GST", 5M), new TaxRate("PST", 7M)) },
            { "Manitoba", new StateTaxDetails(new TaxRate("GST", 5M), new TaxRate("PST", 7M)) },
            { "New Brunswick", new StateTaxDetails(new TaxRate("HST", 15M)) },
            { "Newfoundland", new StateTaxDetails(new TaxRate("HST", 15M)) },
            { "Labrador", new StateTaxDetails(new TaxRate("HST", 15M)) },
            { "Northwest Territories", new StateTaxDetails(new TaxRate("GST", 5M)) },
            { "Nova Scotia", new StateTaxDetails(new TaxRate("HST", 15M)) },
            { "Nunavut", new StateTaxDetails(new TaxRate("GST", 5M)) },
            { "Ontario", new StateTaxDetails(new TaxRate("HST", 13M)) },
            { "Prince Edward Island", new StateTaxDetails(new TaxRate("HST", 15M)) },
            { "Quebec", new StateTaxDetails(new TaxRate("GST", 5M), new TaxRate("QST", 9.975M)) },
            { "Saskatchewan", new StateTaxDetails(new TaxRate("GST", 5M), new TaxRate("PST", 6M)) },
            { "Yukon", new StateTaxDetails(new TaxRate("GST", 5M)) }
        };
        public static ActionResult<IEnumerable<string>> States
        {
            get
            {
                return _statesDetails.Keys;
            }
        }

        public static List<TaxRate> TaxForState(string state)
        {
            return _statesDetails[state].Taxes;
        }
    }
}
