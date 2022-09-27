namespace SpeedCameraProcessor
{
    internal static class Constants
    {
        public const string CosmosDBName = "Police";
        public const string CosmosCollectionName = "Speeders";
        public const string CosmosLeasesCollectionName = "SpeederLeases";
        public const string CosmosConnectionString = "SpeedCameraCosmos";

        public const string StorageConnection = "SpeedCameraStore";
        public const string Inbox = "SpeedCameraInbox";

        public const string NumberPlateQueue = "number-plates";
        public const string CropQueue = "crop-plate";
    }
}
