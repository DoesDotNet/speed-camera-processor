﻿using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions.Web
{
    public static class HistoryFunction
    {
        [FunctionName("History")]
        public static async Task<IActionResult> History(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "History")] HttpRequest req,
             [CosmosDB(
                "%DatabaseName%",
                "%CollectionName%",
                ConnectionStringSetting = "CosmosConnectionString",
                SqlQuery = "SELECT TOP 10 * FROM c WHERE c.Processed = true ORDER BY c._ts DESC")]
                IEnumerable<SpeederDocument> speeders,
            ILogger log)
        {
            return new OkObjectResult(speeders);
        }
    }
}