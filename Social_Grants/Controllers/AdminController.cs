using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Social_Grants.Data;
using Social_Grants.Models;
using Social_Grants.Models.Account;
using Social_Grants.Models.Grant;
using System.Drawing;
using NuGet.Packaging;
using Social_Grants.Services.Abstract;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Linq;

namespace Social_Grants.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        protected IAuthorizationService _AuthorizationService;
        public AdminController(ILogger<AdminController> logger, SignInManager<AppUser> signInManager
            , AppDbContext AppDb, UserManager<AppUser> UserManager, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _AppDb = AppDb;
            _userManager = UserManager;
            _AuthorizationService = authorizationService;
        }
        public async Task<IActionResult> Applications()
        {
            var sessionHandler = new SessionHandler();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
            var applicationsFromDb = _AppDb.TblGrantApplications;
            //var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, applicationFromDb, Operations.Read);
            //if (!isAuthorized.Succeeded)
            //{
            //    TempData["error"] = "You don't have the permission to see application details.";
            //    return RedirectToAction("Index", "Admin");
            //}
            if (!await applicationsFromDb.AnyAsync())
            {
                return View();
            }
            ViewData["Applications"] = "Not Null";
            return View(applicationsFromDb);
        }
        public async Task<IActionResult> ApproveAll()
        {
            var sessionHandler = new SessionHandler();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
            var applicationsFromDb = from application in _AppDb.TblGrantApplications select application;
            if (!await applicationsFromDb.AnyAsync())
            {
                TempData["error"] = "An error occured, please try refreshing the page.";
                return RedirectToAction("Applications", "Admin");
            }
            var isAuthorized = await _userManager.IsInRoleAsync(user, Constants.AdministratorsRole);
            if (!isAuthorized)
            {
                TempData["error"] = "You don't have the permission to change applications statuses.";
                return RedirectToAction("Applications", "Admin");
            }
            await ApproveOrRejectApp(await applicationsFromDb.ToListAsync());
            return RedirectToAction("Applications");
        }
        public async Task<IActionResult> ApplicationDetails(string id, string UserID)
        {
            var sessionHandler = new SessionHandler();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
            var applicantFromDb = await _AppDb.Users.FindAsync(UserID);
            var Id = Convert.ToInt32(id);
            if (applicantFromDb == null)
            {
                return Problem("An error occured, try refreshing the page.");
            }
            var applicationFromDb = await (from grantApplication in _AppDb.TblGrantApplications
                                           join grant in _AppDb.TblGrants
                                           on grantApplication.GrantId equals grant.Id
                                           select new GrantApplications
                                           {
                                               Id = grantApplication.Id,
                                               ApplicationId = grantApplication.ApplicationId,
                                               Grants = grantApplication.Grants,
                                               GrantName = grant.Name,
                                               Status = grantApplication.Status,
                                               Reason = grantApplication.Reason,
                                               DateCreated = grantApplication.DateCreated,
                                               PayDate = grantApplication.PayDate,
                                               MethodOfPayment = grantApplication.MethodOfPayment,
                                               GrantId = grantApplication.GrantId,
                                               AppUserId = applicantFromDb.Id,
                                               AppUser = applicantFromDb,
                                               FullName = $"{applicantFromDb.FirstName} {applicantFromDb.LastName}",
                                               Answers = from x in _AppDb.TblApplicantGrantAnswers where x.AppUserId == applicantFromDb.Id && x.GrantId == grantApplication.GrantId select x,
                                               Questions = from x in _AppDb.TblGrantQuestions where x.GrantId == grantApplication.GrantId select x
                                           }).FirstOrDefaultAsync(x => x.AppUserId == UserID && x.Id == Id);
            //var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, applicationFromDb, Operations.Read);
            //if (!isAuthorized.Succeeded)
            //{
            //    TempData["error"] = "You don't have the permission to see application details.";
            //    return RedirectToAction("Index", "Admin");
            //}
            if (applicationFromDb != null)
                return View(applicationFromDb);
            return NotFound();
        }
        public async Task<IActionResult> ChangePassword()
        {
            try
            {
                SessionHandler sessionHandler = new();
                var user = await _userManager.GetUserAsync(User);
                if (user == null || await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                else
                {
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, user, Operations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to change password.";
                        return Forbid();
                    }
                    var hasPassword = await _userManager.HasPasswordAsync(user);
                    if (!hasPassword)
                    {
                        return RedirectToAction("SetPassword");
                    }
                    return View();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            try
            {
                SessionHandler sessionHandler = new();
                var user = await _userManager.GetUserAsync(User);
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Please make sure all the required field are populated with the required data.");
                    return View();
                }
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(changePassword);
                }

                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("User changed their password successfully.");

                return RedirectToAction("ChangePassword");
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
        public async Task ApproveOrRejectApp(List<GrantApplications> grantApplications)
        {
            try
            {
                var applicantGrantAnswersFromDb = await _AppDb.TblApplicantGrantAnswers.ToListAsync();
                var correctAnswersFromDb = await _AppDb.TblGrantAnswers.ToListAsync();
                List<GrantApplications> ListOfGrants = new();
                for (int m = 0; m < grantApplications.Count(); m++)
                {
                    correctAnswersFromDb = correctAnswersFromDb.Where(Answers => Answers.GrantId == grantApplications[m].GrantId).ToList();
                    applicantGrantAnswersFromDb = applicantGrantAnswersFromDb.Where(AppAnswers => AppAnswers.GrantId == grantApplications[m].GrantId && AppAnswers.AppUserId == grantApplications[m].AppUserId && AppAnswers.GrantApplicationId == grantApplications[m].Id).ToList();
                    if (grantApplications[m].ApplicantType == "Self")
                    {
                        applicantGrantAnswersFromDb = applicantGrantAnswersFromDb.Where(Answers => Answers.DependentId == null).ToList();
                    }
                    else
                    {
                        applicantGrantAnswersFromDb = applicantGrantAnswersFromDb.Where(Answers => Answers.DependentId != null).ToList();
                    }
                    var approveApp = false;
                    var reason = "";
                    for (int i = 0; i < applicantGrantAnswersFromDb.Count; i++)
                    {
                        for (int j = 0; j < correctAnswersFromDb.Count; j++) 
                        {
                            if (correctAnswersFromDb[j].QuestionId == applicantGrantAnswersFromDb[i].QuestionId)
                            {
                                if (applicantGrantAnswersFromDb[i].Answer.ToLower() == correctAnswersFromDb[j].Answer.ToLower())
                                {
                                    approveApp = true;
                                }
                                reason = correctAnswersFromDb[j].Reason;
                            }
                        }
                    }
                    grantApplications[m].Reason = reason;
                    if (approveApp)
                    {
                        grantApplications[m].Status = ApplicationStatus.Approved;
                    }
                    else
                    {
                        grantApplications[m].Status = ApplicationStatus.Rejected;
                    }
                    _AppDb.TblGrantApplications.Update(grantApplications[m]);
                    await _AppDb.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    TempData["error"] = ex.ToString();
                }
            }
        }

        //private async Task ApproveOrRejectApp(GrantApplications application, IQueryable<ApplicantGrantAnswers> applicantGrantAnswersFromDb, IQueryable<GrantAnswers> correctAnswersFromDb)
        //{
        //    var approveApp = false;
        //    var reason = "";
        //    //foreach (var answer in applicantGrantAnswersFromDb)
        //    //{
        //    //foreach (var correctAnswer in correctAnswersFromDb)
        //    //{
        //    //    if (answer.QuestionId == correctAnswer.QuestionId)
        //    //    {
        //    //        if (answer.Answer == correctAnswer.Answer)
        //    //        {
        //    //            approveApp = true;
        //    //        }
        //    //        reason = correctAnswer.Reason;
        //    //    }
        //    //}
        //    //}
        //    //application.Reason = reason;
        //    //if (approveApp)
        //    //{
        //    //    application.Status = ApplicationStatus.Approved;
        //    //}
        //    //else
        //    //{
        //    //    application.Status = ApplicationStatus.Rejected;
        //    //}
        //    //_AppDb.TblGrantApplications.Update(application);
        //    //await _AppDb.SaveChangesAsync();
        //}

        public async Task<IActionResult> Index()
        {
            try
            {
                var sessionHandler = new SessionHandler();
                var user = await _userManager.GetUserAsync(User);
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
                var applicationsFromDb = _AppDb.TblGrantApplications;
                if (!await applicationsFromDb.AnyAsync())
                {
                    return View();
                }
                ViewData["GrantApplications"] = true;
                AdminDashboard adminDashboard = new()
                {
                    Applications = applicationsFromDb.Take(5),
                    NumberOfApplications = applicationsFromDb.Count(),
                    PendingApplications = applicationsFromDb.Count(x => x.Status == ApplicationStatus.Pending),
                    ApprovedApplications = applicationsFromDb.Count(x => x.Status == ApplicationStatus.Approved),
                    RejectedApplications = applicationsFromDb.Count(x => x.Status == ApplicationStatus.Rejected)
                };
                return View(adminDashboard);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    TempData["error"] = ex.ToString();
                }
                return RedirectToAction("Error");
            }
        }
        public async Task<IActionResult> Profile()
        {
            try
            {
                SessionHandler sessionHandler = new();
                var user = await _userManager.GetUserAsync(User);
                if (user == null || await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, user, Operations.Update);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to update your profile.";
                    return Forbid();
                }
                Profile userProfile = new()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    ImageUrl = user.ImageUrl,
                    IDNumber = user.IDNumber,
                    GenderList = _AppDb.TblGender.Select(x => new SelectListItem() { Text = x.GenderName, Value = x.Id.ToString() }).AsEnumerable(),
                    AddressLine1 = user.AddressLine1,
                    AddressLine2 = user.AddressLine2,
                    City = user.City,
                    Province = user.Province,
                    PostalCode = user.PostalCode
                };
                if (user.Gender == null)
                {
                    userProfile.GenderId = "1";
                }
                else
                {
                    var selectedGender = userProfile.GenderList.FirstOrDefault(x => x.Text == user.Gender);
                    userProfile.GenderId = selectedGender == null ? "1" : selectedGender.Value;
                }
                return View(userProfile);
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
        public async Task<IActionResult> Profile(Profile profile)
        {
            try
            {
                SessionHandler sessionHandler = new();
                var user = await _userManager.GetUserAsync(User);
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
                if (!ModelState.IsValid)
                {
                    return View(profile);
                }
                var dateOfBirth = AgeChecker.DateOfBirth(profile.IDNumber);
                if (AgeChecker.Check(dateOfBirth) < 18)
                {
                    ModelState.AddModelError(string.Empty, "Make sure you have entered a correct ID number as you must be over 18 to create an account.");
                    return View(profile);
                }
                profile.GenderList = _AppDb.TblGender.Select(x => new SelectListItem() { Text = x.GenderName, Value = x.Id.ToString() }).AsEnumerable();
                var selectedGender = profile.GenderList.FirstOrDefault(x => x.Value == profile.GenderId);
                if (selectedGender == null || selectedGender.Value == "1")
                {
                    ModelState.AddModelError(string.Empty, "Please select a gender.");
                    return View(profile);
                }
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (profile.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, profile.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        TempData["success"] = "Unexpected error when trying to set phone number.";
                        return View(profile);
                    }
                }
                if (user.Gender == null)
                {
                    user.Gender = selectedGender.Text;
                }
                else
                {
                    if (selectedGender.Text != user.Gender)
                    {
                        user.Gender = selectedGender.Text;
                    }
                }
                user.FirstName = user.FirstName == profile.FirstName ? user.FirstName : profile.FirstName;
                user.LastName = user.LastName == profile.LastName ? user.LastName : profile.LastName;
                user.PhoneNumber = user.PhoneNumber == profile.PhoneNumber ? user.PhoneNumber : profile.PhoneNumber;
                user.Email = user.Email == profile.Email ? user.Email : profile.Email;
                user.ImageUrl = user.ImageUrl == profile.ImageUrl ? user.ImageUrl : profile.ImageUrl;
                user.IDNumber = user.IDNumber == profile.IDNumber ? user.IDNumber : profile.IDNumber;
                user.AddressLine1 = user.AddressLine1 == profile.AddressLine1 ? user.AddressLine1 : profile.AddressLine1;
                user.AddressLine2 = user.AddressLine2 == profile.AddressLine2 ? user.AddressLine2 : profile.AddressLine2;
                user.PostalCode = user.PostalCode == profile.PostalCode ? user.PostalCode : profile.PostalCode;
                user.Gender = user.Gender == selectedGender.Text ? user.Gender : selectedGender.Text;
                user.City = user.City == profile.City ? user.City : profile.City;
                user.Province = user.Province == profile.Province ? user.Province : profile.Province;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                TempData["success"] = "Your profile has been updated";
                return RedirectToAction("Profile");
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
        //public IActionResult ResetPassword(string? code = null)
        //{
        //    try
        //    {
        //        if (code == null)
        //        {
        //            return BadRequest("A code must be supplied for password reset.");
        //        }
        //        else
        //        {
        //            var resetPassword = new ResetPassword
        //            {
        //                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        //            };
        //            return View(resetPassword);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex != null)
        //        {
        //            ViewData["error"] = ex.ToString();
        //        }
        //        return View();
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return View();
        //        }

        //        var user = await _userManager.FindByEmailAsync(resetPassword.Email);
        //        if (user == null)
        //        {
        //            return RedirectToAction("ForgotPassword", "Account");
        //        }

        //        var result = await _userManager.ResetPasswordAsync(user, resetPassword.Code, resetPassword.Password);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Login", "Account");
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex != null)
        //        {
        //            ViewData["error"] = ex.ToString();
        //        }
        //        return View();
        //    }
        //}
        public async Task<IActionResult> SetPassword()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);

                if (hasPassword)
                {
                    return RedirectToAction("ChangePassword");
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
        public async Task<IActionResult> SetPassword(SetPassword setPassword)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, setPassword.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
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
    }
}
