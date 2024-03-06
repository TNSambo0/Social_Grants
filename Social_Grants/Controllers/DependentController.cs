using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_Grants.Data;
using Social_Grants.Models;
using Social_Grants.Models.Account;
using Social_Grants.Services.Abstract;
using Social_Grants.Services.Concrete;

namespace Social_Grants.Controllers
{
    public class DependentController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _AppDb;
        private readonly ILogger<DependentController> _logger;
        //private readonly IDocumentService _iDocumentService;
        protected IAuthorizationService _authorizationService;
        public DependentController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            AppDbContext AppDb,/* IDocumentService iDocumentService,*/ ILogger<DependentController> logger, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _AppDb = AppDb;
            _logger = logger;
            //_iDocumentService = iDocumentService;
            _authorizationService = authorizationService;
        }
        public async Task<IActionResult> Create()
        {
            SessionHandler sessionHandler = new();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); }
            Dependent dependent = new() { AppUserId = user.Id, AppUser = user };
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, dependent, Operations.Create);
            if (!isAuthorized.Succeeded)
            {
                TempData["error"] = "You don't have the permission to create a dependent.";
                return RedirectToAction("Index");
            }
            return View(dependent);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dependent aDependent)
        {
            SessionHandler sessionHandler = new();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); };
            if (ModelState.IsValid && aDependent.IdentityDocument != null)
            {
                try
                {
                    var dependentFromDb = await _AppDb.TblDependent.FirstOrDefaultAsync(x => x.IDNumber == aDependent.IDNumber);
                    if (dependentFromDb == null)
                    {
                        Dependent dependent = new()
                        {
                            FullName = aDependent.FullName,
                            LastName = aDependent.LastName,
                            IDNumber = aDependent.IDNumber,
                            Nationality = aDependent.Nationality,
                            AppUserId = user.Id,
                            //IdentityDocumentUrl = _iDocumentService.UploadDocumentToAzure(aDependent.IdentityDocument)
                        };
                        await _AppDb.TblDependent.AddAsync(dependent);
                        await _AppDb.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "A dependent with the Id number already exists.");
                        return View(aDependent);
                    }
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = ex;
                }
            }
            ModelState.AddModelError(string.Empty, "Please fill all the required fields.");
            return View(aDependent);
        }
        public async Task<IActionResult> Edit(int id)
        {
            SessionHandler sessionHandler = new();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); };
            var dependent = await _AppDb.TblDependent.FindAsync(id);
            if (dependent == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, dependent, Operations.Update);
            if (!isAuthorized.Succeeded)
            {
                TempData["error"] = "You don't have the permission to edit a dependent.";
                return RedirectToAction("Index");
            }
            return View(dependent);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dependent dependent)
        {
            SessionHandler sessionHandler = new();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); };
            if (ModelState.IsValid)
            {
                var dependentFromDb = await _AppDb.TblDependent.ContainsAsync(dependent);
                if (!dependentFromDb)
                {
                    return NotFound();
                }
                if (dependent.IdentityDocument != null)
                {
                   // _iDocumentService.DeleteDocumentFromAzure(dependent.IdentityDocumentUrl);
                   // dependent.IdentityDocumentUrl = _iDocumentService.UploadDocumentToAzure(dependent.IdentityDocument);
                }
                _AppDb.TblDependent.Update(dependent);
                await _AppDb.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Fill all the required fields.");
            return View(dependent);
        }
        public async Task<IActionResult> Index()
        {
            SessionHandler sessionHandler = new();
            var user = await _userManager.GetUserAsync(User);
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger) || user == null) { return RedirectToAction("Login", "Account"); };
            Dependent dependent = new() { AppUserId = user.Id, AppUser = user };
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, dependent, Operations.Read);
            if (!isAuthorized.Succeeded)
            {
                TempData["error"] = "You don't have the permission to view dependents.";
                return RedirectToAction("Index", "Home");
            }
            var dependents = from aDependent in _AppDb.TblDependent where aDependent.AppUserId == user.Id select aDependent;
            if (!await dependents.AnyAsync())
            {
                return View();
            }
            ViewData["Dependents"] = true;
            return View(dependents.AsEnumerable());
        }
    }
}
