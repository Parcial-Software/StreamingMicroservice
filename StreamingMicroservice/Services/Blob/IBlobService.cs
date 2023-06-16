namespace StreamingMicroservice.Services.Blob
{
    public interface IBlobService
    {
        public Task<string> SaveToBlob(string path, string fileName, Stream file);
    }
}
