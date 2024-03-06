namespace Social_Grants.Services.Abstract
{
    public interface IDocumentService
    {
        string UploadDocumentToAzure(IFormFile file);
        void DeleteDocumentFromAzure(string fileName);
    }
}
