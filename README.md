# Speed Camera Processor

Application built using Azure Funcitons to extract number plate information from an image.


## Running Locally


To run locally you will need to create a `local.settings.json` file as outlined below.
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureSignalRConnectionString": "<conn string frp, Azure>",
    "AzureWebJobsStorage": "usedevelopmentstorage=true",
    "SpeedCameraStore": "usedevelopmentstorage=true",
    "SpeedCameraCosmos": "<cosmos db or emulator connection string>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "CustomVisionEndpoint": "<endpoint from azure>",
    "CustomVisionKey": "<custom vision key>",
    "CustomVisionTagName": "numberplate",
    "CustomVisionModelName": "<iteration name>",
    "CustomVisionProjectId": "<custom vision project id>",
    "VisionEndpoint": "<computer vision endpoint>",
    "VisionKey": "<computer visino key>"
  }
}
```

## Architecure

![Spped Camera Processor architecture](https://github.com/DoesDotNet/speed-camera-processor/blob/4d4f0d570125f8876f6cb86a8d27c253e10958a6/docs/SpeedCameraProcessor.png?raw=true)


## Services Used

- [Azure Functions](https://azure.microsoft.com/en-gb/products/functions/)
- [Computer Vision](https://azure.microsoft.com/en-gb/products/cognitive-services/computer-vision)
- [Custom Vision](https://azure.microsoft.com/en-us/products/cognitive-services/custom-vision-service)
- [CosmosDB](https://azure.microsoft.com/en-gb/products/cosmos-db)
- [SignalR Service](https://azure.microsoft.com/en-gb/products/signalr-service/)
- [Azure Storage](https://azure.microsoft.com/en-gb/products/storage)

## Local Emulators

- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite)
- [CosmosDB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator)


## Cusomt Vision Model Training

To aid in training there is an `images` folder with enough examples for you to train your own model.