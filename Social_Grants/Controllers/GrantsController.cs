using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Social_Grants.Data;
using System.Linq;
using Social_Grants.Models;
using Social_Grants.Models.Account;
using Social_Grants.Models.Grant;
using Social_Grants.Services.Abstract;

namespace Social_Grants.Controllers
{
    public class GrantsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _signInManager;
        protected IAuthorizationService _authorizationService;
        //private readonly IDocumentService _iDocumentService;
        public GrantsController(ILogger<HomeController> logger, SignInManager<AppUser> signInManager
            , AppDbContext AppDb, /*IDocumentService iDocumentService, */UserManager<AppUser> UserManager, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _AppDb = AppDb;
            _UserManager = UserManager;
            _authorizationService = authorizationService;
            //_iDocumentService = iDocumentService;
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
                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Read);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to view applications.";
                    return RedirectToAction("Index", "Home");
                }
                var grantApplications = from application in _AppDb.TblGrantApplications
                                        join grant in _AppDb.TblGrants
                                        on application.GrantId equals grant.Id
                                        join payment in _AppDb.TblChosenPaymentMethod
                                        on application.AppUserId equals user.Id into userApplications
                                        from userApplication in userApplications.DefaultIfEmpty()
                                        select new GrantApplications
                                        {
                                            Id = application.Id,
                                            ApplicationId = application.ApplicationId,
                                            GrantName = grant.Name,
                                            Status = application.Status,
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
            }
            return View();
        }

        public IActionResult CareDependencyGrant()
        {
            return View();
        }
        public async Task<IActionResult> CareDependencyGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("CareDependencyGrant");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                CareDependencyGrant careDependencyGrant = new()
                {
                    ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() }),
                    DependentsCount = await dependers.AnyAsync() ? true : null
                };
                return View(careDependencyGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return Problem();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CareDependencyGrantForm(CareDependencyGrant careDependencyGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                careDependencyGrant.ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() });
                if (ModelState.IsValid)
                {
                    var selectedDependent = await dependers.SingleOrDefaultAsync(x => x.Id == careDependencyGrant.DependentId);
                    if (selectedDependent == null)
                    {
                        ModelState.AddModelError("", "An error occured, please try again.");
                        return View(careDependencyGrant);
                    }
                    var dateOfBirth = AgeChecker.DateOfBirth(selectedDependent.IDNumber);
                    if (AgeChecker.Check(dateOfBirth) >= 18)
                    {
                        ModelState.AddModelError(string.Empty, "The child must be less than 18 to be considered for the grant.");
                        return View(careDependencyGrant);
                    }
                    if (selectedDependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(careDependencyGrant);
                    }
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Care Dependancy grant");
                    if (grant == null)
                        return NotFound();
                    //var dependentFromDb = _AppDb.TblDependent.FindAsync(careDependencyGrant.DependentId);
                    var userApplication = await (from grantApplicationFromDb in _AppDb.TblGrantApplications
                                                 join dependentFromDb in _AppDb.TblDependent on
                                                 grantApplicationFromDb.AppUserId equals user.Id
                                                 where grantApplicationFromDb.GrantId == grant.Id &&
                                                 dependentFromDb.Id == careDependencyGrant.DependentId
                                                 select new GrantApplications
                                                 {
                                                     Id = grantApplicationFromDb.Id,
                                                     Status = grantApplicationFromDb.Status,
                                                     AppUserId = grantApplicationFromDb.AppUserId,
                                                     DateCreated = grantApplicationFromDb.DateCreated,
                                                     GrantId = grantApplicationFromDb.GrantId,
                                                     ApplicationId = grantApplicationFromDb.ApplicationId,
                                                     GrantName = grantApplicationFromDb.GrantName,
                                                     AppUser = grantApplicationFromDb.AppUser,
                                                     Reason = grantApplicationFromDb.Reason,
                                                     PayDate = grantApplicationFromDb.PayDate,
                                                     MethodOfPayment = grantApplicationFromDb.MethodOfPayment,
                                                 }).FirstOrDefaultAsync();
                    if (userApplication == null)
                    {
                        var CDGrantApplications = from apps in _AppDb.TblGrantApplications
                                                  join AGrant in _AppDb.TblGrants on
                                                  apps.GrantId equals AGrant.Id
                                                  where AGrant.Name == "Care Dependancy grant"
                                                  select apps;

                        var appId = 1;
                        if (await CDGrantApplications.AnyAsync())
                        {
                            appId = (await CDGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                        }
                        GrantApplications grantApplication;
                        var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                        if (CaredByState == null)
                            return NotFound();
                        grantApplication = new()
                        {
                            Status = ApplicationStatus.Pending,
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            AppUser = user,
                            Reason = null,
                            PayDate = null,
                            MethodOfPayment = null,
                            ApplicantType = "Dependent"
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                new List<AnswerAndQuestionId>()
                                {
                                    new AnswerAndQuestionId { Answer = careDependencyGrant.CaredByState, QuestionId = CaredByState.Id }},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = careDependencyGrant.DependentId,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                        return View(careDependencyGrant);
                    }
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
                return View(careDependencyGrant);
            }
        }
        public IActionResult ChildSupport()
        {
            return View();
        }
        public async Task<IActionResult> ChildSupportForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("ChildSupport");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                ChildSupportGrant childSupportGrant = new()
                {
                    ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() }),
                    DependentsCount = await dependers.AnyAsync() ? true : null
                };
                return View(childSupportGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChildSupportForm(ChildSupportGrant childSupportGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                childSupportGrant.ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() });
                if (ModelState.IsValid)
                {
                    var selectedDependent = await dependers.SingleOrDefaultAsync(x => x.Id == childSupportGrant.DependentId);
                    if (selectedDependent == null)
                    {
                        ModelState.AddModelError("", "An error occured, please try again.");
                        return View(childSupportGrant);
                    }
                    var dateOfBirth = AgeChecker.DateOfBirth(selectedDependent.IDNumber);
                    if (AgeChecker.Check(dateOfBirth) >= 18)
                    {
                        ModelState.AddModelError(string.Empty, "The child must be less than 18 to be considered for the grant.");
                        return View(childSupportGrant);
                    }
                    if (selectedDependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(childSupportGrant);
                    }
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Child Support grant");
                    if (grant == null)
                        return NotFound();
                    var userApplication = await (from grantApplicationFromDb in _AppDb.TblGrantApplications
                                                 join dependentFromDb in _AppDb.TblDependent on
                                                 grantApplicationFromDb.AppUserId equals user.Id
                                                 where grantApplicationFromDb.GrantId == grant.Id &&
                                                 dependentFromDb.Id == childSupportGrant.DependentId
                                                 select new GrantApplications
                                                 {
                                                     Id = grantApplicationFromDb.Id,
                                                     Status = grantApplicationFromDb.Status,
                                                     AppUserId = grantApplicationFromDb.AppUserId,
                                                     DateCreated = grantApplicationFromDb.DateCreated,
                                                     GrantId = grantApplicationFromDb.GrantId,
                                                     ApplicationId = grantApplicationFromDb.ApplicationId,
                                                     GrantName = grantApplicationFromDb.GrantName,
                                                     AppUser = grantApplicationFromDb.AppUser,
                                                     Reason = grantApplicationFromDb.Reason,
                                                     PayDate = grantApplicationFromDb.PayDate,
                                                     MethodOfPayment = grantApplicationFromDb.MethodOfPayment,
                                                 }).FirstOrDefaultAsync();
                    if (userApplication == null)
                    {
                        var CDGrantApplications = from apps in _AppDb.TblGrantApplications
                                                  join AGrant in _AppDb.TblGrants on
                                                  apps.GrantId equals AGrant.Id
                                                  where AGrant.Name == "Care Dependancy grant"
                                                  select apps;
                        var appId = 1;
                        if (await CDGrantApplications.AnyAsync())
                        {
                            appId = (await CDGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                        }
                        GrantApplications grantApplication;
                        var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                        if (CaredByState == null)
                            return NotFound();
                        grantApplication = new()
                        {
                            Status = ApplicationStatus.Pending,
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            AppUser = user,
                            Reason = null,
                            PayDate = null,
                            MethodOfPayment = null,
                            ApplicantType = "Dependent"
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = childSupportGrant.CaredByState, QuestionId = CaredByState.Id }},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = childSupportGrant.DependentId,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                        return View(childSupportGrant);
                    }
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
                return View(childSupportGrant);
            }
        }

        public IActionResult DisabilityGrant()
        {
            return View();
        }
        public async Task<IActionResult> DisabilityGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("DisabilityGrant");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                DisabilityGrant disabilityApp = new()
                {
                    ForWho = _AppDb.TblApplicationForWho.Select(x => new SelectListItem() { Text = x.Answer, Value = x.Id.ToString() }).AsEnumerable(),
                    ForWhoId = 1,
                    ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() }),
                    DependentsCount = await dependers.AnyAsync() ? true : null
                };
                return View(disabilityApp);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisabilityGrantForm(DisabilityGrant disabilityGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                disabilityGrant.ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() });
                var selectedDependent = await dependers.SingleOrDefaultAsync(x => x.Id == disabilityGrant.DependentId);
                if (selectedDependent == null)
                {
                    ModelState.AddModelError("", "An error occured, please try again.");
                    return View(disabilityGrant);
                }
                disabilityGrant.ForWho = _AppDb.TblApplicationForWho.Select(x => new SelectListItem() { Text = x.Answer, Value = x.Id.ToString() }).AsEnumerable();
                var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Disability grant");
                if (grant == null)
                    return NotFound();
                var userApplication = from grantApplicationFromDb in _AppDb.TblGrantApplications
                                      where grantApplicationFromDb.GrantId == grant.Id
                                      && grantApplicationFromDb.AppUserId == user.Id
                                      select grantApplicationFromDb;
                var DisabilityGrantApplications = from apps in _AppDb.TblGrantApplications
                                                  join AGrant in _AppDb.TblGrants on
                                                  apps.GrantId equals AGrant.Id
                                                  where AGrant.Name == "Disability grant"
                                                  select apps;
                var appId = 1;
                if (await DisabilityGrantApplications.AnyAsync())
                {
                    appId = (await DisabilityGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                }
                GrantApplications grantApplication;
                var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                var OtherGrant = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Recieve other grant" && x.GrantId == grant.Id);
                if (CaredByState == null || OtherGrant == null)
                    return NotFound();
                grantApplication = new()
                {
                    Status = ApplicationStatus.Pending,
                    AppUserId = user.Id,
                    DateCreated = DateTime.Now,
                    GrantId = grant.Id,
                    ApplicationId = appId,
                    GrantName = grant.Name,
                    AppUser = user,
                    Reason = null,
                    PayDate = null,
                    MethodOfPayment = null,
                };
                if (disabilityGrant.ForWhoId == 1)
                {
                    if (await userApplication.FirstOrDefaultAsync() == null)
                    {
                        grantApplication.ApplicantType = "Self";
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = disabilityGrant.CaredByState, QuestionId = CaredByState.Id },
                                new AnswerAndQuestionId { Answer = disabilityGrant.OtherGrant, QuestionId = OtherGrant.Id}},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = disabilityGrant.DependentId,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
                    }
                    ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                    return View(disabilityGrant);
                }
                else if (disabilityGrant.ForWhoId == 2)
                {
                    if (disabilityGrant.CaredByState == null && selectedDependent.IDNumber == null && selectedDependent.FullName
                        == null && selectedDependent.LastName == null && selectedDependent.Nationality == null
                        && disabilityGrant.CaredByState == null && disabilityGrant.OtherGrant == null
                        && selectedDependent.IdentityDocument == null)
                    {
                        ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                        return View(disabilityGrant);
                    }
                    if (selectedDependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(disabilityGrant);
                    }
                    var userApplicationFromDb = await (from grantApplicationFromDb in userApplication
                                                       join dependentFromDb in _AppDb.TblDependent on
                                                       grantApplicationFromDb.AppUserId equals user.Id
                                                       where dependentFromDb.Id == disabilityGrant.DependentId
                                                       select grantApplicationFromDb).FirstOrDefaultAsync();
                    if (userApplicationFromDb == null)
                    {
                        grantApplication.ApplicantType = "Dependent";
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = disabilityGrant.CaredByState, QuestionId = CaredByState.Id },
                                new AnswerAndQuestionId { Answer = disabilityGrant.OtherGrant, QuestionId = OtherGrant.Id }},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = disabilityGrant.DependentId,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
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
                return View(disabilityGrant);
            }
        }

        public IActionResult FosterGrant()
        {
            return View();
        }
        public async Task<IActionResult> FosterGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("FosterGrant");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                FosterGrant fosterGrant = new()
                {
                    ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() }),
                    DependentsCount = await dependers.AnyAsync() ? true : null
                };
                return View(fosterGrant);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FosterGrantForm(FosterGrant fosterGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var dependers = from a in _AppDb.TblDependent where a.AppUserId == user.Id select a;
                fosterGrant.ListOfDependers = dependers.Select(m => new SelectListItem { Text = $"{m.FullName} {m.LastName} {m.IDNumber}", Value = m.Id.ToString() });
                if (ModelState.IsValid)
                {
                    var selectedDependent = await dependers.SingleOrDefaultAsync(x => x.Id == fosterGrant.DependentId);
                    if (selectedDependent == null)
                    {
                        ModelState.AddModelError("", "An error occured, please try again.");
                        return View(fosterGrant);
                    }
                    var dateOfBirth = AgeChecker.DateOfBirth(selectedDependent.IDNumber);
                    var age = AgeChecker.Check(dateOfBirth);
                    if (AgeChecker.Check(dateOfBirth) >= 18)
                    {
                        ModelState.AddModelError(string.Empty, "The child must be less than 18 to be considered for the grant.");
                        return View(fosterGrant);
                    }
                    if (selectedDependent.IDNumber == user.IDNumber)
                    {
                        ModelState.AddModelError(string.Empty, "Dependent ID number can not be the same as of the parent/guardian.");
                        return View(fosterGrant);
                    }
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Foster grant");
                    if (grant == null)
                        return NotFound();
                    var userApplication = await (from grantApplicationFromDb in _AppDb.TblGrantApplications
                                                 join dependentFromDb in _AppDb.TblDependent on
                                                 grantApplicationFromDb.AppUserId equals user.Id
                                                 where grantApplicationFromDb.GrantId == grant.Id &&
                                                 dependentFromDb.Id == fosterGrant.DependentId
                                                 select grantApplicationFromDb).FirstOrDefaultAsync();
                    if (userApplication == null)
                    {
                        var FosterGrantApplications = from apps in _AppDb.TblGrantApplications
                                                      join AGrant in _AppDb.TblGrants on
                                                      apps.GrantId equals AGrant.Id
                                                      where AGrant.Name == "Foster grant"
                                                      select apps;
                        var appId = 1;
                        if (await FosterGrantApplications.AnyAsync())
                        {
                            appId = (await FosterGrantApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                        }
                        GrantApplications grantApplication;
                        var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                        if (CaredByState == null)
                            return NotFound();
                        grantApplication = new()
                        {
                            Status = ApplicationStatus.Pending,
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            AppUser = user,
                            Reason = null,
                            PayDate = null,
                            MethodOfPayment = null,
                            ApplicantType = "Dependent"
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = fosterGrant.CaredByState, QuestionId = CaredByState.Id }},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = fosterGrant.DependentId,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
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
                return View(fosterGrant);
            }
        }

        public IActionResult GrantInAid()
        {
            return View();
        }
        public async Task<IActionResult> GrantInAidForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("GrantInAid");
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantInAidForm(GrantInAid grantInAid)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Grant In Aid grant");
                    if (grant == null)
                        return NotFound();
                    var userApplication = await (from grantApplicationFromDb in _AppDb.TblGrantApplications
                                                 where grantApplicationFromDb.AppUserId == user.Id
                                                 where grantApplicationFromDb.GrantId == grant.Id
                                                 select grantApplicationFromDb).FirstOrDefaultAsync();
                    if (userApplication == null)
                    {
                        var grantInAidApplications = from apps in _AppDb.TblGrantApplications
                                                     join AGrant in _AppDb.TblGrants on
                                                     apps.GrantId equals AGrant.Id
                                                     where AGrant.Name == "Grant In Aid grant"
                                                     select apps;
                        var appId = 1;
                        if (await grantInAidApplications.AnyAsync())
                        {
                            appId = (await grantInAidApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                        }
                        GrantApplications grantApplication;
                        var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                        var OtherGrant = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Do you recieve other such as older person grant, disability grant or war veteran grant" && x.GrantId == grant.Id);
                        var Assistance = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Do you require full-time attendance by another person" && x.GrantId == grant.Id);
                        if (CaredByState == null || OtherGrant == null || Assistance == null)
                            return NotFound();
                        grantApplication = new()
                        {
                            Status = ApplicationStatus.Pending,
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            AppUser = user,
                            Reason = null,
                            PayDate = null,
                            MethodOfPayment = null,
                            ApplicantType = "Self"
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = grantInAid.CaredByState, QuestionId = CaredByState.Id },
                                new AnswerAndQuestionId { Answer = grantInAid.OtherGrant, QuestionId = OtherGrant.Id },
                                new AnswerAndQuestionId { Answer = grantInAid.Assistance, QuestionId = Assistance.Id }},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = null,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
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
                return View(grantInAid);
            }
        }

        public IActionResult OldAgeGrant()
        {
            return View();
        }
        public async Task<IActionResult> OldAgeGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("OldAgeGrant");
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OldAgeGrantForm(OldAgeGrant oldAgeGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
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
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Old Age grant");
                    if (grant == null)
                        return NotFound();
                    var userApplication = await (from grantApplicationFromDb in _AppDb.TblGrantApplications
                                                 where grantApplicationFromDb.AppUserId == user.Id
                                                 && grantApplicationFromDb.GrantId == grant.Id
                                                 select grantApplicationFromDb).FirstOrDefaultAsync();
                    if (userApplication == null)
                    {
                        var oldAgeApplications = from apps in _AppDb.TblGrantApplications
                                                 join AGrant in _AppDb.TblGrants on
                                                 apps.GrantId equals AGrant.Id
                                                 where AGrant.Name == "Old Age grant"
                                                 select apps;
                        var appId = 1;
                        if (await oldAgeApplications.AnyAsync())
                        {
                            appId = (await oldAgeApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                        }
                        GrantApplications grantApplication;
                        var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                        var OtherGrant = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Recieve other grant" && x.GrantId == grant.Id);
                        if (CaredByState == null || OtherGrant == null)
                            return NotFound();
                        grantApplication = new()
                        {
                            Status = ApplicationStatus.Pending,
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            AppUser = user,
                            Reason = null,
                            PayDate = null,
                            MethodOfPayment = null,
                            ApplicantType = "Self"
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                            new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = oldAgeGrant.CaredByState, QuestionId = CaredByState.Id },
                                new AnswerAndQuestionId { Answer = oldAgeGrant.OtherGrant, QuestionId = OtherGrant.Id }},
                            new ApplicantGrantAnswers
                            {
                                AppUserId = user.Id,
                                GrantId = grant.Id,
                                DependentId = null,
                                GrantApplicationId = applicationFromDb.Id
                            });
                            await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                            await _AppDb.SaveChangesAsync();
                            return RedirectToAction("CheckStatus");
                        }
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
                return View(oldAgeGrant);
            }
        }

        public IActionResult SocialReliefDistressGrant()
        {
            return View();
        }
        public async Task<IActionResult> SocialReliefDistressGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("SocialReliefDistressGrant");
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SocialReliefDistressGrantForm(SocialReliefDistressGrant socialReliefDistressGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "Social Relief of Distress grant");
                    if (grant == null)
                        return NotFound();
                    var userApplication = await (from grantApplicationFromDb in _AppDb.TblGrantApplications
                                                 where grantApplicationFromDb.AppUserId == user.Id
                                                 where grantApplicationFromDb.GrantId == grant.Id
                                                 select grantApplicationFromDb).FirstOrDefaultAsync();
                    if (userApplication == null)
                    {
                        var SRDApplications = from apps in _AppDb.TblGrantApplications
                                              join AGrant in _AppDb.TblGrants on
                                              apps.GrantId equals AGrant.Id
                                              where AGrant.Name == "Social Relief of Distress grant"
                                              select apps;
                        var appId = 1;
                        if (await SRDApplications.AnyAsync())
                        {
                            appId = (await SRDApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                        }
                        GrantApplications grantApplication;
                        var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                        var OtherGrant = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Recieve other grant" && x.GrantId == grant.Id);
                        var Employment = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Employment status" && x.GrantId == grant.Id);
                        if (CaredByState == null || OtherGrant == null || Employment == null)
                            return NotFound();
                        grantApplication = new()
                        {
                            Status = ApplicationStatus.Pending,
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            AppUser = user,
                            Reason = null,
                            PayDate = null,
                            MethodOfPayment = null,
                            ApplicantType = "Self"
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = socialReliefDistressGrant.CaredByState, QuestionId = CaredByState.Id },
                                new AnswerAndQuestionId { Answer = socialReliefDistressGrant.OlderPersonGrant, QuestionId = OtherGrant.Id },
                                new AnswerAndQuestionId { Answer = socialReliefDistressGrant.EmploymentStatus, QuestionId = Employment.Id }},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = null,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An application already exists, check your application status.");
                        return View(socialReliefDistressGrant);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                    return View(socialReliefDistressGrant);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(socialReliefDistressGrant);
            }
        }

        public IActionResult WarVeteranGrant()
        {
            return View();
        }
        public async Task<IActionResult> WarVeteranGrantForm()
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                GrantApplications grantApplication = new() { AppUserId = user.Id, AppUser = user };
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, grantApplication, Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to create an application.";
                    return RedirectToAction("WarVeteranGrant");
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WarVeteranGrantForm(WarVeteranGrant warVeteranGrant)
        {
            try
            {
                SessionHandler sessionHandler = new();
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var user = await _UserManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var grant = await _AppDb.TblGrants.SingleOrDefaultAsync(x => x.Name == "War Veteran grant");
                    if (grant == null)
                        return NotFound();
                    var userApplications = await (from grantApplicationFromDb in _AppDb.TblGrantApplications
                                                  where grantApplicationFromDb.AppUserId == user.Id
                                                  where grantApplicationFromDb.GrantId == grant.Id
                                                  select grantApplicationFromDb).FirstOrDefaultAsync();
                    if (userApplications == null)
                    {
                        var WVApplications = from apps in _AppDb.TblGrantApplications
                                             join AGrant in _AppDb.TblGrants on
                                             apps.GrantId equals AGrant.Id
                                             where AGrant.Name == "War Veteran grant"
                                             select apps;

                        var appId = 1;
                        if (await WVApplications.AnyAsync())
                        {
                            appId = (await WVApplications.OrderBy(apps => apps.ApplicationId).LastAsync()).ApplicationId++;
                        }
                        GrantApplications grantApplication;
                        var CaredByState = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Cared by State" && x.GrantId == grant.Id);
                        var OtherGrant = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Recieve other grant" && x.GrantId == grant.Id);
                        var SecondWar = await _AppDb.TblGrantQuestions.FirstOrDefaultAsync(x => x.Question == "Fought in the Second World War or Korean War" && x.GrantId == grant.Id);
                        if (CaredByState == null || OtherGrant == null || SecondWar == null)
                            return NotFound();
                        grantApplication = new()
                        {
                            Status = ApplicationStatus.Pending,
                            AppUserId = user.Id,
                            DateCreated = DateTime.Now,
                            GrantId = grant.Id,
                            ApplicationId = appId,
                            GrantName = grant.Name,
                            AppUser = user,
                            Reason = null,
                            PayDate = null,
                            MethodOfPayment = null,
                            ApplicantType = "Self"
                        };
                        await _AppDb.TblGrantApplications.AddAsync(grantApplication);
                        await _AppDb.SaveChangesAsync();
                        var applicationFromDb = await _AppDb.TblGrantApplications.FirstOrDefaultAsync(x => x.ApplicationId == grantApplication.ApplicationId);
                        if (applicationFromDb == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            await SaveAppplicantGrantAnswers(
                                 new List<AnswerAndQuestionId>()
                                {
                                new AnswerAndQuestionId { Answer = warVeteranGrant.CaredByState, QuestionId = CaredByState.Id },
                                new AnswerAndQuestionId { Answer = warVeteranGrant.OlderPersonGrant, QuestionId = OtherGrant.Id },
                                new AnswerAndQuestionId { Answer = warVeteranGrant.SecondWar, QuestionId = SecondWar.Id }},
                                new ApplicantGrantAnswers
                                {
                                    AppUserId = user.Id,
                                    GrantId = grant.Id,
                                    DependentId = null,
                                    GrantApplicationId = applicationFromDb.Id
                                });
                            return RedirectToAction("CheckStatus");
                        }
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
                return View(warVeteranGrant);
            }
        }

        private async Task SaveAppplicantGrantAnswers(IEnumerable<AnswerAndQuestionId> AnswersAndQuestionIds, ApplicantGrantAnswers applicantGrantAnswers)
        {
            try
            {
                List<ApplicantGrantAnswers> ListOfapplicantGrantAnswers = new();
                foreach (var answerAndQuestionId in AnswersAndQuestionIds)
                {
                    ApplicantGrantAnswers answers = new()
                    {
                        AppUserId = applicantGrantAnswers.AppUserId,
                        GrantId = applicantGrantAnswers.GrantId,
                        DependentId = applicantGrantAnswers.DependentId,
                        GrantApplicationId = applicantGrantAnswers.GrantApplicationId,
                        Answer = answerAndQuestionId.Answer,
                        QuestionId = answerAndQuestionId.QuestionId
                    };
                    ListOfapplicantGrantAnswers.Add(answers); 
                }
                await _AppDb.TblApplicantGrantAnswers.AddRangeAsync(ListOfapplicantGrantAnswers);
                await _AppDb.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
            }
            //var getAnswersFromDb = await _AppDb.TblApplicantGrantAnswers.FirstOrDefaultAsync(x => x.GrantId == applicantGrantAnswers.GrantId &&
            //x.AppUserId == applicantGrantAnswers.AppUserId && x.DependentId == applicantGrantAnswers.DependentId);
            //return getAnswersFromDb.Id;
        }
    }
}
