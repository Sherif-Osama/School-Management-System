using Microsoft.AspNetCore.Authorization;
using School.API.Authorization.Handlers;
using School.API.Authorization.Policies;

namespace School.API.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            //this to register the custom authorization policy provider
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            services.AddScoped<IAuthorizationHandler, StudentOwnershipHandler>();
            services.AddScoped<IAuthorizationHandler, ParentOwnershipHandler>();
            services.AddSingleton<IAuthorizationHandler, PersonOwnershipHandler>();
            services.AddAuthorization();
            return services;
        }
    }
}