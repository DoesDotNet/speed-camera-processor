using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SpeedCameraProcessor.OldFunctions;

public static class DocumentProcessorFunction
{
    [FunctionName("DocumentProcessor")]
    public static async Task Run(
        [CosmosDBTrigger(
            "Police",
            "Speeders",
            ConnectionStringSetting = "SpeederCosmos",
            LeaseCollectionName = "SpeederLeases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> documents,
        [Queue("speeder-to-process", Connection = "SpeederStorage")] IAsyncCollector<string> collector,
        ILogger log)
    {
        foreach (Document document in documents)
        {
            log.LogInformation($"Processing document {document.Id}");

            var speederDocument = JsonConvert.DeserializeObject<SpeederDocument>(document.ToString());
            if (speederDocument.Processed)
                continue;

            await collector.AddAsync(speederDocument.Id.ToString());
        }
    }
}