using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Specialized;

namespace SpeedCameraProcessor.Functions.Web
{
    public static class ImagesFunction
    {
        [FunctionName("Images")]
        public static async Task<IActionResult> Images(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Images/{fileName}")] HttpRequest req,
            [Blob("speeders/{fileName}", FileAccess.Read, Connection = Constants.StorageConnection)] BlockBlobClient speederImageBlob,
            ILogger log)
        {
            log.LogInformation($"ImageReader fetching {speederImageBlob.Name}");

            if (!await speederImageBlob.ExistsAsync())
            {
                return new NotFoundResult();
            }

            var stream = await speederImageBlob.OpenReadAsync();
            var properties = speederImageBlob.GetProperties().Value;

            return new FileStreamResult(stream, properties.ContentType);
        }

        [FunctionName("Plates")]
        public static async Task<IActionResult> Plates(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Images/Plates/{fileName}")] HttpRequest req,
            [Blob("speeders/plates/{fileName}", FileAccess.Read, Connection = Constants.StorageConnection)] BlockBlobClient speederImageBlob,
            ILogger log)
        {
            log.LogInformation($"ImageReader fetching {speederImageBlob.Name}");

            if (!await speederImageBlob.ExistsAsync())
            {
                return new NotFoundResult();
            }

            var stream = await speederImageBlob.OpenReadAsync();
            var properties = speederImageBlob.GetProperties().Value;

            return new FileStreamResult(stream, properties.ContentType);
        }
    }
}
