using Microsoft.AspNetCore.Authorization;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;

namespace School.API.Authorization.Handlers
{
    public class ParentOwnershipHandler : AuthorizationHandler<OwnershipRequirement, ParentOwnedResource>
    {
        private readonly IOwnershipService _ownershipService;

        public ParentOwnershipHandler(IOwnershipService ownershipService)
        {
            _ownershipService = ownershipService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, ParentOwnedResource resource)
        {
            string? personIdClaim = context.User.FindFirst(CustomClaimTypes.PersonId)?.Value;

            if (personIdClaim is null || !int.TryParse(personIdClaim, out int currentPersonId))
                return;

            if (await _ownershipService.IsOwnParentRecordAsync(resource.ParentId, currentPersonId))
                context.Succeed(requirement);
        }
    }
}