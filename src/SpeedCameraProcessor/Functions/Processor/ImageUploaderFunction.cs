using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions.Processor;

public static class ImageUploaderFunction
{
    [FunctionName("ImageUploader")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [CosmosDB(
            Constants.CosmosDBName,
            Constants.CosmosCollectionName,
            ConnectionStringSetting = Constants.CosmosConnectionString)] IAsyncCollector<SpeederDocument> speederDocuments,
        ILogger log)
    {
        try
        {
            var formdata = await req.ReadFormAsync();

            if(req.Form.Files.Count < 1)
                return new BadRequestResult();

            var file = req.Form.Files["photo"];

            log.LogInformation("Photo {FileName} uploaded started", file.FileName);

            var extensions = Path.GetExtension(file.FileName);
            var id = Guid.NewGuid();
            var newFileName = $"{id}{extensions}";

            await speederDocuments.AddAsync(new SpeederDocument
            {
                Id = id,
                OriginalFileName = file.FileName,
                Processing = true
            });

            string containerName = Environment.GetEnvironmentVariable("SpeedCameraInbox");
            string connectionString = Environment.GetEnvironmentVariable("SpeedCameraStore");

            BlobClient bloblClient = new BlobClient(connectionString, containerName, newFileName);

            await bloblClient.UploadAsync(file.OpenReadStream());

            log.LogInformation("Photo {FileName} uploaded finished", file.FileName);

            return new OkObjectResult(file.FileName + " - " + file.Length);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex);
        }
    }
}