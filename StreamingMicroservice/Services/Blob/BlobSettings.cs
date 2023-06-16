namespace StreamingMicroservice.Services.Blob
{
    public class BlobSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string ContainerName { get; set; } = null!;
    }
}
