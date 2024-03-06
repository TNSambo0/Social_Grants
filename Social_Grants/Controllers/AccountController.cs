using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Social_Grants.Data;
using Social_Grants.Models;
using Social_Grants.Models.Account;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace Social_Grants.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _AppDb;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        protected IAuthorizationService _authorizationService;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            AppDbContext AppDb, ILogger<AccountController> logger, IEmailSender emailSender, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _AppDb = AppDb;
            _logger = logger;
            _emailSender = emailSender;
            _authorizationService = authorizationService;
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
                    var isAuthorized = await _authorizationService.AuthorizeAsync(User, user, Operations.Update);
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
        public async Task<JsonResult> ChangeProfilePicture()
        {
            var sessionHandler = new SessionHandler();
            var user = await _userManager.GetUserAsync(User);
            ProfilePicStatusModel results = new();
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                results.Status = "Login";
                results.Message = "User autometically logged out due to session end";
                return Json(JsonConvert.SerializeObject(results));
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, user, Operations.Update);
            if (!isAuthorized.Succeeded)
            {
                TempData["error"] = "You don't have the permission to change a profile picture.";
            }
            var file = Request.Form.Files[0];
            //FileSaver.Savefiles(_webHostEnvironment, files, user, user.IDNumber);
            var fileExt = Path.GetExtension(file.FileName);
            string filename = $"{user.IDNumber}_ProfilePicture{fileExt}";
            user.ImageUrl = $"Documents/User/{user.IDNumber}/{filename}";
            await _userManager.UpdateAsync(user);
            results.Status = "success";
            results.Message = user.ImageUrl;
            return Json(JsonConvert.SerializeObject(results));
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
                    if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        return RedirectToAction("Login");
                    }
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { area = "Identity", code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        forgotPassword.Email,
                        "Reset Password",
                        $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError(string.Empty, "Please make sure all the required field are populated with the required data.");
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

        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        HttpContext.Session.SetString("AnnouncementOnce", "true");
                        var user = await _userManager.FindByEmailAsync(login.Email);
                        if (user == null)
                            return View(login);
                        var userRole = await _userManager.IsInRoleAsync(user, Constants.CustomersRole) ? Constants.CustomersRole : Constants.AdministratorsRole;
                        if (userRole == Constants.AdministratorsRole)
                            return RedirectToAction("Index", "Admin");
                        else
                            return RedirectToAction("Index", "Home");

                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction("LoginWith2fa");
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToAction("Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(login);
                    }
                }
                ModelState.AddModelError(string.Empty, "Please make sure all the required field are populated with the required data.");
                return View(login);
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
        public async Task<IActionResult> Logout()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> Profile()
        {
            try
            {
                SessionHandler sessionHandler = new();
                var user = await _userManager.GetUserAsync(User);
                if (user == null || await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); }
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, user, Operations.Update);
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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dateOfBirth = AgeChecker.DateOfBirth(register.IDNumber);
                    if (AgeChecker.Check(dateOfBirth) >= 18)
                    {
                        ModelState.AddModelError(string.Empty, "You must be over 18 to create an account.");
                    }
                    var user = new AppUser
                    {
                        UserName = register.Email,
                        Email = register.Email,
                        FirstName = register.FirstName,
                        LastName = register.LastName,
                        IDNumber = register.IDNumber,
                        ImageUrl = "images/User/images.png",
                    };
                    var result = await _userManager.CreateAsync(user, register.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, Constants.CustomersRole);
                        _logger.LogInformation("User created a new account with password.");
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Login");
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(register);
                }
                ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                return View(register);
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

        public IActionResult ResetPassword(string? code = null)
        {
            try
            {
                if (code == null)
                {
                    return BadRequest("A code must be supplied for password reset.");
                }
                else
                {
                    var resetPassword = new ResetPassword
                    {
                        Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                    };
                    return View(resetPassword);
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
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var user = await _userManager.FindByEmailAsync(resetPassword.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPassword");
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPassword.Code, resetPassword.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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
