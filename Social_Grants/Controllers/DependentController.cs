using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Social_Grants.Data;
using Social_Grants.Models.Account;
using Social_Grants.Models;
using Microsoft.EntityFrameworkCore;

namespace Social_Grants.Controllers
{
    public class DependentController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _AppDb; 
        private readonly ILogger<DependentController> _logger;
        public DependentController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            AppDbContext AppDb, ILogger<DependentController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _AppDb = AppDb;
            _logger = logger;
        }
        public async Task<IActionResult> Edit(int id)
        {
            SessionHandler sessionHandler = new();
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var dependent = await _AppDb.TblDependent.FindAsync(id);
            if(dependent == null)
            {
                return NotFound();
            }
            return View(dependent);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dependent dependent)
        {
            SessionHandler sessionHandler = new();
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (ModelState.IsValid)
            {
                var dependentFromDb = await _AppDb.TblDependent.ContainsAsync(dependent);
                if (dependentFromDb == null)
                {
                    return NotFound();
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
            if (await sessionHandler.GetSession(HttpContext, _signInManager, _logger)) { return RedirectToAction("Login", "Account"); };
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); 
            }
            var dependents = from aDependent in _AppDb.TblDependent where aDependent.AppUserId == user.Id select aDependent;
            if(!await dependents.AnyAsync())
            {
                return View();
            } 
            ViewData["Dependents"] = true;
            return View(dependents.AsEnumerable());
        }
    }
}
