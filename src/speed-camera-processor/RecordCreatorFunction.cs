using System;
using System.IO;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace speed_camera_processor
{
    public static class RecordCreatorFunction
    {
        [FunctionName("RecordCreator")]
        [return: CosmosDB(databaseName: "Police", collectionName: "Speeders", ConnectionStringSetting = "SpeederCosmos")]
        public static SpeederDocument Run(
            [BlobTrigger("incoming/{name}", Connection = "SpeederStorage")] Stream incomingImage,
            string name, ILogger log)
        {
            log.LogInformation($"RecordCreator {name}");

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            var id = Guid.Parse(fileNameWithoutExtension);

            var speederDoc = new SpeederDocument
            {
                Id = id,
                OriginalFileName = name
            };
            return speederDoc;
        }
    }
}
