using Microsoft.AspNetCore.Mvc;
using Social_Grants.Data;
using Social_Grants.Models;
using System.Diagnostics;

namespace Social_Grants.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _AppDb;

        public HomeController(ILogger<HomeController> logger, AppDbContext AppDb)
        {
            _logger = logger; 
            _AppDb = AppDb;
        }

        public IActionResult Index()
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

        public IActionResult About()
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
        public IActionResult SocialGrants()
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
        public IActionResult Contacts()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}