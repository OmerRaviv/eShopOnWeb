using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flaky.API.Services;
using Flaky.Data;
using Microsoft.AspNetCore.Mvc;

namespace Flaky.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeController : ControllerBase
    {
        // POST api/charge
        [HttpPost]
        public ActionResult<Transcation> Complete([FromBody] string encodedCharge)
        {
            var charge = EncryptionService.Dercypt<ChargeDetails>(encodedCharge);

            if (charge.Items == null)
            {
                return BadRequest("items must be set");
            }

            foreach(var item in charge.Items)
            {  
                if (item.LineTotal < 0)
                {
                    return BadRequest("Item line can't be negative");
                }
            }

            if (charge.Taxes == null)
            {
                return BadRequest("taxes must be set");
            }

            foreach (var tax in charge.Taxes)
            {
                if (tax.Amount <= 0)
                {
                    return BadRequest("Tax line can't be zero");
                }
            }

            var result = new TranscationWithChargeDetails()
            {
                ID = Guid.NewGuid(),
                ChargeID = charge.ID,
                ChargeDetails = charge,
                Amount = charge.TotalAmmount,
                Status = TranscationStatus.Completed,
                Timestamp = DateTime.UtcNow
            };

            return result;
        }

        [HttpGet]
        [Route("key")]
        public ActionResult<string> GetPublicKey()
        {
            return EncryptionService.PublicKey;
        }

        [HttpGet]
        [Route("e")]
        public ActionResult<string> EncryptMessage(string message)
        {
            return EncryptionService.Encrypt(message);
        }

        [HttpGet]
        [Route("d")]
        public ActionResult<string> DecryptMessage(string message)
        {
            return EncryptionService.Dercypt(message);
        }
    }

    class TranscationWithChargeDetails : Transcation
    {
        public ChargeDetails ChargeDetails { get; set; }
    }
}
