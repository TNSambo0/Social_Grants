namespace Social_Grants.Services.Abstract
{
    public interface IImageService
    {
        string UploadImageToAzure(IFormFile file);
        void DeleteImageFromAzure(string fileName);
    }
} 
