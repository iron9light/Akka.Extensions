using System;
using System.Diagnostics.CodeAnalysis;

using Akka.Event;

using Microsoft.Extensions.Logging;

namespace Akka.Logger.Extensions.Logging
{
    /// <summary>
    /// This class contains methods used to convert <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// templated messages into normal text messages.
    /// </summary>
    public class LoggingLogMessageFormatter
        : ILogMessageFormatter
    {
        public static readonly LoggingLogMessageFormatter Instance = new LoggingLogMessageFormatter();

        private LoggingLogMessageFormatter()
        {
        }

        /// <inheritdoc />
        public string Format(string format, params object[] args)
        {
            var formatterLogger = new FormatterLogger();
            formatterLogger.Log(Microsoft.Extensions.Logging.LogLevel.None, format, args);
            return formatterLogger.ToString();
        }

        private class FormatterLogger : ILogger
        {
            private string _formattedMessage = string.Empty;

            [ExcludeFromCodeCoverage]
            public IDisposable BeginScope<TState>(TState state)
            {
                throw new InvalidOperationException();
            }

            [ExcludeFromCodeCoverage]
            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                throw new InvalidOperationException();
            }

            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _formattedMessage = formatter(state, exception);
            }

            public override string ToString()
            {
                return _formattedMessage;
            }
        }
    }
}
