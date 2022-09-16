using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SpeedCameraProcessor.Functions;

public static class ImageUploadFunction
{
    [FunctionName("ImageUpload")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        try
        {
            var formdata = await req.ReadFormAsync();
            var file = req.Form.Files["photo"];

            log.LogInformation("Photo {FileName} uploaded started", file.FileName);

            var extensions = Path.GetExtension(file.FileName);
            var newFileName = $"{Guid.NewGuid()}{extensions}";

            await CreateBlob(newFileName, file.OpenReadStream());

            log.LogInformation("Photo {FileName} uploaded finished", file.FileName);

            return new OkObjectResult(file.FileName + " - " + file.Length);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex);
        }
    }

    private static async Task CreateBlob(string name, Stream data)
    {
        string containerName = Environment.GetEnvironmentVariable("SpeedersContainerName");
        string connectionString = Environment.GetEnvironmentVariable("SpeedCameraStore");
        BlobClient bloblClient = new BlobClient(connectionString, containerName, name);

        await bloblClient.UploadAsync(data);
    }
}
