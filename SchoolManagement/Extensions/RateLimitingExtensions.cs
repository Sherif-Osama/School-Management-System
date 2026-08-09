using System.Threading.RateLimiting;

namespace School.API.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter =
                    PartitionedRateLimiter.Create<HttpContext, string>
                    (
                        context => RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                                _ => new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit = 100,
                                    Window = TimeSpan.FromMinutes(1),
                                    QueueLimit = 0
                                }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}