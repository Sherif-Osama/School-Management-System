using Microsoft.AspNetCore.Authorization;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;

namespace School.API.Authorization.Handlers
{
    public class StudentOwnershipHandler : AuthorizationHandler<OwnershipRequirement, StudentOwnedResource>
    {
        private readonly IOwnershipService _ownershipService;

        public StudentOwnershipHandler(IOwnershipService ownershipService)
        {
            _ownershipService = ownershipService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, StudentOwnedResource resource)
        {
            string? personIdClaim = context.User.FindFirst(CustomClaimTypes.PersonId)?.Value;

            if (personIdClaim is null || !int.TryParse(personIdClaim, out int currentPersonId))
                return;

            if (await _ownershipService.IsOwnStudentAsync(resource.StudentId, currentPersonId))
                context.Succeed(requirement);
        }
    }
}