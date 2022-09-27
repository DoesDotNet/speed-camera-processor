namespace SpeedCameraProcessor.Models;

public class NumberPlateMessage
{
    public string Id { get; set; }
    public string NumberPlate { get; set; }
    public bool MatchingFailed { get; set; }
}