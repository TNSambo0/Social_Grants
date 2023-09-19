using Microsoft.EntityFrameworkCore;
using Social_Grants.Models.Account;
using Social_Grants.Models;

namespace Social_Grants.Data
{
    public class AddDependent
    {
        public async Task CreateDependent(AppDbContext _AppDb, Dependent aDependent, AppUser user)
        {
            var dependentFromDb = await _AppDb.TblDependent.FirstOrDefaultAsync(x => x.IDNumber == aDependent.IDNumber);
            if (dependentFromDb == null)
            {
                var fileExt = Path.GetExtension(aDependent.IdentityDocument.FileName);
                var fileName = aDependent.IDNumber + fileExt;
                var fileUrl = $"Documents/User/{user.IDNumber}/{aDependent.IDNumber}/{fileName}"; 
                Dependent dependent = new()
                {
                    FullName = aDependent.FullName,
                    LastName = aDependent.LastName,
                    IDNumber = aDependent.IDNumber,
                    Nationality = aDependent.Nationality,
                    AppUserId = user.Id,
                    IdentityDocumentUrl = fileUrl
                };
                await _AppDb.TblDependent.AddAsync(dependent); 
                await _AppDb.SaveChangesAsync();
            }
            else
            {
                aDependent.Id = dependentFromDb.Id;
                _AppDb.TblDependent.Update(aDependent);
                await _AppDb.SaveChangesAsync();
            }
        }
    }
}
