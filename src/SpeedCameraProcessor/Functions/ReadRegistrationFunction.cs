using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions;

public static class ReadRegistrationFunction
{
    private static readonly string NumberPlateRegEx =
        "(^[A-Z]{2}[0-9]{2}\\s?[A-Z]{3}$)|(^[A-Z][0-9]{1,3}[A-Z]{3}$)|(^[A-Z]{3}[0-9]{1,3}[A-Z]$)|(^[0-9]{1,4}[A-Z]{1,2}$)|(^[0-9]{1,3}[A-Z]{1,3}$)|(^[A-Z]{1,2}[0-9]{1,4}$)|(^[A-Z]{1,3}[0-9]{1,3}$)|(^[A-Z]{1,3}[0-9]{1,4}$)|(^[0-9]{3}[DX]{1}[0-9]{3}$)";
        
    [FunctionName("ReadRegistration")]
    [return: Queue("details", Connection = "SpeedCameraStore")]
    public static async Task<SaveSpeederDetailsMessage> Run([BlobTrigger("speeders/plates/{name}", Connection = "SpeedCameraStore")] Stream photoStream, string name, ILogger log)
    {
        log.LogInformation("ReadNumberPlateFunction");

        string endpoint = Environment.GetEnvironmentVariable("VisionEndpoint");
        string key = Environment.GetEnvironmentVariable("VisionKey");
            
        ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endpoint
        };

        string operationLocation;
            
        try
        {
            var textHeaders = await client.ReadInStreamAsync(photoStream);
            operationLocation = textHeaders.OperationLocation;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error reading stream to vision");
            throw;
        }

        await Task.Delay(2000);

        const int numberOfCharsInOperationId = 36;
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        ReadOperationResult results;

        log.LogInformation("Extracting text from photo");

        do
        {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        } while (results.Status == OperationStatusCodes.Running ||
                 results.Status == OperationStatusCodes.NotStarted);

        var textUrlFileResults = results.AnalyzeResult.ReadResults;
        foreach (ReadResult page in textUrlFileResults)
        {
            foreach (Line line in page.Lines)
            {
                var match = Regex.Match(line.Text, NumberPlateRegEx);
                if (match.Success)
                {
                    log.LogInformation("Text found for {Name}: {Text}", name, line.Text);

                    var message = new SaveSpeederDetailsMessage(name, line.Text);
                    return message;
                }
            }
        }

        return null;
    }
}