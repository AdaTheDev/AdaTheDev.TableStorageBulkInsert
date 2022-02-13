using System;
using Azure.Data.Tables;
using Azure;

namespace AdaTheDev.TableStorageBulkInsert
{
    public class StatsEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public ETag ETag { get; set; }

        public string BlobName { get; set; }

        public int TaskParallelism { get; set; }

        public int ScaleOutMaxInstances { get; set; }

        public double DurationSeconds { get; set; }

        public int RecordsProcessed { get; set; }

        public double RecordsPerSecond { get; set; }        
    }
}
