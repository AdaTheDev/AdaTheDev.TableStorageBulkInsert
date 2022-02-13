using System;
using System.IO;

namespace AdaTheDev.TableStorageBulkInsert.DataGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stream = File.OpenWrite(@"C:\testdatafiles\Chunk-1.csv");
            var writer = new StreamWriter(stream);
            var recordsPerFile = 25000;
            var totalRecordsToCreate = 10000000;

            var thisFileCount = 0;
            var thisFileNumber = 1;

            for (var i = 1; i <= totalRecordsToCreate; i++)
            {
                thisFileCount++;
                writer.WriteLine($"{Guid.NewGuid()},{i}");

                if (thisFileCount == recordsPerFile)
                {
                    writer.Close();
                    writer.Dispose();
                    stream.Dispose();

                    if (i < totalRecordsToCreate)
                    {
                        thisFileNumber++;
                        thisFileCount = 0;
                        stream = File.OpenWrite($"C:\\testdatafiles\\Chunk-{thisFileNumber}.csv");
                        writer = new StreamWriter(stream);
                    }
                }
            }
        }
    }
}
