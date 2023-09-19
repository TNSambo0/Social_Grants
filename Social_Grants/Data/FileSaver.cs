using Social_Grants.Models.Account;
using Social_Grants.Models;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO;

namespace Social_Grants.Data
{
    public class FileSaver
    {
        public static void Savefiles(IWebHostEnvironment webHostEnvironment, List<Files> files, AppUser user, string fileOwnerIdNumber)
        {
            var webRootPath = webHostEnvironment.WebRootPath;
            string path = fileOwnerIdNumber == user.IDNumber ? Path.Combine(webRootPath, $"Documents\\User\\{user.IDNumber}") :
               Path.Combine(webRootPath, $"Documents\\User\\{user.IDNumber}\\{fileOwnerIdNumber}");
            foreach (var obj in files)
            {
                string filename = obj.file.FileName;
                var fileExt = Path.GetExtension(filename);
                string myfile = "";
                if (fileOwnerIdNumber == user.IDNumber)
                {
                    myfile = $"{user.IDNumber}_{obj.name}{fileExt}";
                    string fileNameWithPath = Path.Combine(path, myfile);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (File.Exists(fileNameWithPath))
                    {
                        File.Delete(fileNameWithPath);
                    }
                    using var stream = new FileStream(path, FileMode.Create);
                    obj.file.CopyTo(stream);
                }
                else
                {
                    myfile = $"{fileOwnerIdNumber}_{obj.name}{fileExt}";
                    string fileNameWithPath = Path.Combine(path, myfile);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (File.Exists(fileNameWithPath))
                    {
                        File.Delete(fileNameWithPath);
                    }
                    using var stream = new FileStream(path, FileMode.Create);
                    obj.file.CopyTo(stream);
                }
            }
        }
    }
}
