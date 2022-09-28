using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions.Processor;

public static class FindNumberPlateFunction
{
    [FunctionName("FindNumberPlate")]
    public static async Task Run(
        [BlobTrigger("speeders/{name}", Connection = Constants.StorageConnection )] Stream photoStream, string name,
        [Queue("crop-plate", Connection = Constants.StorageConnection)] IAsyncCollector<CropNumberPlateMessage> cropNumberPlateQueue,
        ILogger log)
    {
        log.LogInformation("Image number plate finding triggered for file {Name}", name);

        if (name.StartsWith("plates"))
        {
            return;
        }

        string endpoint = Environment.GetEnvironmentVariable("CustomVisionEndpoint");
        string key = Environment.GetEnvironmentVariable("CustomVisionKey");
        string tagName = Environment.GetEnvironmentVariable("CustomVisionTagName");
        string modelName = Environment.GetEnvironmentVariable("CustomVisionModelName");
        Guid projectId = Guid.Parse(Environment.GetEnvironmentVariable("CustomVisionProjectId"));

        CustomVisionPredictionClient predictionClient = 
            new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endpoint
        };

        ImagePrediction result;

        try
        {
            result = await predictionClient.DetectImageWithNoStoreAsync(projectId, modelName, photoStream);
        }
        catch (CustomVisionErrorException ex)
        {
            log.LogError(ex, "Error detecting number plate");
            return;
        }

        var plate = result.Predictions.FirstOrDefault(x => x.Probability > 0.75 && x.TagName == tagName);
        if (plate != null)
        {
            log.LogInformation("Plate found for {Name}", name);
            var cropNumberPlateMessage = new CropNumberPlateMessage(name,
                plate.BoundingBox.Top,
                plate.BoundingBox.Left,
                plate.BoundingBox.Width,
                plate.BoundingBox.Height
            );

            await cropNumberPlateQueue.AddAsync(cropNumberPlateMessage);
        }
    }
}
