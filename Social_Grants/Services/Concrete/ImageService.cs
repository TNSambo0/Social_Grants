using Microsoft.Extensions.Options;
using Social_Grants.Options;
using Social_Grants.Services.Abstract;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Social_Grants.Services.Concrete
{
    public class ImageService : IImageService
    {
        private readonly AzureOptions _azureOptions;
        public ImageService(IOptions<AzureOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }
        public string UploadImageToAzure(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            using MemoryStream fileUploadsStream = new();
            file.CopyTo(fileUploadsStream);
            fileUploadsStream.Position = 0;
            BlobContainerClient blobContainerClient = new(_azureOptions.ConnectionString, _azureOptions.Container);
            var uniqueName = Guid.NewGuid().ToString() + fileExtension;
            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueName);
            blobClient.Upload(fileUploadsStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/bitmap"
                }
            }, cancellationToken: default);
            return uniqueName;
        }
        public void DeleteImageFromAzure(string fileName)
        {
            BlobContainerClient blobContainerClient = new(_azureOptions.ConnectionString, _azureOptions.Container);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            blobClient.Delete();
        }
    }
}
