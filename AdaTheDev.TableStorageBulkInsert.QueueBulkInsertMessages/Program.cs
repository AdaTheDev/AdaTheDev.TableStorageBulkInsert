using AdaTheDev.ParallelTableStorageDataGen;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdaTheDev.TableStorageBulkInsert.QueueBulkInsertMessages
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Set these!
            var azureStorageConnectionString = "";
            var testNumberToRun = 1;


            var queue = new Azure.Storage.Queues.QueueClient(azureStorageConnectionString,
                "bulkinsert-queue", new Azure.Storage.Queues.QueueClientOptions { MessageEncoding = Azure.Storage.Queues.QueueMessageEncoding.Base64 });

            await queue.CreateIfNotExistsAsync();

            var tests = new Dictionary<int, (int ScaleOutMaxInstances, int TaskParallelism, int NumberOfFiles)>
            {
                {1, (1, 1, 4) }, 
                {2, (1, 10, 4) },
                {3, (1, 15, 4) },
                {4, (1, 20, 4) },
                {5, (10, 15, 40) }, 
                {6, (40, 15, 40) },
                {7, (40, 15, 400) },
                {8, (40, 15, 400) },
                {9, (35, 15, 400) },
                {10, (35, 15, 400) }
            };

            var test = tests[testNumberToRun];

            for (var i = 1; i <= test.NumberOfFiles; i++)
            {
                var message = new BulkInsertMessage
                {
                    TestNumber = testNumberToRun,
                    ScaleOutMaxInstances = test.ScaleOutMaxInstances,
                    BlobName = $"Chunk-{i}.csv",
                    TaskParallelism = test.TaskParallelism,
                };

                await queue.SendMessageAsync(JsonConvert.SerializeObject(message));
            }            
        }
    }
}
