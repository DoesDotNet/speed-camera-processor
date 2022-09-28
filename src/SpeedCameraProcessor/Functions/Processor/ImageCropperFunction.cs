using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions.Processor;

public static class ImageCropperFunction
{
    [FunctionName("ImageCropper")]
    public static async Task Run(
        [QueueTrigger("crop-plate", Connection = "SpeedCameraStore")] CropNumberPlateMessage cropNumberPlateMessage,
        [Blob("speeders/{FileName}", FileAccess.Read, Connection = Constants.StorageConnection)] Stream speederImage, // input
        [Blob("speeders/plates/{FileName}", FileAccess.Write, Connection = Constants.StorageConnection)] BlockBlobClient speederPlateImageClient, // output
        CancellationToken cancellationToken,
        ILogger log)
    {
        log.LogInformation("Cropping image {FileName}", cropNumberPlateMessage.FileName);

        // crop image
        IImageFormat format;
        using (var image = Image.Load(speederImage, out format))
        {
            var top = (int)(cropNumberPlateMessage.NormalizedTop * image.Height);
            var height = (int)(cropNumberPlateMessage.NormalizedHeight * image.Height);
            var left = (int)(cropNumberPlateMessage.NormalizedLeft * image.Width);
            var width = (int)(cropNumberPlateMessage.NormalizedWidth * image.Width);

            // Computer vision needs at least 50x50 or the endpoint will throw bad request
            if (height < 50)
                height = 50;

            if (width < 50)
                width = 50;

            // crop
            image.Mutate(x => x.Crop(new Rectangle(left, top, width, height)));

            // save cropped image to output binding
            using var stream = new MemoryStream();
            await image.SaveAsync(stream, format, cancellationToken);
            stream.Position = 0;

            await speederPlateImageClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = format.DefaultMimeType });
        }
    }
}
