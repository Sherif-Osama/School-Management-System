using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using School.DTO.LogDTOs;
using System.Threading.Channels;

namespace School.BLL.Logging
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddDatabaseLogging(this IServiceCollection services)
        {
            Channel<LogEntryDTO> channel = Channel.CreateBounded<LogEntryDTO>(new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            });

            services.AddSingleton(channel);
            services.AddHostedService<LogBackgroundService>();

            return services;
        }

        public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, IConfiguration configuration)
        {
            LogLevel minLevel = Enum.TryParse(configuration["Logging:Database:MinLevel"], out LogLevel parsed)
                ? parsed : LogLevel.Warning;

            builder.Services.AddSingleton<ILoggerProvider>(sp =>
                new DatabaseLoggerProvider(sp.GetRequiredService<Channel<LogEntryDTO>>(), minLevel));

            return builder;
        }
    }
}
