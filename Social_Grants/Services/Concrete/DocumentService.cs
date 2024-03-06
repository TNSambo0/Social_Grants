using Social_Grants.Services.Abstract;
using Social_Grants.Options;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Social_Grants.Services.Concrete
{
    public class DocumentService : IDocumentService
    {
        private readonly AzureOptions _azureOptions;
        public DocumentService(IOptions<AzureOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }
        public string UploadDocumentToAzure(IFormFile file)
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
                    ContentType = "application/pdf"
                }
            }, cancellationToken: default);
            return uniqueName;
        }
        public void DeleteDocumentFromAzure(string fileName)
        {
            BlobContainerClient blobContainerClient = new(_azureOptions.ConnectionString, _azureOptions.Container);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            blobClient.Delete();
        }
    }
}
