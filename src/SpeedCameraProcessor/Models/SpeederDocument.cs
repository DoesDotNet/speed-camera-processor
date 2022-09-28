using System;
using Newtonsoft.Json;

namespace SpeedCameraProcessor.Models;

public class SpeederDocument
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; }
    public string PhotoFileName { get; set; }
    public string CropFileName { get; set; }
    public string NumberPlate { get; set; }
    public bool Processed { get; set; }
    public bool Processing { get; set; }
    public bool Failed { get; set; }
}