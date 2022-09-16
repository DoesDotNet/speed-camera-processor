namespace SpeedCameraProcessor.Models;

public class CropNumberPlateMessage
{
    public CropNumberPlateMessage(string fileName, double normalizedTop, double normalizedLeft, double normalizedWidth,
        double normalizedHeight)
    {
        FileName = fileName;
        NormalizedTop = normalizedTop;
        NormalizedLeft = normalizedLeft;
        NormalizedWidth = normalizedWidth;
        NormalizedHeight = normalizedHeight;
    }

    public string FileName { get; private set; }
    public double NormalizedTop { get; private set; }
    public double NormalizedLeft { get; private set; }
    public double NormalizedWidth { get; private set; }
    public double NormalizedHeight { get; private set; }
}