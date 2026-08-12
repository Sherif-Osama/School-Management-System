using Microsoft.Extensions.Logging;
using School.DTO.LogDTOs;
using System.Threading.Channels;

namespace School.BLL.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _category;
        private readonly Channel<LogEntryDTO> _channel;
        private readonly LogLevel _minLevel;

        public DatabaseLogger(string category, Channel<LogEntryDTO> channel, LogLevel minLevel)
        {
            _category = category;
            _channel = channel;
            _minLevel = minLevel;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => (logLevel >= _minLevel && logLevel != LogLevel.None);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            LogEntryDTO entry = new()
            {
                Level = logLevel.ToString(),
                Category = _category,
                Message = formatter(state, exception),
                Exception = exception?.ToString(),
                CreatedAt = DateTime.Now
            };

            _channel.Writer.TryWrite(entry);
        }
    }
}
