using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using MimeTypes;

namespace SpeedCameraProcessor.Functions.Web
{
    public static class IndexFunction
    {
        [FunctionName("Index")]
        public static IActionResult GetHomePage([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "web", "index.html");
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [FunctionName("StaticFiles")]
        public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ExecutionContext context)
        {
            try
            {
                var filePath = GetFilePath(Path.Combine(context.FunctionAppDirectory, "web"), req.Query["file"]);
                if (File.Exists(filePath))
                {
                    var stream = File.OpenRead(filePath);
                    return new FileStreamResult(stream, GetMimeType(filePath))
                    {
                        LastModified = File.GetLastWriteTime(filePath)
                    };
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        private static string GetFilePath(string contentRoot, string pathValue)
        {
            string fullPath = Path.GetFullPath(Path.Combine(contentRoot, pathValue));
            if (!IsInDirectory(contentRoot, fullPath))
            {
                throw new ArgumentException("Invalid path");
            }

            if (Directory.Exists(fullPath))
            {
                fullPath = Path.Combine(fullPath, "index.html");
            }
            return fullPath;
        }

        private static bool IsInDirectory(string parentPath, string childPath) => childPath.StartsWith(parentPath);

        private static string GetMimeType(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return MimeTypeMap.GetMimeType(fileInfo.Extension);
        }
    }
}