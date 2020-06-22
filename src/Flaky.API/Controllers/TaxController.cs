using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flaky.Data;
using Flaky.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flaky.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        // GET api/tax/states
        [HttpGet]
        [Route("states")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return StateTaxService.States;
        }

        // GET api/tax/state
        [HttpGet("states/{state}")]
        public ActionResult<List<TaxRate>> Get(string state)
        {
            return StateTaxService.TaxForState(state);
        }
    }
}
