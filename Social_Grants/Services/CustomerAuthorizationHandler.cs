using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Social_Grants.Data;
using Social_Grants.Models.Account;

namespace Social_Grants.Services
{
    public class CustomerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, AppUser>
    {
        UserManager<AppUser> _userManager;
        public CustomerAuthorizationHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, AppUser appUserResource)
        {
            if (context.User == null || appUserResource == null)
            {
                return Task.CompletedTask;
            }
            // If not asking for read and update permission, return.
            if (requirement.Name != Constants.ReadOperationName &&
            requirement.Name != Constants.UpdateOperationName)
            {
                return Task.CompletedTask;
            }
            if
            (appUserResource.Id == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
