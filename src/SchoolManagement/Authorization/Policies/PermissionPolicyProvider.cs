using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using School.BLL.Authentication;

namespace School.API.Authorization.Policies
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var builder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();

            if (policyName.EndsWith(".View.Own", StringComparison.OrdinalIgnoreCase))
            {
                // "Students.View.Own" -> "Students.View"
                string basePermission = policyName[..^".Own".Length];

                builder.RequireAssertion(ctx =>
                    ctx.User.HasClaim(CustomClaimTypes.Permission, $"{basePermission}.All") ||
                    ctx.User.HasClaim(CustomClaimTypes.Permission, $"{basePermission}.Own"));
            }
            else
            {
                builder.RequireClaim(CustomClaimTypes.Permission, policyName);
            }

            return Task.FromResult<AuthorizationPolicy?>(builder.Build());
        }
    }
}
