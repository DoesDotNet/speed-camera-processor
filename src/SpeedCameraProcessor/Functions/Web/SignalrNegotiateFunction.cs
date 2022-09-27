using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs;
using Azure;
using System.Security.Principal;

namespace SpeedCameraProcessor.Functions.Web
{
    public static class SignalrNegotiateFunction
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
           [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
           [SignalRConnectionInfo(HubName = "serverless")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
