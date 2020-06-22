using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flaky.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Flaky.SDK
{
    public class Billing
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly FlakyBillingConfiguration _configuration;

        public Billing(FlakyBillingConfiguration configruation) : this(null, configruation)
        {
        }

        public Billing(ILogger logger, FlakyBillingConfiguration configruation)
        {
            _logger = logger;
            _configuration = configruation;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaxRate, Client.TaxRate>();
                cfg.CreateMap<Client.TaxRate, TaxRate>()
                   .ConstructUsing((Client.TaxRate taxRate) => new TaxRate(taxRate.Description, taxRate.Rate))
                   .ForMember(x => x.TaxCode, opt => opt.Ignore());
                cfg.CreateMap<Client.Transcation, Transcation>();
            });

            config.AssertConfigurationIsValid();

            _mapper = config.CreateMapper();
        }

        public async Task<List<string>> GetSupportedStates()
        {
            if (_configuration.Testing)
            {
                return new List<string> { "Alberta", "California", "Oregon" };
            }
            else if (!string.IsNullOrEmpty(_configuration.FlakyServerBaseUrl))
            {
                _logger?.LogInformation($"about to load supported states ");
                var taxClient = new Flaky.SDK.Client.TaxClient()
                {
                    BaseUrl = _configuration.FlakyServerBaseUrl
                };
                var states = await taxClient.GetAllAsync();

                _logger?.LogInformation($"supported states loaded");

                return states.ToList();
            }
            else
            {
                throw new MissingConfiguration("Please set configuration as Testing or set ServerBaseUrl");
            }
        }

        public async Task<List<TaxLine>> CalculateTax(ChargeDetails details)
        {
            var taxableAmount = 0M;
            foreach (var item in details.Items)
            {
                if (item.Taxable)
                {
                    taxableAmount += item.LineTotal;
                }
            }

            var taxRates = new List<TaxRate>();

            if (_configuration.Testing)
            {
                taxRates.Add(new TaxRate("Testing Tex", 5M));
            }
            else if (!string.IsNullOrEmpty(_configuration.FlakyServerBaseUrl))
            {
                _logger?.LogInformation($"about to load tax details from server for state {details.Address.State}");
                var taxClient = new Flaky.SDK.Client.TaxClient()
                {
                    BaseUrl = _configuration.FlakyServerBaseUrl
                };
                var stateRates = await taxClient.GetAsync(details.Address.State);

                _logger?.LogInformation($"tax details returned from server for state {details.Address.State}");

                taxRates.AddRange(stateRates.Select(rate => _mapper.Map<TaxRate>(rate)));
            }
            else
            {
                throw new MissingConfiguration("Please set configuration as Testing or set ServerBaseUrl");
            }

            return taxRates.Select(taxRate => taxRate.Calculate(taxableAmount)).ToList();
        }

        public async Task<Transcation> CreateCharge(ChargeDetails details)
        {
            if (details.Taxes == null)
            {
                _logger?.LogInformation($"charge details are missing taxes, calcuulating default default");
                details.Taxes = await CalculateTax(details);
            }

            details.TotalAmmount = details.Items.Sum(item => item.LineTotal) + details.Taxes.Sum(tax => tax.Amount);

            if (_configuration.Testing)
            {
                var result = new Transcation()
                {
                    ID = Guid.NewGuid(),
                    ChargeID = details.ID,
                    Amount = details.TotalAmmount,
                    Status = TranscationStatus.Completed,
                    Timestamp = DateTime.UtcNow
                };

                return result;
            }
            else if (!string.IsNullOrEmpty(_configuration.FlakyServerBaseUrl))
            {
                if (details.PaymentDetails == null)
                {
                    throw new InvalidOperationException("Can't complete charge without payment details");
                }

                _logger?.LogInformation($"about to complete charge ID {details.ID}");

                var chargeClient = new Flaky.SDK.Client.ChargeClient()
                {
                    BaseUrl = _configuration.FlakyServerBaseUrl
                };

                var key = Newtonsoft.Json.JsonConvert.DeserializeObject<EncryptionDetails>(await chargeClient.GetPublicKeyAsync());
                var encryption = Encryption.CreateEncryption(key);
                var encodedCharge = encryption.EncryptToBase64(Newtonsoft.Json.JsonConvert.SerializeObject(details));
                var transcation = await chargeClient.CompleteAsync(encodedCharge);

                _logger?.LogInformation($"got new transcation from server {transcation.Id} for charge {transcation.ChargeID}");

                return _mapper.Map<Transcation>(transcation);
            }
            else
            {
                throw new MissingConfiguration("Please set configuration as Testing or set ServerBaseUrl");
            }
        }
    }

    public class FlakyBillingConfiguration
    {
        public string FlakyServerBaseUrl { get; set; }

        public bool Testing { get; set; }
    }
}
