using System;
using Azure.Data.Tables;
using Azure;

namespace AdaTheDev.TableStorageBulkInsert
{
    public class TestEntity: ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public int SomeNumber { get; set; }

        public string SomeValue { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}
