using Microsoft.AspNetCore.Identity;
using Social_Grants.Models.Account;

namespace Social_Grants.Data
{
    public class SessionHandler
    {
        public async Task<bool> GetSession(HttpContext context, SignInManager<AppUser> _signInManager, ILogger _logger)
        {
            var session = context.Session.GetString("AnnouncementOnce");
            if (String.IsNullOrEmpty(session))
            {
                context.Session.Remove("AnnouncementOnce");
                context.Session.Clear();
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out due to session end.");
                return true;
            }
            return false;
        }
        public async Task SignUserOut(SignInManager<AppUser> _signInManager, ILogger _logger)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out due to session end.");
        }
    }
}
