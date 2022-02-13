namespace AdaTheDev.TableStorageBulkInsert
{
    public class BulkInsertMessage
    {
        public int TestNumber { get; set; }

        public string BlobName { get; set; }

        public int TaskParallelism { get; set; }

        public int ScaleOutMaxInstances { get; set; }        
    }
}
