using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions.Processor;

public static class ImageUploaderFunction
{
    [FunctionName("ImageUploader")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [CosmosDB("%DatabaseName%", "%CollectionName%", ConnectionStringSetting = "CosmosConnectionString")] IAsyncCollector<SpeederDocument> speederDocuments,
        IBinder binder,
        ILogger log)
    {
        try
        {
            var formdata = await req.ReadFormAsync();

            if (req.Form.Files.Count < 1)
                return new BadRequestResult();

            var file = req.Form.Files["photo"];
            var extension = Path.GetExtension(file.FileName);

            log.LogInformation("Photo {FileName} uploaded started", file.FileName);
                        
            // resize image or Custom Vision will have a shit fit (4MB max)
            IImageFormat format;
            using var memoryStream =  new MemoryStream();
            using (Image image = Image.Load(file.OpenReadStream(), out format))
            {
                image.Mutate(x => x.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(2000, 2000) }));

                // I wanted to support webp, but Custom Vision doesn't
                // https://learn.microsoft.com/en-us/azure/cognitive-services/computer-vision/overview#image-requirements
                if (format is WebpFormat)
                {
                    extension = ".jpg";
                    await image.SaveAsync(memoryStream, JpegFormat.Instance);
                }
                else
                {
                    await image.SaveAsync(memoryStream, format);
                }
            }

           
            var id = Guid.NewGuid();
            var newFileName = $"{id}{extension}";


            // instead of using output blob binding as attribute, I'm setting it
            // in code so we can have control of the name and content type
            var outboundBlob = new BlobAttribute($"speeders/{newFileName}", FileAccess.Write);
            var blockBlobClient = binder.Bind<BlockBlobClient>(outboundBlob);

            memoryStream.Position = 0;
            await blockBlobClient.UploadAsync(memoryStream, new BlobHttpHeaders {  ContentType = file.ContentType});

            // write to cosmos
            await speederDocuments.AddAsync(new SpeederDocument
            {
                Id = id,
                OriginalFileName = file.FileName,
                PhotoFileName = newFileName,
                CropFileName = newFileName,
                Processing = true
            });

            log.LogInformation("Photo {FileName} uploaded finished", file.FileName);

            return new OkObjectResult(file.FileName + " - " + file.Length);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex);
        }
    }
}