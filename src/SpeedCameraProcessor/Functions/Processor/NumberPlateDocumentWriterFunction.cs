using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions.Processor;

public static class NumberPlateDocumentWriterFunction
{
    [FunctionName("NumberPlateDocumentWriter")]
    [return: CosmosDB(
        "%DatabaseName%",
        "%CollectionName%",
        ConnectionStringSetting = "CosmosConnectionString")]
    public static SpeederDocument Run(
        [QueueTrigger("%NumberPlateQueueName%", Connection = "SpeedCameraStore")] NumberPlateMessage numberPlateMessage,
        [CosmosDB(
            "%DatabaseName%",
            "%CollectionName%",
            ConnectionStringSetting = "CosmosConnectionString",
            Id ="{Id}",
            PartitionKey = "{Id}"
        )] SpeederDocument speederDocument,
        ILogger log)
    {
        log.LogInformation($"NumberPlateDocumentWriter {numberPlateMessage.Id}, {numberPlateMessage.NumberPlate}");

        if (speederDocument == null)
            return null;

        speederDocument.NumberPlate = numberPlateMessage.NumberPlate;
        speederDocument.Processed = true;
        speederDocument.Failed = numberPlateMessage.MatchingFailed;
        speederDocument.Processing = false;
        return speederDocument;
    }
}