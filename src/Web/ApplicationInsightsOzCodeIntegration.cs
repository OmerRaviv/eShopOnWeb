using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using OzCode.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web
{
    internal class ApplicationInsightsOzCodeIntegration
    {

        private readonly RequestDelegate _next;
        private readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsOzCodeIntegration(RequestDelegate next, TelemetryClient telemetryClient)
        {
            _next = next;
            _telemetryClient = telemetryClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var activty = System.Diagnostics.Activity.Current;

            if (activty != null)
            {
                OzCodeContext.Current.SetTag("TraceID", activty.TraceId);
                OzCodeContext.Current.SetTag("SpanId", activty.SpanId);
            }

            OzCodeIntergations.OnTracepointHit.Register("ApplicationInsight", (tracepointHit) =>
            {                
                var trace = new TraceTelemetry(tracepointHit.Message, SeverityLevel.Information)
                {
                    Timestamp = tracepointHit.Timestamp
                };

                foreach (var prop in tracepointHit.ContexualData)
                {
                    trace.Properties.Add(prop.Key, prop.Value);
                }

                trace.Properties.Add("Method", tracepointHit.MethodName);
                trace.Properties.Add("Class", tracepointHit.ClassName);
                trace.Properties.Add("Module", tracepointHit.ModuleName);
                trace.Properties.Add("TracepointUrl", tracepointHit.TracepointHitUrl);
                
                foreach (var messageProp in tracepointHit.MessageProperties)
                {
                    trace.Properties.Add(messageProp.Key, messageProp.Value);
                }
                
                _telemetryClient.TrackTrace(trace);
            });

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}