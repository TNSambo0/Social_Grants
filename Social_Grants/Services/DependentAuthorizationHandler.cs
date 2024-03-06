using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Social_Grants.Data;
using Social_Grants.Models;
using Social_Grants.Models.Account;

namespace Social_Grants.Services
{
    public class DependentAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Dependent>
    {
        UserManager<AppUser> _userManager;
        public DependentAuthorizationHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Dependent dependentResource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            // If not asking for CRUD permission, return.
            if (requirement.Name != Constants.ReadOperationName && requirement.Name != Constants.UpdateOperationName &&
            requirement.Name != Constants.DeleteOperationName && requirement.Name != Constants.CreateOperationName)
            {
                return Task.CompletedTask;
            }
            var isInRole = await _userManager.IsInRoleAsync(dependentResource.AppUser, Constants.CustomersRole);
            if (dependentResource.AppUserId == _userManager.GetUserId(context.User) && isInRole)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
