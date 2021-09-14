using Newtonsoft.Json;
using System;

namespace speed_camera_processor
{
    public class SpeederDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string NumberPlate { get; set; }
        public bool Processed { get; set; }
    }
}
