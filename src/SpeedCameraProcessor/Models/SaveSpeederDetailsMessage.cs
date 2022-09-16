namespace SpeedCameraProcessor.Models;

public class SaveSpeederDetailsMessage
{
    public SaveSpeederDetailsMessage(string filName, string registration)
    {
        FileName = filName;
        Registration = registration;
    }
    
    public string FileName { get; private set; }
    public string Registration { get; private set; }
}