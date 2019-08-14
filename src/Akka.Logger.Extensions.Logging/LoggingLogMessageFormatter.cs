using Akka.Event;

using Microsoft.Extensions.Logging.Internal;

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
            => new FormattedLogValues(format, args).ToString();
    }
}
