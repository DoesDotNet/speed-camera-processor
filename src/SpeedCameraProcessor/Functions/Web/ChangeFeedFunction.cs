using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace SpeedCameraProcessor.Functions.Web
{
    public static class ChangeFeedFunction
    {
        [FunctionName("ChangeFeed")]
        public static async Task ChangeFeed(
            [CosmosDBTrigger(
                databaseName: Constants.CosmosDBName,
                collectionName: Constants.CosmosCollectionName,
                ConnectionStringSetting = Constants.CosmosConnectionString,
                LeaseCollectionName = Constants.CosmosLeasesCollectionName,
                FeedPollDelay = 500,
                CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            [SignalR(HubName = "serverless")] IAsyncCollector<SignalRMessage> signalRMessages)
        {

            foreach (var doc in input)
            {
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "processing",
                        Arguments = new[] { doc }
                    }
                );
            }
        }
    }
}