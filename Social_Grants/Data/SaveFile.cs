using Social_Grants.Models;

namespace Social_Grants.Data
{
    public class SaveFile
    {
        public static async Task SaveDocument(AppDbContext AppDb, List<string> fileNames, string userId)
        {
            foreach (var name in fileNames)
            {
                await AppDb.TblUserDocuments.AddAsync(new UserDocuments
                {
                    Name = name,
                    AppUserId = userId
                });
            }
        }
    }
}
