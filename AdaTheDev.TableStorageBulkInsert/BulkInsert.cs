using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using Azure.Data.Tables;
using System.Linq;
using System;
using Azure.Storage.Blobs;
using System.IO;
using System.Globalization;

namespace AdaTheDev.TableStorageBulkInsert
{
    public class BulkInsert
    {
        private readonly Settings _settings;

        public BulkInsert(Settings settings)
        {
            _settings = settings;
        }

        [FunctionName("BulkInsert")]
        public async Task Run([QueueTrigger("bulkinsert-queue", Connection = "AzureWebJobsStorage")] BulkInsertMessage message)
        {            
            // Read the whole file, into a list of lists. Each list contains message.TaskParallelism entities for inserting in parallel.
            var allEntitiesBatched = await GetBatchedEntitiesAsync(message.BlobName, message.TaskParallelism);
            
            // Create destination table
            var tableName = $"TestTable{message.TestNumber}";
            var tableClient = new TableClient(_settings.AzureStorageConnectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            var recordsProcessed = 0;

            // Load the data
            var startTime = DateTime.UtcNow;
            foreach (var batch in allEntitiesBatched)
            {
                var tasks = batch.Select(x => tableClient.UpsertEntityAsync(x));
                await Task.WhenAll(tasks);
                recordsProcessed += batch.Count;
            }
            var endTime = DateTime.UtcNow; 

            // Log stats out
            var statsTable = new TableClient(_settings.AzureStorageConnectionString, "StatsTable");
            await statsTable.CreateIfNotExistsAsync();
            
            var statsEntity = new StatsEntity
            {
                PartitionKey = $"TESTCASE-{message.TestNumber}",
                RowKey = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff"),
                BlobName = message.BlobName,
                StartTime = startTime,
                EndTime = endTime,
                ScaleOutMaxInstances = message.ScaleOutMaxInstances,
                TaskParallelism = message.TaskParallelism,
                RecordsProcessed = recordsProcessed,
                DurationSeconds = endTime.Subtract(startTime).TotalSeconds,
                RecordsPerSecond = recordsProcessed / endTime.Subtract(startTime).TotalSeconds                
            };

            await statsTable.AddEntityAsync(statsEntity);
        }

        private async Task<List<List<TestEntity>>> GetBatchedEntitiesAsync(string blobName, int batchSize)
        {
            var data = new List<List<TestEntity>>();

            var blobClient = new BlobClient(_settings.AzureStorageConnectionString, "data", blobName);
            using var stream = new MemoryStream();
            var contentResponse = await blobClient.DownloadToAsync(stream);
            stream.Position = 0;
            using var streamReader = new StreamReader(stream);

            using var csvReader = new CsvHelper.CsvReader(streamReader, CultureInfo.InvariantCulture);

            var batch = new List<TestEntity>();
            var recordCount = 0;

            while (csvReader.Read())
            {
                recordCount++;
                           
                batch.Add(new TestEntity
                {
                    PartitionKey = csvReader[0],
                    RowKey = "DATA",
                    SomeValue = csvReader[0],
                    SomeNumber = int.Parse(csvReader[1])
                });

                if (batch.Count == batchSize)
                {
                    data.Add(new List<TestEntity>(batch));                    
                    batch.Clear();                    
                }
            }

            return data;
        }
    }
}
