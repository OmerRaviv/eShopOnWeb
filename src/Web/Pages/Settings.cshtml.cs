using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Microsoft.eShopWeb.Web.Pages
{
    public class SettingsModel : PageModel
    {
        public bool AgentSettingFound { get; private set; } = false;

        private IHostingEnvironment _hostingEnvironment;

        public SettingsModel(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("OzCode_Agent_Token"))) {
                AgentSettingFound = true;
            }
        }

        public bool IsProduction
        {
            get
            {
                return _hostingEnvironment.IsProduction();
            }
        }

        public bool IsStaging
        {
            get
            {
                return _hostingEnvironment.IsStaging();
            }
        }

        public bool IsDevelopment
        {
            get
            {
                return _hostingEnvironment.IsDevelopment();
            }
        }
    }
}