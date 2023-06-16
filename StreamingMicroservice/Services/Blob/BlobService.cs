using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace StreamingMicroservice.Services.Blob
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;

        public BlobService(IOptions<BlobSettings> options)
        {
            var settings = options.Value;
            _container = new(settings.ConnectionString, settings.ContainerName);
        }
        public async Task<string> SaveToBlob(string path, string fileName, Stream file)
        {
            var blob = _container.GetBlobClient($"{path}/{fileName}");
            await blob.UploadAsync(file, true);
            return blob.Uri.AbsoluteUri;
        }
    }
}
