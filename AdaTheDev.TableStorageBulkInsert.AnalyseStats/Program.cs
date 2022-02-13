using Azure.Data.Tables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdaTheDev.TableStorageBulkInsert.AnalyseStats
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var azureStorageConnectionString = "";

            var tests = Enumerable.Range(1, 10);

            var table = new TableClient(
                azureStorageConnectionString,
                "StatsTable");

            foreach(var testNumber in tests)
            {
                DateTime minStartTime = DateTime.MaxValue;
                DateTime maxEndTime = DateTime.MinValue;
                int numberOfTasks = 0;
                int totalRecordsProcessed = 0;

                await foreach (var stat in table.QueryAsync<StatsEntity>(x => x.PartitionKey == $"TESTCASE-{testNumber}"))
                {
                    minStartTime = stat.StartTime < minStartTime ? stat.StartTime : minStartTime;
                    maxEndTime = stat.EndTime > maxEndTime ? stat.EndTime : maxEndTime;
                    numberOfTasks++;
                    totalRecordsProcessed += stat.RecordsProcessed;
                }

                var timeTakenSeconds = maxEndTime.Subtract(minStartTime).TotalSeconds;
                var recordsPerSecond = totalRecordsProcessed / timeTakenSeconds;

                Console.WriteLine($"TEST CASE {testNumber}:");
                Console.WriteLine($"    Time (s): {timeTakenSeconds:F1}");
                Console.WriteLine($"    Records/s: {recordsPerSecond:F0}");
            }
        }
    }
}
