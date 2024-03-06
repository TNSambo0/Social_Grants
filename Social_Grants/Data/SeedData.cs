using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Social_Grants.Models;
using Social_Grants.Models.Account;
using Social_Grants.Models.Grant;

namespace Social_Grants.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(
             serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());
            var adminID = await EnsureUser(serviceProvider, "@littl3Sourc3", "Admin@outlook.com");
            await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);
            var CustomerID = await EnsureUser(serviceProvider, "@littl3Sourc3", "Customer@outlook.com");
            await EnsureRole(serviceProvider, CustomerID, Constants.CustomersRole);
            var CustomerID1 = await EnsureUser(serviceProvider, "@littl3Sourc3", "Customer1@outlook.com");
            await EnsureRole(serviceProvider, CustomerID1, Constants.CustomersRole);
            await SeedDataOnDb(context);
        }
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<AppUser>>() ?? throw new Exception("The userManager returned a null.");
            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new AppUser
                {
                    FirstName = "Tsepo Noah",
                    LastName = "Sambo",
                    UserName = UserName,
                    EmailConfirmed = true,
                    Email = UserName,
                    ImageUrl = "images//User/images.png",
                    IDNumber = "9907220600081"
                };
                await userManager.CreateAsync(user, testUserPw);
            }
            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }
            return user.Id;
        }
        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string userId, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>() ?? throw new Exception("roleManager null");
            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            var userManager = serviceProvider.GetService<UserManager<AppUser>>() ?? throw new Exception("The userManager returned a null.");
            var user = await userManager.FindByIdAsync(userId) ?? throw new Exception("The testUserPw password was probably not strong enough!");
            IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }
        public static async Task SeedDataOnDb(AppDbContext context)
        {
            if (await context.TblGender.AnyAsync()) { return; }
            else
            {
                await context.TblGender.AddRangeAsync(new Gender
                {
                    GenderName = "Select gender"
                }, new Gender
                {
                    GenderName = "Female"
                }, new Gender
                {
                    GenderName = "Male"
                }, new Gender
                {
                    GenderName = "Prefer not to say"
                }); await context.SaveChangesAsync();
            }
            if (await context.TblApplicationForWho.AnyAsync()) { return; }
            else
            {
                await context.TblApplicationForWho.AddRangeAsync(new ApplyForWho
                {
                    Answer = "Myself"
                }, new ApplyForWho
                {
                    Answer = "Dependent"
                }); await context.SaveChangesAsync();
            }
            if (await context.TblProvinces.AnyAsync()) { return; }
            else
            {
                await context.TblProvinces.AddRangeAsync(new Province
                {
                    ProvinceName = "Gauteng"
                }, new Province
                {
                    ProvinceName = "limpopo"
                }, new Province
                {
                    ProvinceName = "North west"
                }, new Province
                {
                    ProvinceName = "Free state"
                }, new Province
                {
                    ProvinceName = "Northern Cape"
                }, new Province
                {
                    ProvinceName = "Western Cape"
                }, new Province
                {
                    ProvinceName = "Mpumalanga"
                }, new Province
                {
                    ProvinceName = "Eastern Cape"
                }, new Province
                {
                    ProvinceName = "Kwazulu Natal"
                }); await context.SaveChangesAsync();
            }
            if (await context.TblCities.AnyAsync()) { return; }
            else
            {
                await context.TblCities.AddRangeAsync(new City
                {
                    CityName = "Vhembe",
                    ProvinceId = 2
                },
                new City
                {
                    CityName = "Capricorn",
                    ProvinceId = 2
                },
                new City
                {
                    CityName = "Waterberg",
                    ProvinceId = 2
                },
                new City
                {
                    CityName = "Ehlanzeni",
                    ProvinceId = 2
                },
                new City
                {
                    CityName = "Nkangala",
                    ProvinceId = 2
                },
                new City
                {
                    CityName = "NITS Head Office",
                    ProvinceId = 2
                },
                new City
                {
                    CityName = "Sarah Baartman",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "NMBM",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "Joe Gqabi",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "Alfred Nzo",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "Buffalo City",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "Chris Hani",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "Amathole",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "OR Tambo",
                    ProvinceId = 8
                },
                new City
                {
                    CityName = "Eden Karoo",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Atlantic Coast",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Cape Winelands",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Overberg",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Peninsula East",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Peninsula North",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Peninsula South",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Western Region",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Regional Office-Cape Metro",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Regional Office-Western Region",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "SIS Western Region",
                    ProvinceId = 6
                },
                new City
                {
                    CityName = "Ehlanzeni",
                    ProvinceId = 7
                },
                new City
                {
                    CityName = "Gert Sibande",
                    ProvinceId = 7
                },
                new City
                {
                    CityName = "Nkangala",
                    ProvinceId = 7
                },
                new City
                {
                    CityName = "Waterberg",
                    ProvinceId = 7
                },
                new City
                {
                    CityName = "Capricorn",
                    ProvinceId = 7
                },
                new City
                {
                    CityName = "SIS North Region",
                    ProvinceId = 7
                },
                new City
                {
                    CityName = "Alberton",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Johannesburg North",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Vanderbijlpark",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Tshwane South",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Roodepoort",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Benoni",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Johannesburg Central",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Kempton Park",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Springs",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Tshwane East",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Johannesburg Southwest",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Germiston",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Tshwane North",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Tshwane Central",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Internal Audit-National",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Fw Rand/Krugersdorp",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "NITS Head Office",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "NITS Pretoria",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "NITS Johannesburg",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "NPC Head Office",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "SIS Forensic Investigations",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "SIS Gauteng Region",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Regional Office-Wits",
                    ProvinceId = 1
                },
                new City
                {
                    CityName = "Dr Kenneth Kaunda",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Dr Ruth Segomotsi Mompati",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Ngaka Modiri Molema",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Bojanala Platinum",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Waterberg",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Tshwane North",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Fw Rand/Krugersdorp",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Sekhukhune",
                    ProvinceId = 3
                },
                new City
                {
                    CityName = "Mnandi",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Highway",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Midlands",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Ukuhlamba",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "South Coast",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Battlefields",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "North Coast",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Ngome",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Umngeni",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Port Natal",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "NITS Durban",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "SIS KwaZulu Region",
                    ProvinceId = 9
                },
                new City
                {
                    CityName = "Lejweleputswa",
                    ProvinceId = 4
                },
                new City
                {
                    CityName = "Thabo Mofutsanyana",
                    ProvinceId = 4
                },
                new City
                {
                    CityName = "Mangaung Metro",
                    ProvinceId = 4
                },
                new City
                {
                    CityName = "Xhariep",
                    ProvinceId = 4
                },
                new City
                {
                    CityName = "Fezile Dabi",
                    ProvinceId = 4
                },
                new City
                {
                    CityName = "NITS Bloemfontein",
                    ProvinceId = 4
                },
                new City
                {
                    CityName = "SIS Central Region",
                    ProvinceId = 4
                },
                new City
                {
                    CityName = "Kalahari",
                    ProvinceId = 5
                },
                new City
                {
                    CityName = "Diamond Fields",
                    ProvinceId = 5
                },
                new City
                {
                    CityName = "Dr Ruth Segomotsi Mompati",
                    ProvinceId = 5
                },
                new City
                {
                    CityName = "John Taolo Gaetsewe",
                    ProvinceId = 5
                },
                new City
                {
                    CityName = "Xhariep",
                    ProvinceId = 5
                });
            }
            if (await context.TblGrants.AnyAsync())
            {
                return;
            }
            else
            {
                await context.TblGrants.AddRangeAsync(new Grants
                {
                    Name = "Care Dependancy grant",
                }, new Grants
                {
                    Name = "Child Support grant"
                }, new Grants
                {
                    Name = "Disability grant",
                }, new Grants
                {
                    Name = "Foster grant",
                }, new Grants
                {
                    Name = "Grant In Aid grant",
                }, new Grants
                {
                    Name = "Old Age grant",
                }, new Grants
                {
                    Name = "Social Relief of Distress grant",
                }, new Grants
                {
                    Name = "War Veteran grant",
                }); await context.SaveChangesAsync();
            }
            if (await context.TblGrantQuestions.AnyAsync()) { return; }
            else
            {
                await context.TblGrantQuestions.AddRangeAsync(new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 1
                },new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 2
                },new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 3
                },new GrantQuestions
                {
                    Question = "Recieve other grant",
                    GrantId = 3
                },new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 4
                },new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 5
                },new GrantQuestions
                {
                    Question = "Do you recieve other such as older person grant, disability grant or war veteran grant",
                    GrantId = 5
                },new GrantQuestions
                {
                    Question = "Do you require full-time attendance by another person",
                    GrantId = 5
                },new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 6
                },new GrantQuestions
                {
                    Question = "Recieve other grant",
                    GrantId = 6
                },new GrantQuestions
                {
                    Question = "Employment status",
                    GrantId = 7
                },new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 7
                },new GrantQuestions
                {
                    Question = "Recieve other grant",
                    GrantId = 7
                },new GrantQuestions
                {
                    Question = "Fought in the Second World War or Korean War",
                    GrantId = 8
                },new GrantQuestions
                {
                    Question = "Cared by State",
                    GrantId = 8
                },new GrantQuestions
                {
                    Question = "Recieve other grant",
                    GrantId = 8
                }); await context.SaveChangesAsync();
            }
            if (await context.TblGrantAnswers.AnyAsync()) { return; }
            else
            {
                await context.TblGrantAnswers.AddRangeAsync(new GrantAnswers
                {
                    QuestionId = 1,
                    Answer = "no",
                    GrantId = 1,
                    Reason = "Applicant should not be cared by State"
                },new GrantAnswers
                {
                    QuestionId = 2,
                    Answer = "no",
                    GrantId = 2,
                    Reason = "Applicant should not be cared by State"
                },new GrantAnswers
                {
                    QuestionId = 3,
                    Answer = "no",
                    GrantId = 2,
                    Reason = "Applicant should not be cared by State"
                },new GrantAnswers
                {
                    QuestionId = 4,
                    Answer = "no",
                    GrantId = 3,
                    Reason = "Applicant should not recieve another grant"
                },new GrantAnswers
                {
                    QuestionId =5,
                    Answer = "no",
                    GrantId = 4,
                    Reason = "Applicant cared by State"
                },new GrantAnswers
                {
                    QuestionId =6,
                    Answer = "no",
                    GrantId = 5,
                    Reason = "Applicant should not be cared by State"
                },new GrantAnswers
                {
                    QuestionId = 7,
                    Answer = "yes",
                    GrantId = 5,
                    Reason = "Applicant should recieve other such as older person grant, disability grant or war veteran grant"
                },new GrantAnswers
                {
                    QuestionId =8,
                    Answer = "yes",
                    GrantId = 5,
                    Reason = "Applicant should have a full-time helper"
                },new GrantAnswers
                {
                    QuestionId =9,
                    Answer = "no",
                    GrantId = 6,
                    Reason = "Applicant should not be cared by State"
                },new GrantAnswers
                {
                    QuestionId = 10,
                    Answer = "no",
                    GrantId = 6,
                    Reason = "Applicant should not recieve another grant"
                },new GrantAnswers
                {
                    QuestionId = 11,
                    Answer = "unemployed",
                    GrantId = 7,
                    Reason = "Applicant should be unemployed"
                },new GrantAnswers
                {
                    QuestionId =12,
                    Answer = "no",
                    GrantId = 7,
                    Reason = "Applicant should not be cared by State"
                },new GrantAnswers
                {
                    QuestionId = 13,
                    Answer = "no",
                    GrantId = 7,
                    Reason = "Applicant should not recieve another grant"
                },new GrantAnswers
                {
                    QuestionId =14,
                    Answer = "yes",
                    GrantId = 8,
                    Reason = "Applicant must have fought in the Second World War or Korean War"
                },new GrantAnswers
                {
                    QuestionId = 15,
                    Answer = "no",
                    GrantId = 8,
                    Reason = "Applicant should not be cared by State"
                },new GrantAnswers
                {
                    QuestionId =16,
                    Answer = "no",
                    GrantId = 8,
                    Reason = "Applicant should not receive another grant"
                }); await context.SaveChangesAsync();
            }
            if (await context.TblPostOffices.AnyAsync())
            {
                return;
            }
            else
            {
                await context.TblPostOffices.AddRangeAsync(
                new PostOffice { Name = "Babirwa", CityId = 1 },
                new PostOffice { Name = "Bandelierkop", CityId = 1 },
                new PostOffice { Name = "Ba-Phalaborwa", CityId = 1 },
                new PostOffice { Name = "Bonisani", CityId = 1 },
                new PostOffice { Name = "Braambos", CityId = 1 },
                new PostOffice { Name = "Elim Hospitaal", CityId = 1 },
                new PostOffice { Name = "Fhatuwani", CityId = 1 },
                new PostOffice { Name = "Folovhodwe", CityId = 1 },
                new PostOffice { Name = "Fondwe", CityId = 1 },
                new PostOffice { Name = "Dzanani", CityId = 1 },
                new PostOffice { Name = "Dzimauli", CityId = 1 },
                new PostOffice { Name = "Eka Homu", CityId = 1 },
                new PostOffice { Name = "Fumani", CityId = 1 },
                new PostOffice { Name = "Giyani", CityId = 1 },
                new PostOffice { Name = "Gravelotte", CityId = 1 },
                new PostOffice { Name = "Gumbani", CityId = 1 },
                new PostOffice { Name = "Ha-Makuya", CityId = 1 },
                new PostOffice { Name = "Hubyeni", CityId = 1 },
                new PostOffice { Name = "Indermark", CityId = 1 },
                new PostOffice { Name = "Khakhu", CityId = 1 },
                new PostOffice { Name = "Khomanani", CityId = 1 },
                new PostOffice { Name = "Kuranta", CityId = 1 },
                new PostOffice { Name = "Kutama", CityId = 1 },
                new PostOffice { Name = "Lambani", CityId = 1 },
                new PostOffice { Name = "Levubu", CityId = 1 },
                new PostOffice { Name = "Lulekani", CityId = 1 },
                new PostOffice { Name = "Lwamondo", CityId = 1 },
                new PostOffice { Name = "Mahatlani", CityId = 1 },
                new PostOffice { Name = "Makhado", CityId = 1 },
                new PostOffice { Name = "Makonde", CityId = 1 },
                new PostOffice { Name = "Makuleke", CityId = 1 },
                new PostOffice { Name = "Manenzhe", CityId = 1 },
                new PostOffice { Name = "Masakona", CityId = 1 },
                new PostOffice { Name = "Mashamba", CityId = 1 },
                new PostOffice { Name = "Mashau", CityId = 1 },
                new PostOffice { Name = "Masia", CityId = 1 },
                new PostOffice { Name = "Masingita", CityId = 1 },
                new PostOffice { Name = "Masisi", CityId = 1 },
                new PostOffice { Name = "Matavhela", CityId = 1 },
                new PostOffice { Name = "Matidze", CityId = 1 },
                new PostOffice { Name = "Mhinga", CityId = 1 },
                new PostOffice { Name = "Mokwakwaila", CityId = 1 },
                new PostOffice { Name = "Molototsi", CityId = 1 },
                new PostOffice { Name = "Mudimeli", CityId = 1 },
                new PostOffice { Name = "Muila", CityId = 1 },
                new PostOffice { Name = "Mukula", CityId = 1 },
                new PostOffice { Name = "Mulima", CityId = 1 },
                new PostOffice { Name = "Mungomani", CityId = 1 },
                new PostOffice { Name = "Munzhedzi", CityId = 1 },
                new PostOffice { Name = "Musina", CityId = 1 },
                new PostOffice { Name = "Mutale", CityId = 1 },
                new PostOffice { Name = "Muwaweni", CityId = 1 },
                new PostOffice { Name = "Muyexe", CityId = 1 },
                new PostOffice { Name = "Namakgale", CityId = 1 },
                new PostOffice { Name = "Nkuri", CityId = 1 },
                new PostOffice { Name = "Nwamankena", CityId = 1 },
                new PostOffice { Name = "Nwamanungu", CityId = 1 },
                new PostOffice { Name = "Nzhelele", CityId = 1 },
                new PostOffice { Name = "Paulusweg", CityId = 1 },
                new PostOffice { Name = "Phangami", CityId = 1 },
                new PostOffice { Name = "Phipidi", CityId = 1 },
                new PostOffice { Name = "Sagole", CityId = 1 },
                new PostOffice { Name = "Sambo", CityId = 1 },
                new PostOffice { Name = "Saselamani", CityId = 1 },
                new PostOffice { Name = "Selwana", CityId = 1 },
                new PostOffice { Name = "Shakadza", CityId = 1 },
                new PostOffice { Name = "Shayandima", CityId = 1 },
                new PostOffice { Name = "Sibasa", CityId = 1 },
                new PostOffice { Name = "Sifahla", CityId = 1 },
                new PostOffice { Name = "Thohoyandou", CityId = 1 },
                new PostOffice { Name = "Tshakhuma", CityId = 1 },
                new PostOffice { Name = "Tshaulu", CityId = 1 },
                new PostOffice { Name = "Tshidimbini", CityId = 1 },
                new PostOffice { Name = "Tshififi", CityId = 1 },
                new PostOffice { Name = "Tshifudi", CityId = 1 },
                new PostOffice { Name = "Tshilaphala", CityId = 1 },
                new PostOffice { Name = "Tshilwavhusiku", CityId = 1 },
                new PostOffice { Name = "Tshimbupfe", CityId = 1 },
                new PostOffice { Name = "Tshipise", CityId = 1 },
                new PostOffice { Name = "Tshixwadza", CityId = 1 },
                new PostOffice { Name = "Valdezia", CityId = 1 },
                new PostOffice { Name = "Vhahlavi", CityId = 1 },
                new PostOffice { Name = "Vhufuli", CityId = 1 },
                new PostOffice { Name = "Vhulaudzi", CityId = 1 },
                new PostOffice { Name = "Vivo", CityId = 1 },
                new PostOffice { Name = "Vongani", CityId = 1 },
                new PostOffice { Name = "Vuwani", CityId = 1 },
                new PostOffice { Name = "Vuyani", CityId = 1 },
                new PostOffice { Name = "Waterpoort", CityId = 1 },
                new PostOffice { Name = "Xigalo", CityId = 1 },
                new PostOffice { Name = "Xikundu", CityId = 1 },
                new PostOffice { Name = "Zava", CityId = 1 },
                new PostOffice { Name = "Buysdorp", CityId = 1 },
                new PostOffice { Name = "Abel", CityId = 2 },
                new PostOffice { Name = "Addney", CityId = 2 },
                new PostOffice { Name = "Alldays", CityId = 2 },
                new PostOffice { Name = "Ambergate", CityId = 2 },
                new PostOffice { Name = "Apel", CityId = 2 },
                new PostOffice { Name = "Atok", CityId = 2 },
                new PostOffice { Name = "Boyne", CityId = 2 },
                new PostOffice { Name = "Coblentz", CityId = 2 },
                new PostOffice { Name = "Dendron", CityId = 2 },
                new PostOffice { Name = "Dikgale", CityId = 2 },
                new PostOffice { Name = "Ditlhophaneng", CityId = 2 },
                new PostOffice { Name = "Dwarsrivier", CityId = 2 },
                new PostOffice { Name = "Ga-Kgapane", CityId = 2 },
                new PostOffice { Name = "Ga-Maraba", CityId = 2 },
                new PostOffice { Name = "Gamothiba", CityId = 2 },
                new PostOffice { Name = "Ga-Setati", CityId = 2 },
                new PostOffice { Name = "Haenertsburg", CityId = 2 },
                new PostOffice { Name = "Jane Furse Hospitaal", CityId = 2 },
                new PostOffice { Name = "Kgohloane", CityId = 2 },
                new PostOffice { Name = "Koloti", CityId = 2 },
                new PostOffice { Name = "Ladanna", CityId = 2 },
                new PostOffice { Name = "Lebowakgomo", CityId = 2 },
                new PostOffice { Name = "Lefalane", CityId = 2 },
                new PostOffice { Name = "Lengeta", CityId = 2 },
                new PostOffice { Name = "Lenyenye", CityId = 2 },
                new PostOffice { Name = "Leokaneng", CityId = 2 },
                new PostOffice { Name = "Letaba", CityId = 2 },
                new PostOffice { Name = "Lonsdale", CityId = 2 },
                new PostOffice { Name = "Maake", CityId = 2 },
                new PostOffice { Name = "Mabale", CityId = 2 },
                new PostOffice { Name = "Madiba Park", CityId = 2 },
                new PostOffice { Name = "Mafarana", CityId = 2 },
                new PostOffice { Name = "Mafefe", CityId = 2 },
                new PostOffice { Name = "Maja", CityId = 2 },
                new PostOffice { Name = "Makhudu", CityId = 2 },
                new PostOffice { Name = "Malatane", CityId = 2 },
                new PostOffice { Name = "Maleboho", CityId = 2 },
                new PostOffice { Name = "Manganeng", CityId = 2 },
                new PostOffice { Name = "Manthata", CityId = 2 },
                new PostOffice { Name = "Manyama", CityId = 2 },
                new PostOffice { Name = "Maribana", CityId = 2 },
                new PostOffice { Name = "Mathabatha", CityId = 2 },
                new PostOffice { Name = "Medingen", CityId = 2 },
                new PostOffice { Name = "Mmotong", CityId = 2 },
                new PostOffice { Name = "Mobile 7", CityId = 2 },
                new PostOffice { Name = "Mobile 8", CityId = 2 },
                new PostOffice { Name = "Modjadji", CityId = 2 },
                new PostOffice { Name = "Modjadjiskloof", CityId = 2 },
                new PostOffice { Name = "Moetladimo", CityId = 2 },
                new PostOffice { Name = "Mogodumo", CityId = 2 },
                new PostOffice { Name = "Mokgwati", CityId = 2 },
                new PostOffice { Name = "Monakhi", CityId = 2 },
                new PostOffice { Name = "Mooilyk", CityId = 2 },
                new PostOffice { Name = "Mooketsi", CityId = 2 },
                new PostOffice { Name = "Morebeng", CityId = 2 },
                new PostOffice { Name = "Moroke", CityId = 2 },
                new PostOffice { Name = "Mosoroni", CityId = 2 },
                new PostOffice { Name = "Motlhele", CityId = 2 },
                new PostOffice { Name = "Mphahlele", CityId = 2 },
                new PostOffice { Name = "Mphogodiba", CityId = 2 },
                new PostOffice { Name = "Mushung", CityId = 2 },
                new PostOffice { Name = "My Darling", CityId = 2 },
                new PostOffice { Name = "Neandertal", CityId = 2 },
                new PostOffice { Name = "Nkambako", CityId = 2 },
                new PostOffice { Name = "Nkowankowa", CityId = 2 },
                new PostOffice { Name = "Ntsima", CityId = 2 },
                new PostOffice { Name = "Nwamitwa", CityId = 2 },
                new PostOffice { Name = "Paledi", CityId = 2 },
                new PostOffice { Name = "Polokwane", CityId = 2 },
                new PostOffice { Name = "Polokwane North", CityId = 2 },
                new PostOffice { Name = "Raditshaba", CityId = 2 },
                new PostOffice { Name = "Ramokgopa", CityId = 2 },
                new PostOffice { Name = "Rozano", CityId = 2 },
                new PostOffice { Name = "Segopje", CityId = 2 },
                new PostOffice { Name = "Sekgopo", CityId = 2 },
                new PostOffice { Name = "Sekhukhune", CityId = 2 },
                new PostOffice { Name = "Sekhung", CityId = 2 },
                new PostOffice { Name = "Selota", CityId = 2 },
                new PostOffice { Name = "Senwabarwana", CityId = 2 },
                new PostOffice { Name = "Solomondale", CityId = 2 },
                new PostOffice { Name = "Sovenga", CityId = 2 },
                new PostOffice { Name = "Superbia", CityId = 2 },
                new PostOffice { Name = "Tarentaalrand", CityId = 2 },
                new PostOffice { Name = "Thaba", CityId = 2 },
                new PostOffice { Name = "Tholongwe", CityId = 2 },
                new PostOffice { Name = "Trichardtsdal", CityId = 2 },
                new PostOffice { Name = "Tsate", CityId = 2 },
                new PostOffice { Name = "Tzaneen", CityId = 2 },
                new PostOffice { Name = "Westenberg", CityId = 2 },
                new PostOffice { Name = "Xihoko", CityId = 2 },
                new PostOffice { Name = "Chuenespoort", CityId = 2 },
                new PostOffice { Name = "Bakenberg", CityId = 3 },
                new PostOffice { Name = "Bakone", CityId = 3 },
                new PostOffice { Name = "Baltimore", CityId = 3 },
                new PostOffice { Name = "Bela Bela Warmbad", CityId = 3 },
                new PostOffice { Name = "Enkelbult", CityId = 3 },
                new PostOffice { Name = "Dumphries", CityId = 4 },
                new PostOffice { Name = "Dwaalboom", CityId = 3 },
                new PostOffice { Name = "Galebelo", CityId = 3 },
                new PostOffice { Name = "Glen Cowie", CityId = 5 },
                new PostOffice { Name = "Gompies", CityId = 3 },
                new PostOffice { Name = "Groblersdal", CityId = 5 },
                new PostOffice { Name = "Groothoekhospitaal", CityId = 3 },
                new PostOffice { Name = "Hlogo Ya Nku", CityId = 3 },
                new PostOffice { Name = "Hoedspruit", CityId = 4 },
                new PostOffice { Name = "Juno", CityId = 3 },
                new PostOffice { Name = "Koedoeskop", CityId = 3 },
                new PostOffice { Name = "Kokanje", CityId = 3 },
                new PostOffice { Name = "Kwalitho", CityId = 3 },
                new PostOffice { Name = "Leeupoort", CityId = 3 },
                new PostOffice { Name = "Legogwe", CityId = 4 },
                new PostOffice { Name = "Lephalale", CityId = 3 },
                new PostOffice { Name = "Limburg", CityId = 3 },
                new PostOffice { Name = "Maasstroom", CityId = 3 },
                new PostOffice { Name = "Mabatlane", CityId = 3 },
                new PostOffice { Name = "Mahwelereng", CityId = 3 },
                new PostOffice { Name = "Mapela", CityId = 3 },
                new PostOffice { Name = "Marishane", CityId = 5 },
                new PostOffice { Name = "Marken", CityId = 3 },
                new PostOffice { Name = "Maroela", CityId = 3 },
                new PostOffice { Name = "Masemola", CityId = 5 },
                new PostOffice { Name = "Mashashane", CityId = 3 },
                new PostOffice { Name = "Matlalane", CityId = 3 },
                new PostOffice { Name = "Modimolle", CityId = 3 },
                new PostOffice { Name = "Moganyaka", CityId = 5 },
                new PostOffice { Name = "Mokamole", CityId = 3 },
                new PostOffice { Name = "Mokopane", CityId = 3 },
                new PostOffice { Name = "Moletlane", CityId = 3 },
                new PostOffice { Name = "Mookgophong", CityId = 3 },
                new PostOffice { Name = "Motetema", CityId = 5 },
                new PostOffice { Name = "Moteti", CityId = 5 },
                new PostOffice { Name = "Nebo", CityId = 5 },
                new PostOffice { Name = "New Eersterus", CityId = 3 },
                new PostOffice { Name = "NITS Head Office", CityId = 6 },
                new PostOffice { Name = "Nkwe", CityId = 3 },
                new PostOffice { Name = "Northam", CityId = 3 },
                new PostOffice { Name = "Ohrigstad", CityId = 4 },
                new PostOffice { Name = "Onverwacht", CityId = 3 },
                new PostOffice { Name = "Phalala", CityId = 3 },
                new PostOffice { Name = "Pienaarsrivier", CityId = 3 },
                new PostOffice { Name = "Platinum Reef", CityId = 3 },
                new PostOffice { Name = "Ramatatane", CityId = 3 },
                new PostOffice { Name = "Rebone", CityId = 3 },
                new PostOffice { Name = "Roedtan", CityId = 3 },
                new PostOffice { Name = "Rooiberg", CityId = 3 },
                new PostOffice { Name = "Sehlakwane", CityId = 5 },
                new PostOffice { Name = "Seleka", CityId = 3 },
                new PostOffice { Name = "Settlers", CityId = 3 },
                new PostOffice { Name = "Seven Stad", CityId = 5 },
                new PostOffice { Name = "Steelpoort", CityId = 4 },
                new PostOffice { Name = "Swartbooistad", CityId = 3 },
                new PostOffice { Name = "Swartklip", CityId = 3 },
                new PostOffice { Name = "Swartwater", CityId = 3 },
                new PostOffice { Name = "Taueatswala", CityId = 3 },
                new PostOffice { Name = "Thabazimbi", CityId = 3 },
                new PostOffice { Name = "Tibedi", CityId = 3 },
                new PostOffice { Name = "Tom Burke", CityId = 3 },
                new PostOffice { Name = "Treves", CityId = 3 },
                new PostOffice { Name = "Tshilwaneng", CityId = 5 },
                new PostOffice { Name = "Villa Nora", CityId = 3 },
                new PostOffice { Name = "Xipame", CityId = 5 },
                new PostOffice { Name = "Zondereinde", CityId = 3 },
                new PostOffice { Name = "Burgersfort", CityId = 4 },
                new PostOffice { Name = "Chromite", CityId = 3 },
                new PostOffice { Name = "Aberdeen", CityId = 7 },
                new PostOffice { Name = "Addo", CityId = 7 },
                new PostOffice { Name = "Adendorp", CityId = 7 },
                new PostOffice { Name = "Alexandria", CityId = 7 },
                new PostOffice { Name = "Algoapark", CityId = 8 },
                new PostOffice { Name = "Alice", CityId = 9 },
                new PostOffice { Name = "Alicedale", CityId = 7 },
                new PostOffice { Name = "Aliwal North", CityId = 9 },
                new PostOffice { Name = "Amadiba", CityId = 10 },
                new PostOffice { Name = "Amalinda", CityId = 11 },
                new PostOffice { Name = "Askeaton", CityId = 12 },
                new PostOffice { Name = "Bagqozini", CityId = 10 },
                new PostOffice { Name = "Bamboesspruit", CityId = 9 },
                new PostOffice { Name = "Barkly East", CityId = 9 },
                new PostOffice { Name = "Bathurst", CityId = 7 },
                new PostOffice { Name = "Bay West", CityId = 8 },
                new PostOffice { Name = "Beacon Bay", CityId = 11 },
                new PostOffice { Name = "Bellrock", CityId = 13 },
                new PostOffice { Name = "Bengu", CityId = 12 },
                new PostOffice { Name = "Bhisho", CityId = 11 },
                new PostOffice { Name = "Bityi", CityId = 14 },
                new PostOffice { Name = "Bizana", CityId = 10 },
                new PostOffice { Name = "Blythswood", CityId = 13 },
                new PostOffice { Name = "Bojeni", CityId = 13 },
                new PostOffice { Name = "Boknesstrand", CityId = 7 },
                new PostOffice { Name = "Bolo Reserve", CityId = 13 },
                new PostOffice { Name = "Cintsa East", CityId = 13 },
                new PostOffice { Name = "Cisira", CityId = 13 },
                new PostOffice { Name = "Clarendon Marine", CityId = 8 },
                new PostOffice { Name = "Clarkson", CityId = 7 },
                new PostOffice { Name = "Coghlan", CityId = 14 },
                new PostOffice { Name = "Cookhouse", CityId = 7 },
                new PostOffice { Name = "Coville", CityId = 9 },
                new PostOffice { Name = "Cradock", CityId = 12 },
                new PostOffice { Name = "Darabe", CityId = 14 },
                new PostOffice { Name = "Debe Nek", CityId = 9 },
                new PostOffice { Name = "Despatch", CityId = 8 },
                new PostOffice { Name = "Elliot", CityId = 12 },
                new PostOffice { Name = "Elliotdale", CityId = 13 },
                new PostOffice { Name = "Emantlaneni", CityId = 14 },
                new PostOffice { Name = "Enon", CityId = 7 },
                new PostOffice { Name = "Fitches Corner", CityId = 8 },
                new PostOffice { Name = "Flagstaff", CityId = 10 },
                new PostOffice { Name = "Fletcherville", CityId = 9 },
                new PostOffice { Name = "Dordrecht", CityId = 12 },
                new PostOffice { Name = "East London", CityId = 11 },
                new PostOffice { Name = "Elalini", CityId = 14 },
                new PostOffice { Name = "Fullarton", CityId = 7 },
                new PostOffice { Name = "Gengqe", CityId = 14 },
                new PostOffice { Name = "Glenconner", CityId = 7 },
                new PostOffice { Name = "Golden Valley", CityId = 7 },
                new PostOffice { Name = "Gonubie", CityId = 11 },
                new PostOffice { Name = "Gqeberha (Port Elizabeth)", CityId = 8 },
                new PostOffice { Name = "Graaff-Reinet", CityId = 7 },
                new PostOffice { Name = "Greenfields", CityId = 11 },
                new PostOffice { Name = "Gwadana", CityId = 13 },
                new PostOffice { Name = "Hackney", CityId = 12 },
                new PostOffice { Name = "Haga-Haga", CityId = 13 },
                new PostOffice { Name = "Hankey", CityId = 7 },
                new PostOffice { Name = "Hofmeyr", CityId = 12 },
                new PostOffice { Name = "Hogsback", CityId = 13 },
                new PostOffice { Name = "Hoita (Mlamli)", CityId = 9 },
                new PostOffice { Name = "Humansdorp", CityId = 7 },
                new PostOffice { Name = "Idutywa", CityId = 13 },
                new PostOffice { Name = "Ilinge", CityId = 12 },
                new PostOffice { Name = "Ilitha", CityId = 11 },
                new PostOffice { Name = "Jamestown", CityId = 9 },
                new PostOffice { Name = "Jansenville", CityId = 7 },
                new PostOffice { Name = "Jeffrey's Bay", CityId = 7 },
                new PostOffice { Name = "Joubertina", CityId = 7 },
                new PostOffice { Name = "Kamastone", CityId = 12 },
                new PostOffice { Name = "Kariega (Uitenhage)", CityId = 8 },
                new PostOffice { Name = "Keiskammahoek", CityId = 9 },
                new PostOffice { Name = "Kenako", CityId = 8 },
                new PostOffice { Name = "Kentani", CityId = 13 },
                new PostOffice { Name = "Kenton On Sea", CityId = 7 },
                new PostOffice { Name = "Kirkwood", CityId = 7 },
                new PostOffice { Name = "Kleinbos", CityId = 7 },
                new PostOffice { Name = "Kleinpoort", CityId = 7 },
                new PostOffice { Name = "Klipplaat", CityId = 7 },
                new PostOffice { Name = "Kohlo", CityId = 14 },
                new PostOffice { Name = "Kolomana", CityId = 13 },
                new PostOffice { Name = "Komga", CityId = 13 },
                new PostOffice { Name = "Kwaloyiti", CityId = 11 },
                new PostOffice { Name = "Kwa-Meyi", CityId = 9 },
                new PostOffice { Name = "Kwanobuhle", CityId = 8 },
                new PostOffice { Name = "Lady Grey", CityId = 9 },
                new PostOffice { Name = "Libode", CityId = 14 },
                new PostOffice { Name = "Lingelihle", CityId = 12 },
                new PostOffice { Name = "Linton Grange", CityId = 8 },
                new PostOffice { Name = "Ludeke", CityId = 10 },
                new PostOffice { Name = "Lulet", CityId = 7 },
                new PostOffice { Name = "Lurwayizo", CityId = 13 },
                new PostOffice { Name = "Lusikisiki", CityId = 14 },
                new PostOffice { Name = "Macleantown", CityId = 11 },
                new PostOffice { Name = "Mahlungulu", CityId = 14 },
                new PostOffice { Name = "Makhanda", CityId = 7 },
                new PostOffice { Name = "Maluti", CityId = 10 },
                new PostOffice { Name = "Mandileni", CityId = 10 },
                new PostOffice { Name = "Mangoloaneng", CityId = 9 },
                new PostOffice { Name = "Manley Flats", CityId = 7 },
                new PostOffice { Name = "Martindale", CityId = 7 },
                new PostOffice { Name = "Masincedane", CityId = 10 },
                new PostOffice { Name = "Matatiele", CityId = 10 },
                new PostOffice { Name = "Mavelebayi", CityId = 12 },
                new PostOffice { Name = "Mdantsane", CityId = 11 },
                new PostOffice { Name = "Melon", CityId = 7 },
                new PostOffice { Name = "Mfundisweni", CityId = 10 },
                new PostOffice { Name = "Mgojweni", CityId = 14 },
                new PostOffice { Name = "Michausdal", CityId = 12 },
                new PostOffice { Name = "Middelburg (Cape)", CityId = 12 },
                new PostOffice { Name = "Middledrift", CityId = 9 },
                new PostOffice { Name = "Middleton", CityId = 7 },
                new PostOffice { Name = "Misgund", CityId = 7 },
                new PostOffice { Name = "Mkemani", CityId = 10 },
                new PostOffice { Name = "Mlungisi", CityId = 12 },
                new PostOffice { Name = "Mnqaba Khulile", CityId = 13 },
                new PostOffice { Name = "Mnyolo", CityId = 12 },
                new PostOffice { Name = "Molteno", CityId = 12 },
                new PostOffice { Name = "Mooiplaas", CityId = 11 },
                new PostOffice { Name = "Morgans Bay", CityId = 11 },
                new PostOffice { Name = "Mortimer", CityId = 12 },
                new PostOffice { Name = "Motherwell Nu4", CityId = 8 },
                new PostOffice { Name = "Mount Ayliff", CityId = 10 },
                new PostOffice { Name = "Mount Frere", CityId = 10 },
                new PostOffice { Name = "Mqanduli", CityId = 14 },
                new PostOffice { Name = "Mqonci", CityId = 13 },
                new PostOffice { Name = "Mt Fletcher", CityId = 10 },
                new PostOffice { Name = "Mthatha", CityId = 14 },
                new PostOffice { Name = "Mtukukazi", CityId = 10 },
                new PostOffice { Name = "Mvenyane", CityId = 10 },
                new PostOffice { Name = "Newton Park", CityId = 8 },
                new PostOffice { Name = "Ngcobo", CityId = 14 },
                new PostOffice { Name = "Ngonyama", CityId = 12 },
                new PostOffice { Name = "Ngozi", CityId = 10 },
                new PostOffice { Name = "Ngqele", CityId = 13 },
                new PostOffice { Name = "Ngqeleni", CityId = 14 },
                new PostOffice { Name = "Ngwenyathi", CityId = 11 },
                new PostOffice { Name = "Nieu-Bethesda", CityId = 7 },
                new PostOffice { Name = "NkoboNkobo", CityId = 13 },
                new PostOffice { Name = "Nocwane", CityId = 13 },
                new PostOffice { Name = "Nomvalo", CityId = 14 },
                new PostOffice { Name = "North End", CityId = 8 },
                new PostOffice { Name = "Nqaqarhu (Maclear)", CityId = 9 },
                new PostOffice { Name = "Ntlaza", CityId = 14 },
                new PostOffice { Name = "Ntsizwa", CityId = 10 },
                new PostOffice { Name = "Nxaruni", CityId = 11 },
                new PostOffice { Name = "Nyandana", CityId = 12 },
                new PostOffice { Name = "Osborn Mission", CityId = 14 },
                new PostOffice { Name = "Oubosrand", CityId = 7 },
                new PostOffice { Name = "Oyster Bay", CityId = 7 },
                new PostOffice { Name = "Patensie", CityId = 7 },
                new PostOffice { Name = "Pearston", CityId = 7 },
                new PostOffice { Name = "Peddie", CityId = 11 },
                new PostOffice { Name = "Port Alfred", CityId = 7 },
                new PostOffice { Name = "Port St Johns", CityId = 14 },
                new PostOffice { Name = "Qanqu", CityId = 14 },
                new PostOffice { Name = "Queenstown", CityId = 12 },
                new PostOffice { Name = "Quimera", CityId = 9 },
                new PostOffice { Name = "Qumbu", CityId = 14 },
                new PostOffice { Name = "Qunu", CityId = 14 },
                new PostOffice { Name = "Redoubt", CityId = 10 },
                new PostOffice { Name = "Rhodes", CityId = 9 },
                new PostOffice { Name = "Riebeeck East", CityId = 7 },
                new PostOffice { Name = "Riebeeckhoogte", CityId = 8 },
                new PostOffice { Name = "Rietbron", CityId = 7 },
                new PostOffice { Name = "Rode", CityId = 10 },
                new PostOffice { Name = "Romanslaagte", CityId = 12 },
                new PostOffice { Name = "Sabasaba", CityId = 10 },
                new PostOffice { Name = "Saltville", CityId = 8 },
                new PostOffice { Name = "Setabataba", CityId = 9 },
                new PostOffice { Name = "Seymour", CityId = 9 },
                new PostOffice { Name = "Sheshegu", CityId = 13 },
                new PostOffice { Name = "Shukunxa", CityId = 14 },
                new PostOffice { Name = "Sidwadweni", CityId = 14 },
                new PostOffice { Name = "Sidwell", CityId = 8 },
                new PostOffice { Name = "Sihlabeni", CityId = 13 },
                new PostOffice { Name = "Silimela", CityId = 14 },
                new PostOffice { Name = "Somerset East", CityId = 7 },
                new PostOffice { Name = "Southernwood", CityId = 11 },
                new PostOffice { Name = "Southeyville", CityId = 12 },
                new PostOffice { Name = "Southseas", CityId = 7 },
                new PostOffice { Name = "St Albans", CityId = 8 },
                new PostOffice { Name = "St Cuthbert's", CityId = 14 },
                new PostOffice { Name = "St Matthews", CityId = 13 },
                new PostOffice { Name = "Sterkspruit", CityId = 9 },
                new PostOffice { Name = "Sterkstroom", CityId = 12 },
                new PostOffice { Name = "Steynsburg", CityId = 9 },
                new PostOffice { Name = "Steytlerville", CityId = 7 },
                new PostOffice { Name = "Studtis", CityId = 7 },
                new PostOffice { Name = "Stutterheim", CityId = 13 },
                new PostOffice { Name = "Sugarbush", CityId = 10 },
                new PostOffice { Name = "Sulenkama", CityId = 14 },
                new PostOffice { Name = "Summerstrand", CityId = 8 },
                new PostOffice { Name = "Summerville", CityId = 7 },
                new PostOffice { Name = "Sunland", CityId = 7 },
                new PostOffice { Name = "Swartkops", CityId = 8 },
                new PostOffice { Name = "Taleni", CityId = 13 },
                new PostOffice { Name = "Tarkastad", CityId = 12 },
                new PostOffice { Name = "Thornham", CityId = 7 },
                new PostOffice { Name = "Thornhill", CityId = 7 },
                new PostOffice { Name = "Thornpark", CityId = 11 },
                new PostOffice { Name = "Tinana", CityId = 9 },
                new PostOffice { Name = "Tombo", CityId = 14 },
                new PostOffice { Name = "Tsatsana", CityId = 9 },
                new PostOffice { Name = "Tshonya", CityId = 14 },
                new PostOffice { Name = "Tsilitwa", CityId = 14 },
                new PostOffice { Name = "Tsolo", CityId = 14 },
                new PostOffice { Name = "Tsomo", CityId = 12 },
                new PostOffice { Name = "Twee Riviere", CityId = 7 },
                new PostOffice { Name = "Tyinindini", CityId = 9 },
                new PostOffice { Name = "Ugie", CityId = 9 },
                new PostOffice { Name = "Venterstad", CityId = 9 },
                new PostOffice { Name = "Visrivier", CityId = 12 },
                new PostOffice { Name = "Vulindlela Heights", CityId = 14 },
                new PostOffice { Name = "Walmer", CityId = 8 },
                new PostOffice { Name = "West Bank", CityId = 11 },
                new PostOffice { Name = "Whittlesea", CityId = 12 },
                new PostOffice { Name = "Willowmore", CityId = 7 },
                new PostOffice { Name = "Willowvale", CityId = 13 },
                new PostOffice { Name = "Witelsbos", CityId = 7 },
                new PostOffice { Name = "Witmos", CityId = 7 },
                new PostOffice { Name = "Zalu", CityId = 14 },
                new PostOffice { Name = "Zincuka", CityId = 9 },
                new PostOffice { Name = "Braunville", CityId = 13 },
                new PostOffice { Name = "Buffalo Flats", CityId = 11 },
                new PostOffice { Name = "Bungeni", CityId = 14 },
                new PostOffice { Name = "Burgersdorp", CityId = 9 },
                new PostOffice { Name = "Butterworth", CityId = 13 },
                new PostOffice { Name = "Cambridge", CityId = 11 },
                new PostOffice { Name = "Cannon Rocks", CityId = 7 },
                new PostOffice { Name = "Cathcart", CityId = 13 },
                new PostOffice { Name = "Cedarville", CityId = 10 },
                new PostOffice { Name = "Chibini", CityId = 12 },
                new PostOffice { Name = "Albertinia", CityId = 15 },
                new PostOffice { Name = "Area Office-Atlantic Coast", CityId = 16 },
                new PostOffice { Name = "Area Office-Cape Winelands", CityId = 17 },
                new PostOffice { Name = "Area Office-Eden Karoo", CityId = 15 },
                new PostOffice { Name = "Area Office-Overberg", CityId = 18 },
                new PostOffice { Name = "Area Office-Peninsula East", CityId = 19 },
                new PostOffice { Name = "Area Office-Peninsula North", CityId = 20 },
                new PostOffice { Name = "Area Office-Peninsula South", CityId = 21 },
                new PostOffice { Name = "Athlone", CityId = 19 },
                new PostOffice { Name = "Attaway", CityId = 16 },
                new PostOffice { Name = "Aurora", CityId = 16 },
                new PostOffice { Name = "Avontuur", CityId = 15 },
                new PostOffice { Name = "Barrydale", CityId = 18 },
                new PostOffice { Name = "Beaufort West", CityId = 15 },
                new PostOffice { Name = "Belhar", CityId = 20 },
                new PostOffice { Name = "Bella Vista", CityId = 17 },
                new PostOffice { Name = "Bellville", CityId = 20 },
                new PostOffice { Name = "Betty'S Bay", CityId = 18 },
                new PostOffice { Name = "Bitterfontein", CityId = 16 },
                new PostOffice { Name = "Bonnievale", CityId = 18 },
                new PostOffice { Name = "Bontheuwel", CityId = 19 },
                new PostOffice { Name = "Botha", CityId = 17 },
                new PostOffice { Name = "Botrivier", CityId = 18 },
                new PostOffice { Name = "Brackenfell", CityId = 20 },
                new PostOffice { Name = "Brandwacht", CityId = 15 },
                new PostOffice { Name = "Citrusdal", CityId = 16 },
                new PostOffice { Name = "Clanwilliam", CityId = 16 },
                new PostOffice { Name = "Clareinch", CityId = 21 },
                new PostOffice { Name = "Da Gamaskop", CityId = 15 },
                new PostOffice { Name = "Darling", CityId = 16 },
                new PostOffice { Name = "De Doorns", CityId = 18 },
                new PostOffice { Name = "Delft", CityId = 19 },
                new PostOffice { Name = "Elim", CityId = 18 },
                new PostOffice { Name = "Elonwabeni", CityId = 19 },
                new PostOffice { Name = "Elsenburg", CityId = 17 },
                new PostOffice { Name = "Elsiesrivier", CityId = 20 },
                new PostOffice { Name = "Franschhoek", CityId = 17 },
                new PostOffice { Name = "Friemersheim", CityId = 15 },
                new PostOffice { Name = "Durbanville", CityId = 20 },
                new PostOffice { Name = "Durrheim", CityId = 20 },
                new PostOffice { Name = "Dysselsdorp", CityId = 15 },
                new PostOffice { Name = "Ebenhaeser", CityId = 16 },
                new PostOffice { Name = "Eersterivier", CityId = 19 },
                new PostOffice { Name = "Gansbaai", CityId = 18 },
                new PostOffice { Name = "Gatesville", CityId = 19 },
                new PostOffice { Name = "George", CityId = 15 },
                new PostOffice { Name = "George-Industria", CityId = 15 },
                new PostOffice { Name = "Goedverwag", CityId = 16 },
                new PostOffice { Name = "Goodwood", CityId = 20 },
                new PostOffice { Name = "Gordon's Bay", CityId = 17 },
                new PostOffice { Name = "Goudiniweg", CityId = 17 },
                new PostOffice { Name = "Gouritsmond", CityId = 15 },
                new PostOffice { Name = "Graafwater", CityId = 16 },
                new PostOffice { Name = "Grabouw", CityId = 18 },
                new PostOffice { Name = "Grassy Park", CityId = 21 },
                new PostOffice { Name = "Groot-Brakrivier", CityId = 15 },
                new PostOffice { Name = "Groot-Jongensfontein", CityId = 15 },
                new PostOffice { Name = "Hammanshof", CityId = 17 },
                new PostOffice { Name = "Heidelberg (Cape)", CityId = 15 },
                new PostOffice { Name = "Herbertsdale", CityId = 15 },
                new PostOffice { Name = "Hermanus", CityId = 18 },
                new PostOffice { Name = "Hermon", CityId = 16 },
                new PostOffice { Name = "Herold", CityId = 15 },
                new PostOffice { Name = "Hexrivier", CityId = 17 },
                new PostOffice { Name = "Hoekwil", CityId = 15 },
                new PostOffice { Name = "Houtbaai", CityId = 21 },
                new PostOffice { Name = "Johnson's Post", CityId = 15 },
                new PostOffice { Name = "Karatara", CityId = 15 },
                new PostOffice { Name = "Katzenberg", CityId = 16 },
                new PostOffice { Name = "Kenilworth", CityId = 21 },
                new PostOffice { Name = "Khayelitsha", CityId = 19 },
                new PostOffice { Name = "Klawer", CityId = 16 },
                new PostOffice { Name = "Kliprand", CityId = 16 },
                new PostOffice { Name = "Knysna", CityId = 15 },
                new PostOffice { Name = "Koringberg", CityId = 16 },
                new PostOffice { Name = "Kraaifontein", CityId = 20 },
                new PostOffice { Name = "Kuilsrivier", CityId = 20 },
                new PostOffice { Name = "Kwanokuthula", CityId = 15 },
                new PostOffice { Name = "La Motte", CityId = 17 },
                new PostOffice { Name = "Ladismith", CityId = 15 },
                new PostOffice { Name = "L'Agulhas", CityId = 18 },
                new PostOffice { Name = "Laingsburg", CityId = 15 },
                new PostOffice { Name = "Lambertsbaai", CityId = 16 },
                new PostOffice { Name = "Langebaan", CityId = 16 },
                new PostOffice { Name = "Lansdowne", CityId = 19 },
                new PostOffice { Name = "Lavistown", CityId = 20 },
                new PostOffice { Name = "Leeu-Gamka", CityId = 15 },
                new PostOffice { Name = "Leipoldtville", CityId = 16 },
                new PostOffice { Name = "Lynedoch", CityId = 17 },
                new PostOffice { Name = "Macassar", CityId = 17 },
                new PostOffice { Name = "Maitland", CityId = 20 },
                new PostOffice { Name = "Malmesbury", CityId = 16 },
                new PostOffice { Name = "Mamre", CityId = 16 },
                new PostOffice { Name = "Matjiesfontein", CityId = 15 },
                new PostOffice { Name = "Matroosfontein", CityId = 20 },
                new PostOffice { Name = "Merweville", CityId = 15 },
                new PostOffice { Name = "Mfuleni", CityId = 19 },
                new PostOffice { Name = "Milnerton", CityId = 16 },
                new PostOffice { Name = "Molen Drift", CityId = 15 },
                new PostOffice { Name = "Montagu", CityId = 18 },
                new PostOffice { Name = "Moorreesburg", CityId = 16 },
                new PostOffice { Name = "Mosselbaai", CityId = 15 },
                new PostOffice { Name = "Nelspoort", CityId = 15 },
                new PostOffice { Name = "NITS CapeMail", CityId = 22 },
                new PostOffice { Name = "Noll", CityId = 15 },
                new PostOffice { Name = "Nuwerus", CityId = 16 },
                new PostOffice { Name = "Oudtshoorn", CityId = 15 },
                new PostOffice { Name = "Paarl", CityId = 17 },
                new PostOffice { Name = "Paarl East", CityId = 17 },
                new PostOffice { Name = "Parow", CityId = 20 },
                new PostOffice { Name = "Parow East", CityId = 20 },
                new PostOffice { Name = "Paternoster", CityId = 16 },
                new PostOffice { Name = "Pearly Beach", CityId = 18 },
                new PostOffice { Name = "Philippi", CityId = 19 },
                new PostOffice { Name = "Piketberg", CityId = 16 },
                new PostOffice { Name = "Plettenbergbaai", CityId = 15 },
                new PostOffice { Name = "Porterville", CityId = 16 },
                new PostOffice { Name = "Prince Albert", CityId = 15 },
                new PostOffice { Name = "Prince Alfred Hamlet", CityId = 17 },
                new PostOffice { Name = "Regional Office-Cape Metro", CityId = 23 },
                new PostOffice { Name = "Regional Office-Western Cape	Regional Office-", CityId = 22 },
                new PostOffice { Name = "Retreat", CityId = 21 },
                new PostOffice { Name = "Reygersdal", CityId = 16 },
                new PostOffice { Name = "Rhodes Gift", CityId = 21 },
                new PostOffice { Name = "Riebeeck-Wes", CityId = 16 },
                new PostOffice { Name = "Rietpoort", CityId = 16 },
                new PostOffice { Name = "Riversdale", CityId = 15 },
                new PostOffice { Name = "Riviersonderend", CityId = 18 },
                new PostOffice { Name = "Robertson", CityId = 18 },
                new PostOffice { Name = "Ruiterbos", CityId = 15 },
                new PostOffice { Name = "Saldanha", CityId = 16 },
                new PostOffice { Name = "Saron", CityId = 17 },
                new PostOffice { Name = "Sedgefield", CityId = 15 },
                new PostOffice { Name = "Sinksabrug", CityId = 15 },
                new PostOffice { Name = "SIS Western Region	SIS ", CityId = 22 },
                new PostOffice { Name = "Smutsville", CityId = 15 },
                new PostOffice { Name = "Somerset West", CityId = 17 },
                new PostOffice { Name = "Stanford", CityId = 18 },
                new PostOffice { Name = "Stellenbosch", CityId = 17 },
                new PostOffice { Name = "Stilbaai", CityId = 15 },
                new PostOffice { Name = "Strand", CityId = 17 },
                new PostOffice { Name = "Surwell", CityId = 19 },
                new PostOffice { Name = "Swellendam", CityId = 18 },
                new PostOffice { Name = "Table View", CityId = 16 },
                new PostOffice { Name = "The Craggs", CityId = 15 },
                new PostOffice { Name = "Thembalethu", CityId = 15 },
                new PostOffice { Name = "Touwsrivier", CityId = 18 },
                new PostOffice { Name = "Trawal", CityId = 16 },
                new PostOffice { Name = "Tulbagh", CityId = 17 },
                new PostOffice { Name = "Tyger Valley", CityId = 20 },
                new PostOffice { Name = "Uniondale", CityId = 15 },
                new PostOffice { Name = "Vanrhynsdorp", CityId = 16 },
                new PostOffice { Name = "Vermaaklikheid", CityId = 15 },
                new PostOffice { Name = "Villiersdorp", CityId = 18 },
                new PostOffice { Name = "Vishoek", CityId = 21 },
                new PostOffice { Name = "Vlaeberg", CityId = 21 },
                new PostOffice { Name = "Vlottenburg", CityId = 17 },
                new PostOffice { Name = "Vredenburg", CityId = 16 },
                new PostOffice { Name = "Vredendal", CityId = 16 },
                new PostOffice { Name = "Waboomskraal", CityId = 15 },
                new PostOffice { Name = "Wellington", CityId = 17 },
                new PostOffice { Name = "Wittedrif", CityId = 15 },
                new PostOffice { Name = "Wolseley", CityId = 17 },
                new PostOffice { Name = "Worcester", CityId = 17 },
                new PostOffice { Name = "Yzerfontein", CityId = 16 },
                new PostOffice { Name = "Bredasdorp", CityId = 18 },
                new PostOffice { Name = "Buffeljagsrivier", CityId = 18 },
                new PostOffice { Name = "Caledon", CityId = 18 },
                new PostOffice { Name = "Capemail Centre Court", CityId = 20 },
                new PostOffice { Name = "Caravelle", CityId = 19 },
                new PostOffice { Name = "Ceres", CityId = 17 },
                new PostOffice { Name = "Chempet", CityId = 16 },
                new PostOffice { Name = "Acornhoek", CityId = 26 },
                new PostOffice { Name = "Amersfoort", CityId = 27 },
                new PostOffice { Name = "Amsterdam", CityId = 27 },
                new PostOffice { Name = "Babethu", CityId = 28 },
                new PostOffice { Name = "Badplaas", CityId = 27 },
                new PostOffice { Name = "Balfour (North Central)", CityId = 27 },
                new PostOffice { Name = "Balmoral", CityId = 28 },
                new PostOffice { Name = "Bamokgoko", CityId = 28 },
                new PostOffice { Name = "Belfast", CityId = 28 },
                new PostOffice { Name = "Bethal", CityId = 27 },
                new PostOffice { Name = "Blackhill", CityId = 28 },
                new PostOffice { Name = "Blinkpan", CityId = 28 },
                new PostOffice { Name = "Bosbokrand", CityId = 26 },
                new PostOffice { Name = "Clewer", CityId = 28 },
                new PostOffice { Name = "Commondale", CityId = 27 },
                new PostOffice { Name = "Daggakraal", CityId = 27 },
                new PostOffice { Name = "Davel", CityId = 27 },
                new PostOffice { Name = "Elandshoek", CityId = 26 },
                new PostOffice { Name = "Elukwatini", CityId = 27 },
                new PostOffice { Name = "Emalahleni Central", CityId = 28 },
                new PostOffice { Name = "Emalahleni North", CityId = 28 },
                new PostOffice { Name = "Emgulatshani", CityId = 27 },
                new PostOffice { Name = "EMpumalanga", CityId = 28 },
                new PostOffice { Name = "Ermelo", CityId = 27 },
                new PostOffice { Name = "Evander", CityId = 27 },
                new PostOffice { Name = "Fernie North", CityId = 27 },
                new PostOffice { Name = "Dichoeung", CityId = 28 },
                new PostOffice { Name = "Dirkiesdorp", CityId = 27 },
                new PostOffice { Name = "Dullstroom", CityId = 28 },
                new PostOffice { Name = "Dundonald", CityId = 27 },
                new PostOffice { Name = "Ekulindeni", CityId = 27 },
                new PostOffice { Name = "Gamampane", CityId = 28 },
                new PostOffice { Name = "Ga-Nala", CityId = 28 },
                new PostOffice { Name = "Garakgwadi", CityId = 28 },
                new PostOffice { Name = "Glenmore", CityId = 27 },
                new PostOffice { Name = "Graskop", CityId = 26 },
                new PostOffice { Name = "Greylingstad", CityId = 27 },
                new PostOffice { Name = "Grootvlei", CityId = 27 },
                new PostOffice { Name = "Hazyview", CityId = 26 },
                new PostOffice { Name = "Hectorspruit", CityId = 26 },
                new PostOffice { Name = "Hendrina", CityId = 28 },
                new PostOffice { Name = "Hluvukani", CityId = 26 },
                new PostOffice { Name = "Holmdene", CityId = 27 },
                new PostOffice { Name = "Imbuzini", CityId = 26 },
                new PostOffice { Name = "Intuthuko", CityId = 27 },
                new PostOffice { Name = "Iswepe", CityId = 27 },
                new PostOffice { Name = "Kaapmuiden", CityId = 26 },
                new PostOffice { Name = "Kabokweni", CityId = 26 },
                new PostOffice { Name = "Kanyamazane", CityId = 26 },
                new PostOffice { Name = "Karino", CityId = 26 },
                new PostOffice { Name = "Kayedwa", CityId = 26 },
                new PostOffice { Name = "Kediketse", CityId = 28 },
                new PostOffice { Name = "Kendal", CityId = 28 },
                new PostOffice { Name = "Keteke", CityId = 28 },
                new PostOffice { Name = "Kgautswane", CityId = 26 },
                new PostOffice { Name = "Khokhovela", CityId = 26 },
                new PostOffice { Name = "Kinross", CityId = 27 },
                new PostOffice { Name = "Kranspoort", CityId = 28 },
                new PostOffice { Name = "Kwalugedlane", CityId = 26 },
                new PostOffice { Name = "Kwa-Mhlanga", CityId = 28 },
                new PostOffice { Name = "Kwa-Ngema", CityId = 27 },
                new PostOffice { Name = "Lamagadlela", CityId = 27 },
                new PostOffice { Name = "Lefifi", CityId = 29 },
                new PostOffice { Name = "Leraatsfontein", CityId = 28 },
                new PostOffice { Name = "Leslie", CityId = 27 },
                new PostOffice { Name = "Litjelembube", CityId = 27 },
                new PostOffice { Name = "Lothair", CityId = 27 },
                new PostOffice { Name = "Low's Creek", CityId = 26 },
                new PostOffice { Name = "Lydenburg", CityId = 26 },
                new PostOffice { Name = "Machadodorp", CityId = 28 },
                new PostOffice { Name = "Madlayedwa", CityId = 28 },
                new PostOffice { Name = "Mafemani", CityId = 26 },
                new PostOffice { Name = "Magelembe", CityId = 28 },
                new PostOffice { Name = "Makadikwe", CityId = 28 },
                new PostOffice { Name = "Malelane", CityId = 26 },
                new PostOffice { Name = "Malope", CityId = 28 },
                new PostOffice { Name = "Marble Hall", CityId = 28 },
                new PostOffice { Name = "Marloth Park", CityId = 26 },
                new PostOffice { Name = "Marobogo", CityId = 28 },
                new PostOffice { Name = "Maseke", CityId = 26 },
                new PostOffice { Name = "Masibekela", CityId = 26 },
                new PostOffice { Name = "Maswaneng", CityId = 28 },
                new PostOffice { Name = "Mataffin", CityId = 26 },
                new PostOffice { Name = "Matibidi", CityId = 26 },
                new PostOffice { Name = "Matjhirini", CityId = 28 },
                new PostOffice { Name = "Matsulu", CityId = 26 },
                new PostOffice { Name = "Mazolwandle", CityId = 26 },
                new PostOffice { Name = "Mbangwane", CityId = 26 },
                new PostOffice { Name = "Mbibane", CityId = 28 },
                new PostOffice { Name = "Meerlus", CityId = 28 },
                new PostOffice { Name = "Mhluzi", CityId = 28 },
                new PostOffice { Name = "Middelburg", CityId = 28 },
                new PostOffice { Name = "Mkhuhlu", CityId = 26 },
                new PostOffice { Name = "M'Lapa-kgomo", CityId = 26 },
                new PostOffice { Name = "Moloto", CityId = 28 },
                new PostOffice { Name = "Moolman", CityId = 27 },
                new PostOffice { Name = "Mpharangope", CityId = 28 },
                new PostOffice { Name = "Mpudulle North", CityId = 28 },
                new PostOffice { Name = "Mpuluzi", CityId = 27 },
                new PostOffice { Name = "Msogwaba", CityId = 26 },
                new PostOffice { Name = "Mthambothini", CityId = 28 },
                new PostOffice { Name = "Mutlestad", CityId = 29 },
                new PostOffice { Name = "Nelspruit", CityId = 26 },
                new PostOffice { Name = "Ngodwana", CityId = 26 },
                new PostOffice { Name = "Ngonini", CityId = 27 },
                new PostOffice { Name = "Ngwabe", CityId = 30 },
                new PostOffice { Name = "Noordkaap", CityId = 26 },
                new PostOffice { Name = "Ogies", CityId = 28 },
                new PostOffice { Name = "Oshoek Border Post", CityId = 27 },
                new PostOffice { Name = "Pankop", CityId = 29 },
                new PostOffice { Name = "Patamedi", CityId = 26 },
                new PostOffice { Name = "Penge", CityId = 26 },
                new PostOffice { Name = "Perdekop", CityId = 27 },
                new PostOffice { Name = "Phiva", CityId = 26 },
                new PostOffice { Name = "Piet Retief", CityId = 27 },
                new PostOffice { Name = "Pilgrims Rest", CityId = 26 },
                new PostOffice { Name = "Platrand", CityId = 27 },
                new PostOffice { Name = "Pullen's Hope", CityId = 28 },
                new PostOffice { Name = "Rietkuil", CityId = 28 },
                new PostOffice { Name = "Rietspruit", CityId = 27 },
                new PostOffice { Name = "River Crescent", CityId = 28 },
                new PostOffice { Name = "Robinsdale", CityId = 27 },
                new PostOffice { Name = "Roossenekal", CityId = 28 },
                new PostOffice { Name = "Sabie", CityId = 26 },
                new PostOffice { Name = "Salebona", CityId = 27 },
                new PostOffice { Name = "Schagen", CityId = 26 },
                new PostOffice { Name = "Seabe", CityId = 28 },
                new PostOffice { Name = "Secunda", CityId = 27 },
                new PostOffice { Name = "Sekwati", CityId = 30 },
                new PostOffice { Name = "Shatale", CityId = 26 },
                new PostOffice { Name = "Sheepmoor", CityId = 27 },
                new PostOffice { Name = "Shongwe Mission", CityId = 26 },
                new PostOffice { Name = "Sibuyile", CityId = 26 },
                new PostOffice { Name = "Sidlamafa", CityId = 26 },
                new PostOffice { Name = "SIS North Region", CityId = 31 },
                new PostOffice { Name = "Siyabuswa", CityId = 28 },
                new PostOffice { Name = "Skilpadfontein", CityId = 28 },
                new PostOffice { Name = "Skukuza", CityId = 26 },
                new PostOffice { Name = "Sleutelfontein", CityId = 28 },
                new PostOffice { Name = "Songeni", CityId = 26 },
                new PostOffice { Name = "Standerton", CityId = 27 },
                new PostOffice { Name = "Steiltes", CityId = 26 },
                new PostOffice { Name = "Steyndorp", CityId = 27 },
                new PostOffice { Name = "Stoffberg", CityId = 28 },
                new PostOffice { Name = "Strydmag", CityId = 28 },
                new PostOffice { Name = "Sundra", CityId = 28 },
                new PostOffice { Name = "Tasbet Park", CityId = 28 },
                new PostOffice { Name = "The Brook", CityId = 27 },
                new PostOffice { Name = "Thulamahashe", CityId = 26 },
                new PostOffice { Name = "Tlapeng", CityId = 26 },
                new PostOffice { Name = "Trichardt", CityId = 27 },
                new PostOffice { Name = "Tsimanyane", CityId = 28 },
                new PostOffice { Name = "Val", CityId = 27 },
                new PostOffice { Name = "Vandyksdrif", CityId = 27 },
                new PostOffice { Name = "Verena", CityId = 28 },
                new PostOffice { Name = "Volksrust", CityId = 27 },
                new PostOffice { Name = "Voltargo", CityId = 28 },
                new PostOffice { Name = "Voorreg", CityId = 28 },
                new PostOffice { Name = "Wakkerstroom", CityId = 27 },
                new PostOffice { Name = "Warburton", CityId = 27 },
                new PostOffice { Name = "Waterval-Boven", CityId = 28 },
                new PostOffice { Name = "Weverley", CityId = 27 },
                new PostOffice { Name = "Witrivier", CityId = 26 },
                new PostOffice { Name = "Wonderfontein", CityId = 26 },
                new PostOffice { Name = "Ximhungwe", CityId = 26 },
                new PostOffice { Name = "Breyten", CityId = 27 },
                new PostOffice { Name = "Buthi", CityId = 28 },
                new PostOffice { Name = "Carolina", CityId = 27 },
                new PostOffice { Name = "Casteel", CityId = 26 },
                new PostOffice { Name = "Charl Cilliers", CityId = 27 },
                new PostOffice { Name = "Chrissiesmeer", CityId = 27 },
                new PostOffice { Name = "Albemarle", CityId = 32 },
                new PostOffice { Name = "Alberton", CityId = 32 },
                new PostOffice { Name = "Alexandra", CityId = 33 },
                new PostOffice { Name = "Alrode", CityId = 32 },
                new PostOffice { Name = "Arcon Park", CityId = 34 },
                new PostOffice { Name = "Atteridgeville", CityId = 35 },
                new PostOffice { Name = "Azaadville", CityId = 36 },
                new PostOffice { Name = "Bapsfontein", CityId = 37 },
                new PostOffice { Name = "Bedfordview", CityId = 38 },
                new PostOffice { Name = "Bedworth Park", CityId = 34 },
                new PostOffice { Name = "Benmore", CityId = 33 },
                new PostOffice { Name = "Benoni", CityId = 37 },
                new PostOffice { Name = "Birchleigh", CityId = 39 },
                new PostOffice { Name = "Bluegum View", CityId = 40 },
                new PostOffice { Name = "Boksburg", CityId = 37 },
                new PostOffice { Name = "Boksburg West", CityId = 37 },
                new PostOffice { Name = "Bonaero Park", CityId = 39 },
                new PostOffice { Name = "Booysens", CityId = 38 },
                new PostOffice { Name = "Braamfontein", CityId = 38 },
                new PostOffice { Name = "Bracken Gardens", CityId = 32 },
                new PostOffice { Name = "Brackendowns", CityId = 32 },
                new PostOffice { Name = "Cinda Park", CityId = 37 },
                new PostOffice { Name = "Cosmo City", CityId = 33 },
                new PostOffice { Name = "Craighall", CityId = 33 },
                new PostOffice { Name = "Cresta", CityId = 33 },
                new PostOffice { Name = "Crown Mines", CityId = 38 },
                new PostOffice { Name = "Crystal Park", CityId = 37 },
                new PostOffice { Name = "Cullinan", CityId = 41 },
                new PostOffice { Name = "Danville", CityId = 35 },
                new PostOffice { Name = "Daveyton East", CityId = 37 },
                new PostOffice { Name = "Dersley", CityId = 40 },
                new PostOffice { Name = "Eldoradopark", CityId = 42 },
                new PostOffice { Name = "Elsburg", CityId = 43 },
                new PostOffice { Name = "Evaton", CityId = 34 },
                new PostOffice { Name = "Field", CityId = 35 },
                new PostOffice { Name = "Florida", CityId = 36 },
                new PostOffice { Name = "Fordsburg", CityId = 38 },
                new PostOffice { Name = "Devon", CityId = 40 },
                new PostOffice { Name = "Diepkloof", CityId = 42 },
                new PostOffice { Name = "Dunswart", CityId = 37 },
                new PostOffice { Name = "Eco Boulevard", CityId = 35 },
                new PostOffice { Name = "Edenvale", CityId = 39 },
                new PostOffice { Name = "Ekangala", CityId = 41 },
                new PostOffice { Name = "Ga-Rankuwa", CityId = 44 },
                new PostOffice { Name = "Garsfontein", CityId = 41 },
                new PostOffice { Name = "Germiston South", CityId = 43 },
                new PostOffice { Name = "Greenside", CityId = 38 },
                new PostOffice { Name = "Greenstone", CityId = 39 },
                new PostOffice { Name = "Halfway House", CityId = 39 },
                new PostOffice { Name = "Hebron", CityId = 44 },
                new PostOffice { Name = "Helderkruin", CityId = 36 },
                new PostOffice { Name = "Henley On Klip", CityId = 34 },
                new PostOffice { Name = "Hercules", CityId = 45 },
                new PostOffice { Name = "Highlands North", CityId = 33 },
                new PostOffice { Name = "Honeydew", CityId = 36 },
                new PostOffice { Name = "Industria", CityId = 38 },
                new PostOffice { Name = "Internal Audit", CityId = 46 },
                new PostOffice { Name = "Isando", CityId = 39 },
                new PostOffice { Name = "IT SD SM Service Desk and Incident Mgt", CityId = 45 },
                new PostOffice { Name = "Jatniel", CityId = 37 },
                new PostOffice { Name = "Jeppestown", CityId = 38 },
                new PostOffice { Name = "JIMC Customs", CityId = 39 },
                new PostOffice { Name = "Kelvin", CityId = 33 },
                new PostOffice { Name = "Kibler Park", CityId = 38 },
                new PostOffice { Name = "Kocksvlei", CityId = 47 },
                new PostOffice { Name = "Krugersdorp", CityId = 47 },
                new PostOffice { Name = "Kwathema", CityId = 40 },
                new PostOffice { Name = "Kwenzekile", CityId = 32 },
                new PostOffice { Name = "Langlaagte", CityId = 38 },
                new PostOffice { Name = "Lanseria", CityId = 47 },
                new PostOffice { Name = "Laudium", CityId = 35 },
                new PostOffice { Name = "Lebanon", CityId = 44 },
                new PostOffice { Name = "Lenasia", CityId = 42 },
                new PostOffice { Name = "Lonehill", CityId = 33 },
                new PostOffice { Name = "Lotus Gardens", CityId = 35 },
                new PostOffice { Name = "Lynn East", CityId = 41 },
                new PostOffice { Name = "Lyttelton", CityId = 35 },
                new PostOffice { Name = "Mabopane", CityId = 44 },
                new PostOffice { Name = "Mafatsana", CityId = 34 },
                new PostOffice { Name = "Majaneng", CityId = 44 },
                new PostOffice { Name = "Mamelodi", CityId = 41 },
                new PostOffice { Name = "Maraisburg", CityId = 36 },
                new PostOffice { Name = "Marshalltown", CityId = 38 },
                new PostOffice { Name = "Meadowlands", CityId = 42 },
                new PostOffice { Name = "Medunsa", CityId = 44 },
                new PostOffice { Name = "Menlo Park", CityId = 45 },
                new PostOffice { Name = "Menlyn", CityId = 41 },
                new PostOffice { Name = "Meyerton", CityId = 34 },
                new PostOffice { Name = "Mondeor", CityId = 38 },
                new PostOffice { Name = "Morula", CityId = 44 },
                new PostOffice { Name = "Nigel", CityId = 40 },
                new PostOffice { Name = "NITS Corporate", CityId = 48 },
                new PostOffice { Name = "NITS Northern Region", CityId = 49 },
                new PostOffice { Name = "NITS Wits Region", CityId = 50 },
                new PostOffice { Name = "North Riding", CityId = 33 },
                new PostOffice { Name = "Northlands", CityId = 33 },
                new PostOffice { Name = "Northmead", CityId = 37 },
                new PostOffice { Name = "NPC Head Office", CityId = 51 },
                new PostOffice { Name = "Olifantsfontein", CityId = 39 },
                new PostOffice { Name = "Olivenhoutbosch", CityId = 35 },
                new PostOffice { Name = "OR Tambo International Airport", CityId = 39 },
                new PostOffice { Name = "Orange Grove", CityId = 33 },
                new PostOffice { Name = "Orlando", CityId = 42 },
                new PostOffice { Name = "Park South", CityId = 34 },
                new PostOffice { Name = "Parkview", CityId = 38 },
                new PostOffice { Name = "Pimville", CityId = 42 },
                new PostOffice { Name = "Pinegowrie", CityId = 33 },
                new PostOffice { Name = "Pretoria", CityId = 45 },
                new PostOffice { Name = "Pretoria North", CityId = 45 },
                new PostOffice { Name = "Primrose", CityId = 43 },
                new PostOffice { Name = "Protea Glen", CityId = 42 },
                new PostOffice { Name = "Protea North", CityId = 42 },
                new PostOffice { Name = "Pyramid", CityId = 45 },
                new PostOffice { Name = "Randburg", CityId = 33 },
                new PostOffice { Name = "Randfontein", CityId = 36 },
                new PostOffice { Name = "Rayton", CityId = 41 },
                new PostOffice { Name = "Refentse West", CityId = 44 },
                new PostOffice { Name = "Rensburg", CityId = 40 },
                new PostOffice { Name = "Rethabile", CityId = 41 },
                new PostOffice { Name = "Roodepoort", CityId = 36 },
                new PostOffice { Name = "Rosettenville", CityId = 38 },
                new PostOffice { Name = "Rosslyn", CityId = 44 },
                new PostOffice { Name = "Saulsville", CityId = 35 },
                new PostOffice { Name = "Sebokeng", CityId = 34 },
                new PostOffice { Name = "Selcourt", CityId = 40 },
                new PostOffice { Name = "Sharon Park", CityId = 40 },
                new PostOffice { Name = "Sharpeville", CityId = 34 },
                new PostOffice { Name = "SIS Forensic Investigations", CityId = 52 },
                new PostOffice { Name = "SIS Gauteng Region", CityId = 53 },
                new PostOffice { Name = "Social Grants", CityId = 44 },
                new PostOffice { Name = "Soshanguve", CityId = 44 },
                new PostOffice { Name = "Soshanguve Central", CityId = 44 },
                new PostOffice { Name = "South Gate", CityId = 38 },
                new PostOffice { Name = "South Hills", CityId = 38 },
                new PostOffice { Name = "Sunninghill West", CityId = 33 },
                new PostOffice { Name = "Sunnyside", CityId = 45 },
                new PostOffice { Name = "Tarlton", CityId = 47 },
                new PostOffice { Name = "Thaba Tshwane", CityId = 35 },
                new PostOffice { Name = "The Reeds", CityId = 35 },
                new PostOffice { Name = "Thorn Tree", CityId = 44 },
                new PostOffice { Name = "Toekomsrus", CityId = 36 },
                new PostOffice { Name = "Trade Route", CityId = 42 },
                new PostOffice { Name = "Tsakane", CityId = 40 },
                new PostOffice { Name = "Tshiawelo", CityId = 42 },
                new PostOffice { Name = "Tshwane South", CityId = 35 },
                new PostOffice { Name = "Unisarand", CityId = 35 },
                new PostOffice { Name = "Valhalla", CityId = 35 },
                new PostOffice { Name = "Vanderbijlpark", CityId = 34 },
                new PostOffice { Name = "Vosloorus", CityId = 32 },
                new PostOffice { Name = "Wadeville", CityId = 43 },
                new PostOffice { Name = "Wes-Krugersdorp", CityId = 47 },
                new PostOffice { Name = "Westgate", CityId = 36 },
                new PostOffice { Name = "Westonaria", CityId = 36 },
                new PostOffice { Name = "Wierdapark", CityId = 35 },
                new PostOffice { Name = "Willow Crossing", CityId = 41 },
                new PostOffice { Name = "Winterveld", CityId = 44 },
                new PostOffice { Name = "Wits", CityId = 38 },
                new PostOffice { Name = "Wits:Quality & Oversight", CityId = 55 },
                new PostOffice { Name = "Witspos", CityId = 42 },
                new PostOffice { Name = "Wonderboom Poort", CityId = 45 },
                new PostOffice { Name = "Brenthurst", CityId = 40 },
                new PostOffice { Name = "Brixton", CityId = 38 },
                new PostOffice { Name = "Bronkhorstspruit", CityId = 41 },
                new PostOffice { Name = "Bruma", CityId = 38 },
                new PostOffice { Name = "Bryanston", CityId = 33 },
                new PostOffice { Name = "Carletonville", CityId = 47 },
                new PostOffice { Name = "Central City", CityId = 44 },
                new PostOffice { Name = "Alabama", CityId = 55 },
                new PostOffice { Name = "Amalia", CityId = 56 },
                new PostOffice { Name = "Atamelang", CityId = 57 },
                new PostOffice { Name = "Bakerville", CityId = 57 },
                new PostOffice { Name = "Baleema", CityId = 58 },
                new PostOffice { Name = "Bamare-A-Phogole", CityId = 58 },
                new PostOffice { Name = "Bapo ll", CityId = 57 },
                new PostOffice { Name = "Baratheo", CityId = 58 },
                new PostOffice { Name = "Basebo", CityId = 58 },
                new PostOffice { Name = "Bataung", CityId = 58 },
                new PostOffice { Name = "Batho-Batho", CityId = 57 },
                new PostOffice { Name = "Bedwang", CityId = 59 },
                new PostOffice { Name = "Beestekraal", CityId = 58 },
                new PostOffice { Name = "Bethanie", CityId = 58 },
                new PostOffice { Name = "Biesiesvlei Central", CityId = 57 },
                new PostOffice { Name = "Bloemhof", CityId = 56 },
                new PostOffice { Name = "Bosplaas", CityId = 59 },
                new PostOffice { Name = "Braklaagte", CityId = 57 },
                new PostOffice { Name = "Coligny", CityId = 57 },
                new PostOffice { Name = "Delareyville", CityId = 57 },
                new PostOffice { Name = "Derby", CityId = 57 },
                new PostOffice { Name = "Erasmus", CityId = 60 },
                new PostOffice { Name = "Flamwood", CityId = 55 },
                new PostOffice { Name = "Fochville", CityId = 61 },
                new PostOffice { Name = "Dinokana", CityId = 57 },
                new PostOffice { Name = "Disaneng", CityId = 57 },
                new PostOffice { Name = "Dominionville", CityId = 55 },
                new PostOffice { Name = "Ga-Habedi", CityId = 59 },
                new PostOffice { Name = "Ganyesa", CityId = 56 },
                new PostOffice { Name = "Gasefanyetso", CityId = 58 },
                new PostOffice { Name = "Gerdau", CityId = 57 },
                new PostOffice { Name = "Gopane", CityId = 57 },
                new PostOffice { Name = "Groot-Marico", CityId = 57 },
                new PostOffice { Name = "Hartbeesfontein", CityId = 55 },
                new PostOffice { Name = "Hartbeespoort", CityId = 58 },
                new PostOffice { Name = "Heunaar", CityId = 56 },
                new PostOffice { Name = "Huhudi", CityId = 56 },
                new PostOffice { Name = "Ikageng", CityId = 55 },
                new PostOffice { Name = "Itsoseng", CityId = 57 },
                new PostOffice { Name = "Kalafi", CityId = 58 },
                new PostOffice { Name = "Kameel", CityId = 56 },
                new PostOffice { Name = "Kameelboom", CityId = 58 },
                new PostOffice { Name = "Kanana East", CityId = 55 },
                new PostOffice { Name = "Kayakulu", CityId = 58 },
                new PostOffice { Name = "Khuma", CityId = 55 },
                new PostOffice { Name = "Koster", CityId = 58 },
                new PostOffice { Name = "Kraaipan", CityId = 57 },
                new PostOffice { Name = "Kraalhoek", CityId = 57 },
                new PostOffice { Name = "Kunana", CityId = 57 },
                new PostOffice { Name = "Lebotloane", CityId = 59 },
                new PostOffice { Name = "Leeudoringstad", CityId = 55 },
                new PostOffice { Name = "Lerato", CityId = 57 },
                new PostOffice { Name = "Lesetlheng", CityId = 57 },
                new PostOffice { Name = "Letlhabile", CityId = 58 },
                new PostOffice { Name = "Lichtenburg", CityId = 57 },
                new PostOffice { Name = "Logageng", CityId = 57 },
                new PostOffice { Name = "Louwna", CityId = 56 },
                new PostOffice { Name = "Luka", CityId = 58 },
                new PostOffice { Name = "Maanhaarrand", CityId = 58 },
                new PostOffice { Name = "Mabaalstad", CityId = 58 },
                new PostOffice { Name = "Mabeskraal", CityId = 58 },
                new PostOffice { Name = "Maboloka West", CityId = 58 },
                new PostOffice { Name = "Madibogo", CityId = 57 },
                new PostOffice { Name = "Madidi", CityId = 60 },
                new PostOffice { Name = "Madikwe", CityId = 58 },
                new PostOffice { Name = "Mafikeng", CityId = 57 },
                new PostOffice { Name = "Makapaanstad", CityId = 58 },
                new PostOffice { Name = "Makgobistad", CityId = 57 },
                new PostOffice { Name = "Makwassie", CityId = 55 },
                new PostOffice { Name = "Manamela", CityId = 58 },
                new PostOffice { Name = "Mantserre", CityId = 58 },
                new PostOffice { Name = "Marapallo", CityId = 58 },
                new PostOffice { Name = "Mareetsane", CityId = 57 },
                new PostOffice { Name = "Miga", CityId = 57 },
                new PostOffice { Name = "Mmabatho", CityId = 57 },
                new PostOffice { Name = "Mmakau", CityId = 60 },
                new PostOffice { Name = "Mmasebodule", CityId = 57 },
                new PostOffice { Name = "Modimosana", CityId = 58 },
                new PostOffice { Name = "Module", CityId = 58 },
                new PostOffice { Name = "Moedwil", CityId = 58 },
                new PostOffice { Name = "Mogoditshane", CityId = 57 },
                new PostOffice { Name = "Mogono", CityId = 58 },
                new PostOffice { Name = "Mogosane", CityId = 57 },
                new PostOffice { Name = "Mokgalwana", CityId = 57 },
                new PostOffice { Name = "Mokgaotsistad", CityId = 58 },
                new PostOffice { Name = "Mokgatlha", CityId = 58 },
                new PostOffice { Name = "Molatedi", CityId = 58 },
                new PostOffice { Name = "Molorwe", CityId = 58 },
                new PostOffice { Name = "Montshiwa", CityId = 57 },
                new PostOffice { Name = "Mooinooi", CityId = 58 },
                new PostOffice { Name = "Morokweng", CityId = 56 },
                new PostOffice { Name = "Mothotlung", CityId = 58 },
                new PostOffice { Name = "Motlhabe North", CityId = 58 },
                new PostOffice { Name = "Motswedi", CityId = 57 },
                new PostOffice { Name = "Mphe-Batho", CityId = 59 },
                new PostOffice { Name = "Nietverdiend", CityId = 57 },
                new PostOffice { Name = "Orkney", CityId = 55 },
                new PostOffice { Name = "Ottosdal", CityId = 57 },
                new PostOffice { Name = "Ottoshoop", CityId = 57 },
                new PostOffice { Name = "Phalane", CityId = 58 },
                new PostOffice { Name = "Photsaneng", CityId = 58 },
                new PostOffice { Name = "Piet Plessies", CityId = 56 },
                new PostOffice { Name = "Potchefstroom", CityId = 55 },
                new PostOffice { Name = "Promosa", CityId = 55 },
                new PostOffice { Name = "Puana", CityId = 57 },
                new PostOffice { Name = "Radithuso", CityId = 57 },
                new PostOffice { Name = "Ramatlabama", CityId = 57 },
                new PostOffice { Name = "Rankunyana", CityId = 58 },
                new PostOffice { Name = "Rantebeng", CityId = 59 },
                new PostOffice { Name = "Rantjiepan", CityId = 59 },
                new PostOffice { Name = "Rashoop", CityId = 58 },
                new PostOffice { Name = "Ratshidi", CityId = 57 },
                new PostOffice { Name = "Reivilo", CityId = 56 },
                new PostOffice { Name = "Rustenburg", CityId = 58 },
                new PostOffice { Name = "Sannieshof", CityId = 57 },
                new PostOffice { Name = "Schweizer-Reneke", CityId = 56 },
                new PostOffice { Name = "Sekama", CityId = 57 },
                new PostOffice { Name = "Silwerkrans", CityId = 58 },
                new PostOffice { Name = "Siyabonga", CityId = 58 },
                new PostOffice { Name = "Skeerpoort", CityId = 58 },
                new PostOffice { Name = "Skuinsdrift", CityId = 57 },
                new PostOffice { Name = "Sonop", CityId = 58 },
                new PostOffice { Name = "Southey", CityId = 56 },
                new PostOffice { Name = "Stella", CityId = 56 },
                new PostOffice { Name = "Stilfontein", CityId = 55 },
                new PostOffice { Name = "Supingstad", CityId = 57 },
                new PostOffice { Name = "Swartruggens", CityId = 58 },
                new PostOffice { Name = "Syferbult", CityId = 57 },
                new PostOffice { Name = "Tampostad", CityId = 58 },
                new PostOffice { Name = "Taung", CityId = 56 },
                new PostOffice { Name = "Taung Station", CityId = 56 },
                new PostOffice { Name = "Temba", CityId = 60 },
                new PostOffice { Name = "Thulwe", CityId = 59 },
                new PostOffice { Name = "Tlhabane", CityId = 58 },
                new PostOffice { Name = "Tlhakgameng", CityId = 56 },
                new PostOffice { Name = "Toemaskop", CityId = 57 },
                new PostOffice { Name = "Tosca", CityId = 56 },
                new PostOffice { Name = "Tseoge", CityId = 62 },
                new PostOffice { Name = "Tsetse", CityId = 57 },
                new PostOffice { Name = "Tshidila Molomo", CityId = 57 },
                new PostOffice { Name = "Tsitsing", CityId = 58 },
                new PostOffice { Name = "Vaal Reef", CityId = 55 },
                new PostOffice { Name = "Ventersdorp", CityId = 55 },
                new PostOffice { Name = "Vermaas", CityId = 57 },
                new PostOffice { Name = "Vorentoe", CityId = 58 },
                new PostOffice { Name = "Vorstershoop", CityId = 56 },
                new PostOffice { Name = "Vryburg", CityId = 56 },
                new PostOffice { Name = "Wolmaransstad", CityId = 55 },
                new PostOffice { Name = "Wonderkop", CityId = 58 },
                new PostOffice { Name = "Ya-Rona", CityId = 58 },
                new PostOffice { Name = "Zeerust", CityId = 57 },
                new PostOffice { Name = "Zinniaville", CityId = 58 },
                new PostOffice { Name = "Bray", CityId = 56 },
                new PostOffice { Name = "Brits", CityId = 58 },
                new PostOffice { Name = "Chaneng", CityId = 58 },
                new PostOffice { Name = "Amanzimtoti", CityId = 63 },
                new PostOffice { Name = "Ashwood", CityId = 64 },
                new PostOffice { Name = "Austerville", CityId = 63 },
                new PostOffice { Name = "Baynesfield", CityId = 65 },
                new PostOffice { Name = "Bergville", CityId = 66 },
                new PostOffice { Name = "Braemar", CityId = 67 },
                new PostOffice { Name = "Copesville", CityId = 65 },
                new PostOffice { Name = "Cramond", CityId = 65 },
                new PostOffice { Name = "Dalton", CityId = 65 },
                new PostOffice { Name = "Dannhauser", CityId = 68 },
                new PostOffice { Name = "Dargle", CityId = 65 },
                new PostOffice { Name = "Deemount", CityId = 67 },
                new PostOffice { Name = "Elandskraal", CityId = 66 },
                new PostOffice { Name = "Empangeni Station", CityId = 69 },
                new PostOffice { Name = "Eshowe", CityId = 69 },
                new PostOffice { Name = "Estcourt", CityId = 66 },
                new PostOffice { Name = "Ezakheni", CityId = 66 },
                new PostOffice { Name = "Ezimpisini", CityId = 70 },
                new PostOffice { Name = "Franklin", CityId = 66 },
                new PostOffice { Name = "Dormerton", CityId = 71 },
                new PostOffice { Name = "Dukuza", CityId = 66 },
                new PostOffice { Name = "Dundee", CityId = 68 },
                new PostOffice { Name = "Durban", CityId = 71 },
                new PostOffice { Name = "Durban North", CityId = 71 },
                new PostOffice { Name = "Durmail", CityId = 72 },
                new PostOffice { Name = "Durmail Parcel Counter", CityId = 72 },
                new PostOffice { Name = "Durnacol", CityId = 68 },
                new PostOffice { Name = "Dweshula", CityId = 67 },
                new PostOffice { Name = "Egagasini", CityId = 70 },
                new PostOffice { Name = "Ekuvukeni", CityId = 66 },
                new PostOffice { Name = "Gamalakhe", CityId = 67 },
                new PostOffice { Name = "Gingindlovu", CityId = 69 },
                new PostOffice { Name = "Glenashley", CityId = 71 },
                new PostOffice { Name = "Glencoe", CityId = 68 },
                new PostOffice { Name = "Glenside", CityId = 65 },
                new PostOffice { Name = "Greytown", CityId = 68 },
                new PostOffice { Name = "Hammarsdale", CityId = 64 },
                new PostOffice { Name = "Harding", CityId = 67 },
                new PostOffice { Name = "Hattingspruit", CityId = 68 },
                new PostOffice { Name = "Hermannsburg", CityId = 68 },
                new PostOffice { Name = "Hillcrest", CityId = 64 },
                new PostOffice { Name = "Hlabisa", CityId = 70 },
                new PostOffice { Name = "Hluhluwe", CityId = 70 },
                new PostOffice { Name = "Howick", CityId = 65 },
                new PostOffice { Name = "Ingogo", CityId = 68 },
                new PostOffice { Name = "Ingwavuma", CityId = 70 },
                new PostOffice { Name = "Isipingo Beach", CityId = 63 },
                new PostOffice { Name = "Ixopo", CityId = 66 },
                new PostOffice { Name = "Izingolweni", CityId = 67 },
                new PostOffice { Name = "Jacobs", CityId = 63 },
                new PostOffice { Name = "Jozini", CityId = 70 },
                new PostOffice { Name = "Khan Road", CityId = 65 },
                new PostOffice { Name = "Kloof", CityId = 64 },
                new PostOffice { Name = "Kokstad", CityId = 67 },
                new PostOffice { Name = "Kranskop", CityId = 68 },
                new PostOffice { Name = "Kwadlangezwa", CityId = 69 },
                new PostOffice { Name = "Kwa-Makhasa", CityId = 70 },
                new PostOffice { Name = "Kwa-Ndengezi", CityId = 64 },
                new PostOffice { Name = "Kwa-Ngcolosi", CityId = 64 },
                new PostOffice { Name = "Kwangwanase", CityId = 70 },
                new PostOffice { Name = "Kwa-Nxamalala", CityId = 69 },
                new PostOffice { Name = "Kwaximba", CityId = 64 },
                new PostOffice { Name = "Ladysmith", CityId = 66 },
                new PostOffice { Name = "Lamontville", CityId = 63 },
                new PostOffice { Name = "Lidgetton", CityId = 65 },
                new PostOffice { Name = "Lions River", CityId = 65 },
                new PostOffice { Name = "Luneburg", CityId = 68 },
                new PostOffice { Name = "Madadeni", CityId = 68 },
                new PostOffice { Name = "Mahlabatini", CityId = 70 },
                new PostOffice { Name = "Mandeni", CityId = 69 },
                new PostOffice { Name = "Mapumulo", CityId = 69 },
                new PostOffice { Name = "Marble Ray", CityId = 71 },
                new PostOffice { Name = "Marburg", CityId = 67 },
                new PostOffice { Name = "Margate", CityId = 67 },
                new PostOffice { Name = "Margate Retirement Village", CityId = 67 },
                new PostOffice { Name = "Mayorswalk", CityId = 65 },
                new PostOffice { Name = "Mayville", CityId = 64 },
                new PostOffice { Name = "Mbazwana", CityId = 70 },
                new PostOffice { Name = "Meer En See", CityId = 69 },
                new PostOffice { Name = "Mehlomnyama", CityId = 67 },
                new PostOffice { Name = "Melmoth", CityId = 69 },
                new PostOffice { Name = "Mfolozi", CityId = 70 },
                new PostOffice { Name = "Mid Illovo", CityId = 66 },
                new PostOffice { Name = "Mkuze", CityId = 70 },
                new PostOffice { Name = "Mobeni", CityId = 63 },
                new PostOffice { Name = "Montclair", CityId = 63 },
                new PostOffice { Name = "Mooirivier", CityId = 66 },
                new PostOffice { Name = "Mtubatuba", CityId = 70 },
                new PostOffice { Name = "Mtunzini", CityId = 69 },
                new PostOffice { Name = "Muden", CityId = 68 },
                new PostOffice { Name = "Munster", CityId = 67 },
                new PostOffice { Name = "Ndumo", CityId = 70 },
                new PostOffice { Name = "Newcastle", CityId = 68 },
                new PostOffice { Name = "Nhlazatshe", CityId = 68 },
                new PostOffice { Name = "NITS Kwazulu Natal Region", CityId = 73 },
                new PostOffice { Name = "Nkandla", CityId = 69 },
                new PostOffice { Name = "Nongoma", CityId = 70 },
                new PostOffice { Name = "Nottingham Road", CityId = 65 },
                new PostOffice { Name = "Nqabeni", CityId = 66 },
                new PostOffice { Name = "Nqutu", CityId = 68 },
                new PostOffice { Name = "Ntokozweni", CityId = 63 },
                new PostOffice { Name = "Osizweni", CityId = 68 },
                new PostOffice { Name = "Overport", CityId = 71 },
                new PostOffice { Name = "Paddock", CityId = 67 },
                new PostOffice { Name = "Phoenix", CityId = 71 },
                new PostOffice { Name = "Phungashe", CityId = 67 },
                new PostOffice { Name = "Pietermaritzburg", CityId = 65 },
                new PostOffice { Name = "Pinetown", CityId = 64 },
                new PostOffice { Name = "Point", CityId = 71 },
                new PostOffice { Name = "Pomeroy", CityId = 68 },
                new PostOffice { Name = "Pongola", CityId = 70 },
                new PostOffice { Name = "Port Shepstone", CityId = 67 },
                new PostOffice { Name = "Red Hill", CityId = 71 },
                new PostOffice { Name = "Renishaw", CityId = 67 },
                new PostOffice { Name = "Reservoir Hills", CityId = 64 },
                new PostOffice { Name = "Rorke's Drift", CityId = 68 },
                new PostOffice { Name = "Rosetta", CityId = 66 },
                new PostOffice { Name = "Rossburgh", CityId = 63 },
                new PostOffice { Name = "Scottburgh", CityId = 67 },
                new PostOffice { Name = "Sea Park", CityId = 67 },
                new PostOffice { Name = "Sezela", CityId = 67 },
                new PostOffice { Name = "Sibhayi", CityId = 70 },
                new PostOffice { Name = "SIS KwaZulu Region", CityId = 74 },
                new PostOffice { Name = "St Augustine's", CityId = 66 },
                new PostOffice { Name = "St Lucia Estuary", CityId = 70 },
                new PostOffice { Name = "St Wendolins", CityId = 64 },
                new PostOffice { Name = "Stanger", CityId = 69 },
                new PostOffice { Name = "Swartberg", CityId = 66 },
                new PostOffice { Name = "Swart-Mfolozi", CityId = 70 },
                new PostOffice { Name = "Tongaat", CityId = 71 },
                new PostOffice { Name = "Tugela", CityId = 68 },
                new PostOffice { Name = "Tweedie", CityId = 65 },
                new PostOffice { Name = "Ubombo", CityId = 70 },
                new PostOffice { Name = "Ulundi", CityId = 70 },
                new PostOffice { Name = "Umbumbulu", CityId = 63 },
                new PostOffice { Name = "Umhlali", CityId = 69 },
                new PostOffice { Name = "Umhlanga Rocks", CityId = 71 },
                new PostOffice { Name = "Umkomaas", CityId = 63 },
                new PostOffice { Name = "Umlaas Road", CityId = 67 },
                new PostOffice { Name = "Umzimkulu", CityId = 67 },
                new PostOffice { Name = "Utrecht", CityId = 68 },
                new PostOffice { Name = "Vryheid", CityId = 68 },
                new PostOffice { Name = "Wasbank", CityId = 68 },
                new PostOffice { Name = "Watersmeet", CityId = 66 },
                new PostOffice { Name = "Weenen", CityId = 66 },
                new PostOffice { Name = "Westville", CityId = 64 },
                new PostOffice { Name = "Winterton", CityId = 66 },
                new PostOffice { Name = "Yellowwood Park", CityId = 63 },
                new PostOffice { Name = "Zimbali", CityId = 69 },
                new PostOffice { Name = "Buchanana", CityId = 69 },
                new PostOffice { Name = "Buxedeni", CityId = 70 },
                new PostOffice { Name = "Camperdown", CityId = 65 },
                new PostOffice { Name = "Cato Ridge", CityId = 64 },
                new PostOffice { Name = "Ceza", CityId = 70 },
                new PostOffice { Name = "Chatsworth", CityId = 63 },
                new PostOffice { Name = "Allanridge", CityId = 75 },
                new PostOffice { Name = "Arlington", CityId = 76 },
                new PostOffice { Name = "Bain'S Vlei", CityId = 77 },
                new PostOffice { Name = "Bethlehem", CityId = 76 },
                new PostOffice { Name = "Bethulie", CityId = 78 },
                new PostOffice { Name = "Bloemfontein", CityId = 77 },
                new PostOffice { Name = "Boshof", CityId = 78 },
                new PostOffice { Name = "Bothaville", CityId = 75 },
                new PostOffice { Name = "Brandfort", CityId = 75 },
                new PostOffice { Name = "Brandhof", CityId = 77 },
                new PostOffice { Name = "Clocolan", CityId = 76 },
                new PostOffice { Name = "Cornelia", CityId = 76 },
                new PostOffice { Name = "Deneysville", CityId = 79 },
                new PostOffice { Name = "Excelsior", CityId = 78 },
                new PostOffice { Name = "Fauresmith", CityId = 78 },
                new PostOffice { Name = "Fichardt Park", CityId = 77 },
                new PostOffice { Name = "Ficksburg", CityId = 76 },
                new PostOffice { Name = "Fouriesburg", CityId = 76 },
                new PostOffice { Name = "Frankfort", CityId = 79 },
                new PostOffice { Name = "Dewetsdorp", CityId = 78 },
                new PostOffice { Name = "Dikgakeng", CityId = 76 },
                new PostOffice { Name = "Edenville", CityId = 79 },
                new PostOffice { Name = "Eerstemyn", CityId = 75 },
                new PostOffice { Name = "Ga-Sehunelo", CityId = 77 },
                new PostOffice { Name = "Gladstone", CityId = 77 },
                new PostOffice { Name = "Harrismith", CityId = 76 },
                new PostOffice { Name = "Heidedal", CityId = 77 },
                new PostOffice { Name = "Heilbron", CityId = 79 },
                new PostOffice { Name = "Hennenman", CityId = 75 },
                new PostOffice { Name = "Hertzogville", CityId = 78 },
                new PostOffice { Name = "Heuwelsig", CityId = 77 },
                new PostOffice { Name = "Hobhouse", CityId = 78 },
                new PostOffice { Name = "Hoopstad", CityId = 75 },
                new PostOffice { Name = "Intabazwe", CityId = 76 },
                new PostOffice { Name = "Jacobsdal", CityId = 78 },
                new PostOffice { Name = "Jagersfontein", CityId = 78 },
                new PostOffice { Name = "Kagisanong", CityId = 77 },
                new PostOffice { Name = "Kestell", CityId = 76 },
                new PostOffice { Name = "Koffiefontein", CityId = 78 },
                new PostOffice { Name = "Koppies", CityId = 79 },
                new PostOffice { Name = "Kroonstad", CityId = 79 },
                new PostOffice { Name = "Ladybrand", CityId = 78 },
                new PostOffice { Name = "Langenhovenpark", CityId = 77 },
                new PostOffice { Name = "Lindley", CityId = 76 },
                new PostOffice { Name = "Luckhoff", CityId = 78 },
                new PostOffice { Name = "Mafube", CityId = 77 },
                new PostOffice { Name = "Mangaung", CityId = 77 },
                new PostOffice { Name = "Marobeng", CityId = 77 },
                new PostOffice { Name = "Marquard", CityId = 76 },
                new PostOffice { Name = "Meloding", CityId = 75 },
                new PostOffice { Name = "Memel", CityId = 76 },
                new PostOffice { Name = "Merafong", CityId = 75 },
                new PostOffice { Name = "Mokodumela", CityId = 76 },
                new PostOffice { Name = "Motsethabong", CityId = 75 },
                new PostOffice { Name = "NITS Central Region", CityId = 80 },
                new PostOffice { Name = "Oppermansgrond", CityId = 78 },
                new PostOffice { Name = "Oranjeville", CityId = 79 },
                new PostOffice { Name = "Pansig", CityId = 75 },
                new PostOffice { Name = "Parys", CityId = 79 },
                new PostOffice { Name = "Petrus Steyn", CityId = 76 },
                new PostOffice { Name = "Petrusburg", CityId = 78 },
                new PostOffice { Name = "Philippolis", CityId = 78 },
                new PostOffice { Name = "Phiritona", CityId = 79 },
                new PostOffice { Name = "Phuthaditjhaba", CityId = 76 },
                new PostOffice { Name = "Postbank Account Administration", CityId = 77 },
                new PostOffice { Name = "Postbank Bank Mail Room", CityId = 77 },
                new PostOffice { Name = "Postbank Banking and Balancing SASSA", CityId = 77 },
                new PostOffice { Name = "Postbank Call Centre", CityId = 77 },
                new PostOffice { Name = "Postbank Card Management", CityId = 77 },
                new PostOffice { Name = "Postbank Fraud Analytics And Detection", CityId = 77 },
                new PostOffice { Name = "Postbank Human Resources", CityId = 77 },
                new PostOffice { Name = "Postbank Operations", CityId = 77 },
                new PostOffice { Name = "Postbank Portfolio Management Office", CityId = 77 },
                new PostOffice { Name = "Postbank Process Quality and SLA Monitoring", CityId = 77 },
                new PostOffice { Name = "Postbank System Support: Production", CityId = 77 },
                new PostOffice { Name = "Reddersburg", CityId = 78 },
                new PostOffice { Name = "Reitz", CityId = 76 },
                new PostOffice { Name = "Renosterspruit", CityId = 77 },
                new PostOffice { Name = "Riebeeckstad", CityId = 75 },
                new PostOffice { Name = "Rosendal", CityId = 76 },
                new PostOffice { Name = "Rouxville", CityId = 78 },
                new PostOffice { Name = "Sasolburg", CityId = 79 },
                new PostOffice { Name = "Selosesha", CityId = 77 },
                new PostOffice { Name = "Senekal", CityId = 76 },
                new PostOffice { Name = "Sheridan", CityId = 77 },
                new PostOffice { Name = "SIS Central Region", CityId = 81 },
                new PostOffice { Name = "Slabberts", CityId = 75 },
                new PostOffice { Name = "Smithfield", CityId = 78 },
                new PostOffice { Name = "Soutpan", CityId = 77 },
                new PostOffice { Name = "Springfontein", CityId = 78 },
                new PostOffice { Name = "Steynsrus", CityId = 79 },
                new PostOffice { Name = "Thaba Nchu", CityId = 77 },
                new PostOffice { Name = "Theunissen", CityId = 75 },
                new PostOffice { Name = "Trompsburg", CityId = 78 },
                new PostOffice { Name = "Tsheseng", CityId = 76 },
                new PostOffice { Name = "Tshiame", CityId = 76 },
                new PostOffice { Name = "Tweeling", CityId = 79 },
                new PostOffice { Name = "Tweespruit", CityId = 78 },
                new PostOffice { Name = "Ventersburg", CityId = 75 },
                new PostOffice { Name = "Verkykerskop", CityId = 76 },
                new PostOffice { Name = "Vierfontein", CityId = 79 },
                new PostOffice { Name = "Viljoenskroon", CityId = 79 },
                new PostOffice { Name = "Villiers", CityId = 79 },
                new PostOffice { Name = "Virginia", CityId = 75 },
                new PostOffice { Name = "Vrede", CityId = 76 },
                new PostOffice { Name = "Vredefort", CityId = 79 },
                new PostOffice { Name = "Warden", CityId = 76 },
                new PostOffice { Name = "Welkom", CityId = 75 },
                new PostOffice { Name = "Wepener", CityId = 78 },
                new PostOffice { Name = "Wesselsbron", CityId = 75 },
                new PostOffice { Name = "Wiegandia", CityId = 77 },
                new PostOffice { Name = "Winburg", CityId = 75 },
                new PostOffice { Name = "Zamdela", CityId = 79 },
                new PostOffice { Name = "Zastron", CityId = 78 },
                new PostOffice { Name = "Bronville", CityId = 75 },
                new PostOffice { Name = "Bultfontein", CityId = 75 },
                new PostOffice { Name = "Aggeneys", CityId = 82 },
                new PostOffice { Name = "Alexander Bay", CityId = 82 },
                new PostOffice { Name = "Area Office-Diamond Field", CityId = 83 },
                new PostOffice { Name = "Area Office-Kalahari", CityId = 82 },
                new PostOffice { Name = "Askham", CityId = 82 },
                new PostOffice { Name = "Barkly West", CityId = 83 },
                new PostOffice { Name = "Bothithong", CityId = 84 },
                new PostOffice { Name = "Brandvlei", CityId = 82 },
                new PostOffice { Name = "Colesberg", CityId = 83 },
                new PostOffice { Name = "Concordia", CityId = 82 },
                new PostOffice { Name = "De Aar", CityId = 83 },
                new PostOffice { Name = "Delportshoop", CityId = 83 },
                new PostOffice { Name = "Fraserburg", CityId = 82 },
                new PostOffice { Name = "Dithakong", CityId = 85 },
                new PostOffice { Name = "Douglas", CityId = 83 },
                new PostOffice { Name = "Dyansonsklip", CityId = 82 },
                new PostOffice { Name = "Edenburg", CityId = 86 },
                new PostOffice { Name = "Eksteenfontein", CityId = 82 },
                new PostOffice { Name = "Galeshewe", CityId = 83 },
                new PostOffice { Name = "Ganspan", CityId = 83 },
                new PostOffice { Name = "Garagams", CityId = 82 },
                new PostOffice { Name = "Gariepdam", CityId = 83 },
                new PostOffice { Name = "Griekwastad", CityId = 83 },
                new PostOffice { Name = "Groblershoop", CityId = 82 },
                new PostOffice { Name = "Grootdrink", CityId = 82 },
                new PostOffice { Name = "Hartswater", CityId = 83 },
                new PostOffice { Name = "Hondeklipbaai", CityId = 82 },
                new PostOffice { Name = "Hotazel", CityId = 82 },
                new PostOffice { Name = "Jan Kempdorp", CityId = 83 },
                new PostOffice { Name = "Kakamas", CityId = 82 },
                new PostOffice { Name = "Kamden", CityId = 82 },
                new PostOffice { Name = "Kamiesberg", CityId = 82 },
                new PostOffice { Name = "Kamieskroon", CityId = 82 },
                new PostOffice { Name = "Kanoneiland", CityId = 82 },
                new PostOffice { Name = "Kathu", CityId = 82 },
                new PostOffice { Name = "Keimoes", CityId = 82 },
                new PostOffice { Name = "Kenhardt", CityId = 82 },
                new PostOffice { Name = "Kgomotso", CityId = 83 },
                new PostOffice { Name = "Kimberley", CityId = 83 },
                new PostOffice { Name = "Koingnaas", CityId = 82 },
                new PostOffice { Name = "Komaggas", CityId = 82 },
                new PostOffice { Name = "Kotzesrus", CityId = 82 },
                new PostOffice { Name = "Kuruman", CityId = 82 },
                new PostOffice { Name = "Lekkersing", CityId = 82 },
                new PostOffice { Name = "Lime Acres", CityId = 82 },
                new PostOffice { Name = "Loeriesfontein", CityId = 82 },
                new PostOffice { Name = "Losasaneng", CityId = 83 },
                new PostOffice { Name = "Louisvaleweg", CityId = 82 },
                new PostOffice { Name = "Loxton", CityId = 83 },
                new PostOffice { Name = "Magogong", CityId = 83 },
                new PostOffice { Name = "Mankurwane", CityId = 83 },
                new PostOffice { Name = "Marydale", CityId = 83 },
                new PostOffice { Name = "Matsheng", CityId = 84 },
                new PostOffice { Name = "Melton Wold", CityId = 83 },
                new PostOffice { Name = "Mier", CityId = 82 },
                new PostOffice { Name = "Modderrivier", CityId = 83 },
                new PostOffice { Name = "Mothibistat", CityId = 82 },
                new PostOffice { Name = "Niekerkshoop", CityId = 83 },
                new PostOffice { Name = "Nieuwoudtville", CityId = 82 },
                new PostOffice { Name = "Noenieput", CityId = 82 },
                new PostOffice { Name = "Noupoort", CityId = 83 },
                new PostOffice { Name = "Nourivier", CityId = 82 },
                new PostOffice { Name = "Olifantshoek", CityId = 82 },
                new PostOffice { Name = "Orania", CityId = 83 },
                new PostOffice { Name = "Paballelo", CityId = 82 },
                new PostOffice { Name = "Pampierstad", CityId = 83 },
                new PostOffice { Name = "Pella", CityId = 82 },
                new PostOffice { Name = "Pescodia", CityId = 83 },
                new PostOffice { Name = "Petrusville", CityId = 83 },
                new PostOffice { Name = "Pniel Farm 281", CityId = 83 },
                new PostOffice { Name = "Pofadder", CityId = 82 },
                new PostOffice { Name = "Port Nolloth", CityId = 82 },
                new PostOffice { Name = "Postmasburg", CityId = 82 },
                new PostOffice { Name = "Prieska", CityId = 83 },
                new PostOffice { Name = "Richmond (Cape)", CityId = 83 },
                new PostOffice { Name = "Salt Lake", CityId = 83 },
                new PostOffice { Name = "Santoy", CityId = 82 },
                new PostOffice { Name = "Sekhing", CityId = 83 },
                new PostOffice { Name = "Shaleng", CityId = 83 },
                new PostOffice { Name = "Soebatsfontein", CityId = 82 },
                new PostOffice { Name = "Spoegrivier", CityId = 82 },
                new PostOffice { Name = "Springbok", CityId = 82 },
                new PostOffice { Name = "Steinkopf", CityId = 82 },
                new PostOffice { Name = "Strydenburg", CityId = 83 },
                new PostOffice { Name = "Sutherland", CityId = 82 },
                new PostOffice { Name = "Ulco", CityId = 83 },
                new PostOffice { Name = "Upington", CityId = 82 },
                new PostOffice { Name = "Vanderkloofdam", CityId = 83 },
                new PostOffice { Name = "Vanwyksvlei", CityId = 83 },
                new PostOffice { Name = "Vanzylsrus", CityId = 82 },
                new PostOffice { Name = "Victoria West", CityId = 83 },
                new PostOffice { Name = "Vosburg", CityId = 83 },
                new PostOffice { Name = "Warrenton", CityId = 83 },
                new PostOffice { Name = "Williston", CityId = 82 },
                new PostOffice { Name = "Britstown", CityId = 83 },
                new PostOffice { Name = "Buffelsrivier", CityId = 82 },
                new PostOffice { Name = "Calvinia", CityId = 82 },
                new PostOffice { Name = "Carnarvon", CityId = 83 },
                new PostOffice { Name = "Cassel", CityId = 84 });
                await context.SaveChangesAsync();
            }
        }
    }
}