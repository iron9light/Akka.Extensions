using System;

using Akka.Event;

namespace Akka.Logger.Extensions.Logging
{
    public class LoggingLoggingAdapter
        : BusLogging
    {
        public LoggingLoggingAdapter(LoggingBus bus, string logSource, Type logClass)
            : base(bus, logSource, logClass, LoggingLogMessageFormatter.Instance)
        {
        }
    }
}
