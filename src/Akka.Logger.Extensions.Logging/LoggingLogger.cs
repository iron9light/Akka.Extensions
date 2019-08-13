using System.Collections.Generic;

using Akka.Actor;
using Akka.Dispatch;
using Akka.Event;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Akka.Logger.Extensions.Logging
{
    public class LoggingLogger
        : UntypedActor, IRequiresMessageQueue<ILoggerMessageQueueSemantics>
    {
        public static ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Error errorLogEvent:
                    Log(errorLogEvent);
                    break;
                case Warning warningLogEvent:
                    Log(warningLogEvent);
                    break;
                case Info infologEvent:
                    Log(infologEvent);
                    break;
                case Debug debugLogEvent:
                    Log(debugLogEvent);
                    break;
                case InitializeLogger _:
                    Sender.Tell(new LoggerInitialized());
                    break;
            }
        }

        private static (string Format, object[] Args) GetFormat(LogEvent logEvent)
        {
            string format = "{Timestamp} [{LogSource}] ({Thread})";
            var args = new List<object>
            {
                logEvent.Timestamp,
                logEvent.LogSource,
                logEvent.Thread.ManagedThreadId.ToString().PadLeft(4, '0'),
            };

            if (logEvent.Message is LogMessage logMessage)
            {
                if (logMessage.Format != null)
                {
                    format = $"{format} {logMessage.Format}";
                }

                if (logMessage.Args != null)
                {
                    args.AddRange(logMessage.Args);
                }
            }
            else if (logEvent.Message != null)
            {
                format = $"{format} {{Message}}";
                args.Add(logEvent.Message);
            }

            return (format, args.ToArray());
        }

        private void Log(Error logEvent)
        {
            var (format, args) = GetFormat(logEvent);
            GetLogger(logEvent).LogError(logEvent.Cause, format, args);
        }

        private void Log(Warning logEvent)
        {
            var (format, args) = GetFormat(logEvent);
            GetLogger(logEvent).LogWarning(logEvent.Cause, format, args);
        }

        private void Log(Info logEvent)
        {
            var (format, args) = GetFormat(logEvent);
            GetLogger(logEvent).LogInformation(logEvent.Cause, format, args);
        }

        private void Log(Debug logEvent)
        {
            var (format, args) = GetFormat(logEvent);
            GetLogger(logEvent).LogDebug(logEvent.Cause, format, args);
        }

        private ILogger GetLogger(LogEvent logEvent)
        {
            return LoggerFactory.CreateLogger(logEvent.LogClass);
        }
    }
}
