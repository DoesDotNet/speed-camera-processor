using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SpeedCameraProcessor.Functions.Web
{
    public static class SignalrAliveFunction
    {
        //[FunctionName("Alive")]
        //public static async Task Alive([TimerTrigger("*/4 * * * * *")] TimerInfo myTimer,
        //[SignalR(HubName = "serverless")] IAsyncCollector<SignalRMessage> signalRMessages)
        //{
        //    await signalRMessages.AddAsync(
        //    new SignalRMessage
        //    {
        //        Target = "alive",
        //        Arguments = new[] { $"{DateTime.UtcNow.ToString("r")}" }
        //    });
        //}
    }
}
