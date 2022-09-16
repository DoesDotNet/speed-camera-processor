using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SpeedCameraProcessor.OldFunctions;

public static class ImageReaderFunction
{
    [FunctionName("ImageReader")]
    public static async Task Run(
        [QueueTrigger("speeder-to-process", Connection = "SpeederStorage")] string speederId,
        [Blob("incoming/{queueTrigger}.png", FileAccess.Read, Connection = "SpeederStorage")] Stream speederImage,
        [Queue("number-plates", Connection = "SpeederStorage")] IAsyncCollector<NumberPlateMessage> collector,
        ILogger log)
    {
        var key = Environment.GetEnvironmentVariable("VisionKey");
        var endpoint = Environment.GetEnvironmentVariable("VisionEndpoint");

        ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
        { 
            Endpoint = endpoint 
        };

        var headers = await client.ReadInStreamAsync(speederImage);
        string operationLocation = headers.OperationLocation;

        await Task.Delay(2000);

        const int numberOfCharsInOperationId = 36;
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        // Extract the text
        ReadOperationResult results;
        log.LogInformation($"Extracting text from {speederId}");

        do
        {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        }
        while (results.Status == OperationStatusCodes.Running ||
               results.Status == OperationStatusCodes.NotStarted);

        if(!results.AnalyzeResult.ReadResults.Any())
        {
            return;
        }

        string numberPlate = results.AnalyzeResult.ReadResults[0].Lines.First().Text;
        var message = new NumberPlateMessage
        {
            Id = speederId,
            NumberPlate = numberPlate
        };

        await collector.AddAsync(message);
    }
}