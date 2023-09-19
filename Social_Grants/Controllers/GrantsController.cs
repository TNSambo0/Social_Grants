using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Social_Grants.Data;
using Microsoft.AspNetCore.Identity;
using Social_Grants.Models.Account;
using Social_Grants.Models.Grant;
using Social_Grants.Models;
using System.Drawing;

namespace Social_Grants.Controllers
{
    public class GrantsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _AppDb; 
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public GrantsController(ILogger<HomeController> logger, SignInManager<AppUser> signInManager
            , AppDbContext AppDb, UserManager<AppUser> UserManager, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _signInManager = signInManager;
            _AppDb = AppDb;
            _UserManager = UserManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> CheckStatus()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger))
                {
                    return RedirectToAction("Login", "Account");
                }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var grantApplications = from application in _AppDb.TblGrantApplications
                                        join grant in _AppDb.TblGrants
                                        on application.GrantId equals grant.Id
                                        join payment in _AppDb.TblChosenPaymentMethod
                                        on application.AppUserId equals payment.AppUserId
                                        where application.AppUserId ==
                                        user.Id
                                        select new GrantApplications
                                        {
                                            Id = application.Id,
                                            IDNumber = application.IDNumber,
                                            ApplicationId = application.ApplicationId,
                                            GrantName = grant.Name,
                                            ApplicationStatus = application.ApplicationStatus,
                                            Reason = application.Reason,
                                            DateCreated = application.DateCreated,
                                            PayDate = application.PayDate,
                                            MethodOfPayment = application.MethodOfPayment,
                                            GrantId = application.GrantId,
                                            AppUserId = application.AppUserId,

                                        };
                var ApprovedApplications = await grantApplications.FirstOrDefaultAsync(x => x.MethodOfPayment == "Post Office" || x.MethodOfPayment == "Bank account");
                if (ApprovedApplications != null)
                {
                    ViewData["ShowPMBtn"] = true;
                }
                if (await grantApplications.AnyAsync())
                {
                    ViewData["Applications"] = true;
                    return View(grantApplications.AsAsyncEnumerable());
                }
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult CareDependencyGrant()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> CareDependencyGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CareDependencyGrantForm(CareDependencyGrant careDependencyGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                AddDependent addDependent = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var dateOfBirth = AgeChecker.DateOfBirth(careDependencyGrant.Dependent.IDNumber);
                    if (AgeChecker.Check(dateOfBirth) >= 18)
                    {
                        ModelState.AddModelError(string.Empty, "The child must be less than 18 to be considered for the grant.");
                        return View(careDependencyGrant);
                    }
                    if (careDependencyGrant.Dependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(careDependencyGrant);
                    }
                    var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                    var CDGrantApplications = from apps in _AppDb.TblGrantApplications
                                              join AGrant in _AppDb.TblGrants on
                                              apps.GrantId equals AGrant.Id
                                              where AGrant.Name == "Care Dependancy grant"
                                              select apps;
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Care Dependancy grant");
                    var appId = 1;
                    if (await CDGrantApplications.AnyAsync())
                    {
                        appId = (await CDGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                    }
                    GrantApplications grantApplication;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = careDependencyGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, careDependencyGrant.Dependent, user);
                        var files = new List<Files>()
                        {
                            new Files
                            {
                                file = careDependencyGrant.Dependent.IdentityDocument,
                                name = "IdentityDocument"
                            },
                            new Files
                            {
                                file = careDependencyGrant.MedicalReport,
                                name = "MedicalReport"
                            },new Files
                            {
                                file = careDependencyGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                        };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, careDependencyGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = from apps in userApplications where apps.GrantId == grant.Id && apps.IDNumber == careDependencyGrant.Dependent.IDNumber select apps;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = careDependencyGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, careDependencyGrant.Dependent, user);
                        var files = new List<Files>()
                        {
                            new Files
                            {
                                file = careDependencyGrant.Dependent.IdentityDocument,
                                name = "IdentityDocument"
                            },new Files
                            {
                                file = careDependencyGrant.MedicalReport,
                                name = "MedicalReport"
                            },new Files
                            {
                                file = careDependencyGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                        };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, careDependencyGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(careDependencyGrant);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(careDependencyGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult ChildSupport()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> ChildSupportForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChildSupportForm(ChildSupportGrant childSupportGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                AddDependent addDependent = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var dateOfBirth = AgeChecker.DateOfBirth(childSupportGrant.Dependent.IDNumber);
                    if (AgeChecker.Check(dateOfBirth) >= 18)
                    {
                        ModelState.AddModelError(string.Empty, "The child must be less than 18 to be considered for the grant.");
                        return View(childSupportGrant);
                    }
                    if (childSupportGrant.Dependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(childSupportGrant);
                    }
                    var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                    var CSGrantApplications = from apps in _AppDb.TblGrantApplications
                                              join AGrant in _AppDb.TblGrants on
                                              apps.GrantId equals AGrant.Id
                                              where AGrant.Name == "Child Support grant"
                                              select apps;
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Child Support grant");
                    var appId = 1;
                    if (await CSGrantApplications.AnyAsync())
                    {
                        appId = (await CSGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                    }
                    GrantApplications grantApplication;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = childSupportGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, childSupportGrant.Dependent, user);
                        var files = new List<Files>()
                        {
                            new Files
                            {
                                file = childSupportGrant.Dependent.IdentityDocument,
                                name = "IdentityDocument"
                            },new Files
                            {
                                file = childSupportGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                        };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, childSupportGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = userApplications.Where(apps => apps.GrantId == grant.Id && apps.IDNumber == childSupportGrant.Dependent.IDNumber);
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = childSupportGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, childSupportGrant.Dependent, user);
                        var files = new List<Files>()
                        {
                            new Files
                            {
                                file = childSupportGrant.Dependent.IdentityDocument,
                                name = "IdentityDocument"
                            },new Files
                            {
                                file = childSupportGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                        };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, childSupportGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(childSupportGrant);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(childSupportGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult DisabilityGrant()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> DisabilityGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                DisabilityGrant disabilityApp = new()
                {
                    ForWho = _AppDb.TblApplicationForWho.Select(x => new SelectListItem() { Text = x.Answer, Value = x.Id.ToString() }).AsEnumerable(),
                    ForWhoId = 1
                };

                return View(disabilityApp);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisabilityGrantForm(DisabilityGrant disabilityGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                AddDependent addDependent = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                disabilityGrant.ForWho = _AppDb.TblApplicationForWho.Select(x => new SelectListItem() { Text = x.Answer, Value = x.Id.ToString() }).AsEnumerable();
                var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                var DisabilityGrantApplications = from apps in _AppDb.TblGrantApplications
                                                  join AGrant in _AppDb.TblGrants on
                                                  apps.GrantId equals AGrant.Id
                                                  where AGrant.Name == "Disability grant"
                                                  select apps;
                var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Disability grant");
                var appId = 1;
                if (await DisabilityGrantApplications.AnyAsync())
                {
                    appId = (await DisabilityGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                }
                GrantApplications grantApplication;
                if (disabilityGrant.ForWhoId == 1)
                {
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var files = new List<Files>()
                            {
                                new Files
                                {
                                    file = disabilityGrant.MedicalReport,
                                    name = "MedicalReport"
                                },new Files
                            {
                                file = disabilityGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                            };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, user.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = from apps in userApplications where apps.GrantId == grant.Id select apps;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var files = new List<Files>()
                            {
                                new Files
                                {
                                    file = disabilityGrant.MedicalReport,
                                    name = "MedicalReport"
                                },new Files
                            {
                                file = disabilityGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                            };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, user.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(disabilityGrant);
                }
                else if (disabilityGrant.ForWhoId == 2)
                {
                    if (disabilityGrant.CaredByState == null && disabilityGrant.Dependent.IDNumber == null && disabilityGrant.Dependent.FullName
                        == null && disabilityGrant.Dependent.LastName == null && disabilityGrant.Dependent.Nationality == null
                        && disabilityGrant.CaredByState == null && disabilityGrant.OtherGrant == null
                        && disabilityGrant.Dependent.IdentityDocument == null)
                    {
                        ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                        return View(disabilityGrant);
                    }
                    if (disabilityGrant.Dependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(disabilityGrant);
                    }
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = disabilityGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, disabilityGrant.Dependent, user);
                        var files = new List<Files>()
                            {
                                new Files
                                {
                                    file = disabilityGrant.Dependent.IdentityDocument,
                                    name = "IdentityDocument"
                                },
                                new Files
                                {
                                    file = disabilityGrant.MedicalReport,
                                    name = "MedicalReport"
                                },new Files
                                {
                                    file = disabilityGrant.ApplicationForm,
                                    name = "ApplicationForm"
                                }
                            };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, disabilityGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = from apps in userApplications where apps.GrantId == grant.Id && apps.IDNumber == disabilityGrant.Dependent.IDNumber select apps;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = disabilityGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, disabilityGrant.Dependent, user);
                        var files = new List<Files>()
                            {
                                new Files
                                {
                                    file = disabilityGrant.Dependent.IdentityDocument,
                                    name = "IdentityDocument"
                                },
                                new Files
                                {
                                    file = disabilityGrant.MedicalReport,
                                    name = "MedicalReport"
                                },new Files
                                {
                                    file = disabilityGrant.ApplicationForm,
                                    name = "ApplicationForm"
                                }
                            };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, disabilityGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(disabilityGrant);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                    return View(disabilityGrant);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult FosterGrant()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> FosterGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FosterGrantForm(FosterGrant fosterGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                AddDependent addDependent = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var dateOfBirth = AgeChecker.DateOfBirth(user.IDNumber);
                    if (AgeChecker.Check(dateOfBirth) < 65)
                    {
                        ModelState.AddModelError(string.Empty, "The child must be less than 18 to be considered for the grant.");
                        return View(fosterGrant);
                    }
                    if (fosterGrant.Dependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(fosterGrant);
                    }
                    var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                    var FosterGrantApplications = from apps in _AppDb.TblGrantApplications
                                                  join AGrant in _AppDb.TblGrants on
                                                  apps.GrantId equals AGrant.Id
                                                  where AGrant.Name == "Foster grant"
                                                  select apps;
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Foster grant");
                    var appId = 1;
                    if (await FosterGrantApplications.AnyAsync())
                    {
                        appId = (await FosterGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                    }
                    GrantApplications grantApplication;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = fosterGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, fosterGrant.Dependent, user);
                        var files = new List<Files>()
                        {
                            new Files
                            {
                                file = fosterGrant.Dependent.IdentityDocument,
                                name = "IdentityDocument"
                            },new Files
                            {
                                file = fosterGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                        };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, fosterGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = from apps in userApplications where apps.GrantId == grant.Id && apps.IDNumber == fosterGrant.Dependent.IDNumber select apps;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = fosterGrant.Dependent.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        await addDependent.CreateDependent(_AppDb, fosterGrant.Dependent, user);
                        var files = new List<Files>()
                        {
                            new Files
                            {
                                file = fosterGrant.Dependent.IdentityDocument,
                                name = "IdentityDocument"
                            },new Files
                            {
                                file = fosterGrant.ApplicationForm,
                                name = "ApplicationForm"
                            }
                        };
                        //FileSaver.Savefiles(_webHostEnvironment, files, user, fosterGrant.Dependent.IDNumber);
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(fosterGrant);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(fosterGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult GrantInAid()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> GrantInAidForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantInAidForm(GrantInAid grantInAid)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                    var grantInAidApplications = from apps in _AppDb.TblGrantApplications
                                                 join AGrant in _AppDb.TblGrants on
                                                 apps.GrantId equals AGrant.Id
                                                 where AGrant.Name == "Grant In Aid grant"
                                                 select apps;
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Grant In Aid grant");
                    var appId = 1;
                    if (await grantInAidApplications.AnyAsync())
                    {
                        appId = (await grantInAidApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                    }
                    GrantApplications grantApplication;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = userApplications.Where(apps => apps.GrantId == grant.Id);
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(grantInAid);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(grantInAid);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult OldAgeGrant()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> OldAgeGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OldAgeGrantForm(OldAgeGrant oldAgeGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var dateOfBirth = AgeChecker.DateOfBirth(user.IDNumber);
                    if (AgeChecker.Check(dateOfBirth) < 60)
                    {
                        ModelState.AddModelError(string.Empty, "You should be 60 or older to be considered for the grant.");
                        return View(oldAgeGrant);
                    }
                    var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                    var WVApplications = from apps in _AppDb.TblGrantApplications
                                         join AGrant in _AppDb.TblGrants on
                                         apps.GrantId equals AGrant.Id
                                         where AGrant.Name == "Old Age grant"
                                         select apps;
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Old Age grant");
                    var appId = 1;
                    if (await WVApplications.AnyAsync())
                    {
                        appId = (await WVApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                    }
                    GrantApplications grantApplication;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = userApplications.Where(apps => apps.GrantId == grant.Id);
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(oldAgeGrant);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(oldAgeGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult SocialReliefDistressGrant()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> SocialReliefDistressGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SocialReliefDistressGrantForm(SocialReliefDistressGrant socialReliefDistressGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                    var SRDApplications = from apps in _AppDb.TblGrantApplications
                                          join AGrant in _AppDb.TblGrants on
                                          apps.GrantId equals AGrant.Id
                                          where AGrant.Name == "Social Relief of Distress grant"
                                          select apps;
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Social Relief of Distress grant");
                    var appId = 1;
                    if (await SRDApplications.AnyAsync())
                    {
                        appId = (await SRDApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                    }
                    GrantApplications grantApplication;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = userApplications.Where(apps => apps.GrantId == grant.Id);
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(socialReliefDistressGrant);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(socialReliefDistressGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public IActionResult WarVeteranGrant()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        public async Task<IActionResult> WarVeteranGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WarVeteranGrantForm(WarVeteranGrant warVeteranGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var userApplications = from apps in _AppDb.TblGrantApplications where apps.AppUserId == user.Id select apps;
                    var WVApplications = from apps in _AppDb.TblGrantApplications
                                         join AGrant in _AppDb.TblGrants on
                                         apps.GrantId equals AGrant.Id
                                         where AGrant.Name == "War Veteran grant"
                                         select apps;
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "War Veteran grant");
                    var appId = 1;
                    if (await WVApplications.AnyAsync())
                    {
                        appId = (await WVApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                    }
                    GrantApplications grantApplication;
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    userApplications = userApplications.Where(apps => apps.GrantId == grant.Id);
                    if (!await userApplications.AnyAsync())
                    {
                        grantApplication = new()
                        {
                            ApplicationStatus = "Pending",
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            IDNumber = user.IDNumber,
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("CheckStatus");
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(warVeteranGrant);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(warVeteranGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
    }
}
