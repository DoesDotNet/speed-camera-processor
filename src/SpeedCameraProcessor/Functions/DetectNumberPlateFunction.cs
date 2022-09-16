using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions;

public static class DetectNumberPlateFunction
{
    [FunctionName("DetectNumberPlate")]
    [return: Queue("crops", Connection = "SpeedCameraStore")]
    public static async Task<CropNumberPlateMessage> Run([BlobTrigger("speeders/{name}", Connection = "SpeedCameraStore")] Stream photoStream, string name, ILogger log)
    {
        log.LogInformation("Image number plate finding triggered for file {Name}", name);

        if (name.StartsWith("plates"))
        {
            return null;
        }

        string endpoint = Environment.GetEnvironmentVariable("CustomVisionEndpoint");
        string key = Environment.GetEnvironmentVariable("CustomVisionKey");
        string tagName = Environment.GetEnvironmentVariable("CustomVisionTagName");
        string modelName = Environment.GetEnvironmentVariable("CustomVisionModelName");
        Guid projectId = Guid.Parse(Environment.GetEnvironmentVariable("CustomVisionProjectId"));

        var client = AuthenticatePrediction(endpoint, key);

        ImagePrediction result;

        try
        {
            result = await client.DetectImageWithNoStoreAsync(projectId, modelName, photoStream);
        }
        catch (CustomVisionErrorException ex)
        {
            log.LogError(ex, "Error detecting number plate");
            return null;
        }

        var plate = result.Predictions.FirstOrDefault(x => x.Probability > 0.75 && x.TagName == tagName);
        if (plate != null)
        {
            log.LogInformation("Plate found for {Name}", name);
            CropNumberPlateMessage cropNumberPlateMessage = new CropNumberPlateMessage(name,
                plate.BoundingBox.Top,
                plate.BoundingBox.Left,
                plate.BoundingBox.Width,
                plate.BoundingBox.Height
            );

            return cropNumberPlateMessage;
        }

        return null;
    }

    private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
    {
        CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };
        return predictionApi;
    }
}
