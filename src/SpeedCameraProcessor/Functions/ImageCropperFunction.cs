using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SpeedCameraProcessor.Models;

namespace SpeedCameraProcessor.Functions;

public static class ImageCropperFunction
{
    [FunctionName("ImageCropper")]
    public static async Task Run(
        [QueueTrigger("crops", Connection = "SpeedCameraStore")] CropNumberPlateMessage cropNumberPlateMessage,
        CancellationToken cancellationToken,
        ILogger log)
    {
        log.LogInformation("Cropping image {FileName}", cropNumberPlateMessage.FileName);

        string storageConnectionString = Environment.GetEnvironmentVariable("SpeedCameraStore");
        string containerName = Environment.GetEnvironmentVariable("SpeedersContainerName");

        BlobContainerClient blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
        BlobClient originalImageBlobClient = blobContainerClient.GetBlobClient(cropNumberPlateMessage.FileName);
        var blob = await originalImageBlobClient.DownloadContentAsync(cancellationToken);
        var blobContent = blob.Value;

        using (Image image = await Image.LoadAsync(blobContent.Content.ToStream()))
        {
            var top = (int) (cropNumberPlateMessage.NormalizedTop * image.Height);
            var height = (int) (cropNumberPlateMessage.NormalizedHeight * image.Height);
            var left = (int) (cropNumberPlateMessage.NormalizedLeft * image.Width);
            var width = (int) (cropNumberPlateMessage.NormalizedWidth * image.Width);

            image.Mutate(x => x.Crop(new Rectangle(left, top, width, height)));

            using (var memoryStream = new MemoryStream())
            {
                await image.SaveAsync(memoryStream, new PngEncoder(), cancellationToken);
                string croppedImageFileName = $"plates/{Path.GetFileNameWithoutExtension(cropNumberPlateMessage.FileName)}-plate.png";
                BlobClient croppedImageBlobClient = blobContainerClient.GetBlobClient(croppedImageFileName);

                memoryStream.Position = 0;
                await croppedImageBlobClient.UploadAsync(memoryStream, cancellationToken);
            }

        }
    }
}
