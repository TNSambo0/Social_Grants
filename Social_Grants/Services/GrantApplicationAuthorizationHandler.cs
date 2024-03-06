using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Social_Grants.Data;
using Social_Grants.Models.Account;
using Social_Grants.Models.Grant;

namespace Social_Grants.Services
{
    public class GrantApplicationAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, GrantApplications>
    {
        UserManager<AppUser> _userManager;
        public GrantApplicationAuthorizationHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, GrantApplications grantApplicationsResource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            // If not asking for Create or Read permission, return.
            if (requirement.Name != Constants.CreateOperationName && requirement.Name != Constants.ReadOperationName)
            {
                return Task.CompletedTask;
            }
            var isInRole = await _userManager.IsInRoleAsync(grantApplicationsResource.AppUser, Constants.CustomersRole);
            if (grantApplicationsResource.AppUserId == _userManager.GetUserId(context.User) &&
               isInRole)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
