using Microsoft.AspNetCore.Authorization;
using School.API.Authorization;

namespace School.API.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            services.AddAuthorization();

            return services;
        }
    }
}