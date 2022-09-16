

namespace SpeedCameraProcessor.OldFunctions;

public static class ImageUploadFunction
{
    // [FunctionName("ImageUpload")]
    // public static async Task<IActionResult> Run(
    //     [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
    //     [Blob("incoming/{rand-guid}.png", FileAccess.Write, Connection = "SpeederStorage")] CloudBlockBlob incomingImage,
    //     ILogger log)
    // {
    //     log.LogInformation("Image Uploading");
    //
    //     var file = req.Form.Files["speeder"];
    //
    //     using (var image = Image.Load(file.OpenReadStream()))
    //     {
    //         using(var ms = new MemoryStream())
    //         {
    //             await image.SaveAsPngAsync(ms, new PngEncoder());
    //             ms.Position = 0;
    //             await incomingImage.UploadFromStreamAsync(ms);
    //         }
    //     }
    //
    //     return new OkResult();
    // }
}
