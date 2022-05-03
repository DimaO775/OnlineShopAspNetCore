using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.CustomPolicies
{
    public class AllowUserPolicy : IAuthorizationRequirement
    {
        public string[] AllowedUsers { get; set; }

        public AllowUserPolicy(params string[] users)
        {
            AllowedUsers = users;
        }
    }

    public class AllowUserHandler : AuthorizationHandler<AllowUserPolicy>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowUserPolicy requirement)
        {
            if (requirement.AllowedUsers.Any(user => user.Equals(context.User. Identity.Name, StringComparison.OrdinalIgnoreCase)))
                context.Succeed(requirement);
            else
                context.Fail();
            return Task.CompletedTask;
        }
    }
}
