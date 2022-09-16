using System;
using Newtonsoft.Json;

namespace SpeedCameraProcessor;

public class SpeederDocument
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; }
    public string NumberPlate { get; set; }
    public bool Processed { get; set; }
}