    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Social_Grants.Data;
using Social_Grants.Models.Grant;

namespace Social_Grants.Services
{
    public class AdministratorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, GrantApplications>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, GrantApplications grantResource)
        {
            if (context.User == null || grantResource == null)
            {
                return Task.CompletedTask;
            }
            // If not asking for reading/approval/reject, return.
            if
            (requirement.Name != Constants.ApproveOperationName && 
            requirement.Name != Constants.ReadOperationName)
            {
                return
                Task.CompletedTask;
            }
            // Administrator can approve or reject.
            if (context.User.IsInRole(Constants.AdministratorsRole))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}