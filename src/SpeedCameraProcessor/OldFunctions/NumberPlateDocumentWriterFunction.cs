using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SpeedCameraProcessor.OldFunctions;

public static class NumberPlateDocumentWriterFunction
{
    [FunctionName("NumberPlateDocumentWriter")]
    [return: CosmosDB("Police", "Speeders", ConnectionStringSetting = "SpeederCosmos")]
    public static SpeederDocument Run(
        [QueueTrigger("number-plates", Connection = "SpeederStorage")] NumberPlateMessage numberPlateMessage,
        [CosmosDB(
            "Police",
            "Speeders", 
            ConnectionStringSetting = "SpeederCosmos",
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
        return speederDocument;
    }
}