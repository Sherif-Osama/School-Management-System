using Microsoft.AspNetCore.Authorization;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;

namespace School.API.Authorization.Handlers
{

    public class PersonOwnershipHandler : AuthorizationHandler<OwnershipRequirement, PersonOwnedResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, PersonOwnedResource resource)
        {
            string? personIdClaim = context.User.FindFirst(CustomClaimTypes.PersonId)?.Value;

            if (personIdClaim is not null && int.TryParse(personIdClaim, out int currentPersonId) && currentPersonId == resource.PersonId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}