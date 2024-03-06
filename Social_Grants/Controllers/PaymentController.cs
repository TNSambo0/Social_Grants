using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Social_Grants.Data;
using Social_Grants.Models;
using Social_Grants.Models.Account;

namespace Social_Grants.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager; 
        private readonly SignInManager<AppUser> _signInManager;
        private IAuthorizationService _authorizationService;
        public PaymentController(ILogger<PaymentController> logger, AppDbContext AppDb, UserManager<AppUser>
            userManager, SignInManager<AppUser> signInManager, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _AppDb = AppDb;
            _userManager = userManager;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
        }
        public async Task<IActionResult> PaymentMethod()
        {
            try
            {
                SessionHandler sessionHandler = new();
                var user = await _userManager.GetUserAsync(User);
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
                PaymentMethod paymentMethod = new()
                {
                    ChosenPostOffice = new()
                    {
                        Id = 0,
                        AppUserId = user.Id,
                        PostOfficeId = 0,
                        PostOffices = _AppDb.TblPostOffices.Select(x => new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Id.ToString()
                        }),
                        CityId = 0,
                        Cities = _AppDb.TblCities.Select(x => new SelectListItem
                        {
                            Text = x.CityName,
                            Value =
                        x.Id.ToString()
                        }),
                        ProvinceId = 0,
                        Provinces = _AppDb.TblProvinces.Select(x => new SelectListItem
                        {
                            Text = x.ProvinceName,
                            Value = x.Id.ToString()
                        })
                    },
                    BankDetails = new()
                    {
                        Id = 0,
                        AppUserId = user.Id
                    },
                    PaymentMethods = new List<SelectListItem>() { new SelectListItem { Value = "0", Text =
                            "Select  an option" }, new SelectListItem { Value = "1", Text = "Bank account" },
                                new SelectListItem { Value = "2", Text = "Post Office" }
                            }.AsEnumerable(),
                    SelectedPaymentMethodId = 0
                };
                var paymentMethodFromDb = await _AppDb.TblChosenPaymentMethod.FirstOrDefaultAsync(x => x.AppUserId == user.Id);
                if (paymentMethodFromDb == null)
                {
                    return View(paymentMethod);
                }
                if (paymentMethodFromDb.PaymentMethod == "Bank account")
                {
                    var bankDetailsFromDb = await _AppDb.TblBankDetails.FirstOrDefaultAsync(x => x.AppUserId == user.Id);
                    if (bankDetailsFromDb == null)
                    {
                        return NotFound();
                    }
                    paymentMethod.BankDetails.Id = bankDetailsFromDb.Id;
                    paymentMethod.BankDetails.AccountNumber = bankDetailsFromDb.AccountNumber;
                    paymentMethod.BankDetails.BankAccountHolder = bankDetailsFromDb.BankAccountHolder;
                    paymentMethod.BankDetails.BranchCode = bankDetailsFromDb.BranchCode;
                    paymentMethod.BankDetails.BankName = bankDetailsFromDb.BankName;
                    paymentMethod.SelectedPaymentMethodId = 1;
                    return View(paymentMethod);
                }
                else if (paymentMethodFromDb.PaymentMethod == "Post Office")
                {
                    var postOfficeFromDb = await _AppDb.TblChosenPostOffice.FirstOrDefaultAsync(x => x.AppUserId == user.Id);
                    if (postOfficeFromDb == null)
                    {
                        return NotFound();
                    }
                    paymentMethod.ChosenPostOffice.Id = postOfficeFromDb.Id;
                    paymentMethod.ChosenPostOffice.PostOfficeId = postOfficeFromDb.PostOfficeId;
                    paymentMethod.ChosenPostOffice.CityId = postOfficeFromDb.CityId;
                    paymentMethod.ChosenPostOffice.ProvinceId = postOfficeFromDb.ProvinceId;
                    paymentMethod.SelectedPaymentMethodId = 2;
                    return View(paymentMethod);
                }
                else
                    return View(paymentMethod);
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
        public async Task<IActionResult> PaymentMethod(PaymentMethod paymentMethod)
        {
            try
            {
                SessionHandler sessionHandler = new();
                var user = await _userManager.GetUserAsync(User);
                if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
                paymentMethod.ChosenPostOffice.PostOffices = _AppDb.TblPostOffices.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                paymentMethod.ChosenPostOffice.Cities = _AppDb.TblCities.Select(x => new SelectListItem
                {
                    Text = x.CityName,
                    Value =
                        x.Id.ToString()
                });
                paymentMethod.ChosenPostOffice.Provinces = _AppDb.TblProvinces.Select(x => new SelectListItem
                {
                    Text = x.ProvinceName,
                    Value = x.Id.ToString()
                });
                paymentMethod.PaymentMethods = new List<SelectListItem>() { new SelectListItem { Value = "0", Text =
                            "Select  an option" }, new SelectListItem { Value = "1", Text = "Bank account" },
                                new SelectListItem { Value = "2", Text = "Post Office" }}.AsEnumerable();
                var paymentMethodFromDb = await _AppDb.TblChosenPaymentMethod.FirstOrDefaultAsync(x => x.AppUserId
                == user.Id) ?? new() { AppUserId = user.Id, PaymentMethod = String.Empty };
                if (paymentMethod.SelectedPaymentMethodId == 1)
                {
                    //if (String.IsNullOrEmpty(paymentMethod.BankDetails.AccountNumber) ||
                    //    String.IsNullOrEmpty(paymentMethod.BankDetails.BankAccountHolder) ||
                    //    String.IsNullOrEmpty(paymentMethod.BankDetails.BranchCode) ||
                    //    String.IsNullOrEmpty(paymentMethod.BankDetails.BankName))
                    if (paymentMethod.BankDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                        return View(paymentMethod);
                    }
                    if (paymentMethod.BankDetails.Id == 0)
                    {
                        paymentMethodFromDb.PaymentMethod = "Bank account";
                        if (paymentMethodFromDb.Id != 0)
                        {
                            var chosenPO = await _AppDb.TblChosenPostOffice.FirstOrDefaultAsync(x => x.AppUserId
                            == user.Id);
                            if (chosenPO != null)
                                _AppDb.TblChosenPostOffice.Remove(chosenPO);
                            _AppDb.TblChosenPaymentMethod.Update(paymentMethodFromDb);
                        }
                        else
                            await _AppDb.TblChosenPaymentMethod.AddAsync(paymentMethodFromDb);
                        await _AppDb.TblBankDetails.AddAsync(paymentMethod.BankDetails);
                        await _AppDb.SaveChangesAsync();
                        return View(paymentMethod);
                    }
                    var bankDetailsFromDb = await _AppDb.TblBankDetails.ContainsAsync(paymentMethod.BankDetails);
                    if (!bankDetailsFromDb)
                    {
                        return NotFound();
                    }
                    _AppDb.TblBankDetails.Update(paymentMethod.BankDetails);
                    await _AppDb.SaveChangesAsync();
                    return View(paymentMethod);
                }
                else if (paymentMethod.SelectedPaymentMethodId == 2)
                {
                    //if (paymentMethod.ChosenPostOffice.CityId == 0 || paymentMethod.ChosenPostOffice.ProvinceId ==
                    //    0 || paymentMethod.ChosenPostOffice.PostOfficeId == 0)
                    if (paymentMethod.ChosenPostOffice == null)
                    {
                        ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                        return View(paymentMethod);
                    }
                    if (paymentMethod.ChosenPostOffice.Id == 0)
                    {
                        paymentMethodFromDb.PaymentMethod = "Post Office";
                        if (paymentMethodFromDb.Id != 0)
                        {
                            var chosenBA = await _AppDb.TblBankDetails.FirstOrDefaultAsync(x => x.AppUserId
                            == user.Id);
                            if (chosenBA != null)
                                _AppDb.TblBankDetails.Remove(chosenBA);
                            _AppDb.TblChosenPaymentMethod.Update(paymentMethodFromDb);
                        }
                        else
                            await _AppDb.TblChosenPaymentMethod.AddAsync(paymentMethodFromDb);
                        await _AppDb.TblChosenPostOffice.AddAsync(paymentMethod.ChosenPostOffice);
                        await _AppDb.SaveChangesAsync();
                        return View(paymentMethodFromDb);
                    }
                    var postOfficeDetailsFromDb = await _AppDb.TblChosenPostOffice.ContainsAsync(paymentMethod
                    .ChosenPostOffice);
                    if (!postOfficeDetailsFromDb)
                    {
                        return NotFound();
                    }
                    _AppDb.TblChosenPostOffice.Update(paymentMethod.ChosenPostOffice);
                    await _AppDb.SaveChangesAsync();
                    return View(paymentMethodFromDb);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
                    return View(paymentMethod);
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
    }
}
