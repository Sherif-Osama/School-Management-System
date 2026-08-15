using Microsoft.Extensions.Logging;
using School.DTO.LogDTOs;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace School.BLL.Logging
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly Channel<LogEntryDTO> _channel;
        private readonly LogLevel _minLevel;
        private readonly ConcurrentDictionary<string, DatabaseLogger> _loggers = new();

        public DatabaseLoggerProvider(Channel<LogEntryDTO> channel, LogLevel minLevel)
        {
            _channel = channel;
            _minLevel = minLevel;
        }

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new DatabaseLogger(name, _channel, _minLevel));

        public void Dispose() => _loggers.Clear();
    }
}
