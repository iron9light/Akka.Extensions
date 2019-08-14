using Akka.Actor;
using Akka.Event;

using Microsoft;

namespace Akka.Logger.Extensions.Logging
{
    public static class LoggingLoggingAdapterExtensions
    {
        /// <summary>
        /// Creates a new logging adapter using the specified context's event stream.
        /// </summary>
        /// <param name="context">The context used to configure the logging adapter.</param>
        /// <returns>The newly created logging adapter.</returns>
        public static ILoggingAdapter GetLogger(this IActorContext context)
        {
            Requires.NotNull(context, nameof(context));

            var logSource = context.Self.ToString();
            var logClass = context.Props.Type;

            return new LoggingLoggingAdapter(context.System.EventStream, logSource, logClass);
        }

        /// <summary>
        /// Creates a new logging adapter using the specified system's event stream.
        /// </summary>
        /// <param name="system">The system used to configure the logging adapter.</param>
        /// <returns>The newly created logging adapter.</returns>
        public static ILoggingAdapter GetLogger(this ActorSystem system)
        {
            Requires.NotNull(system, nameof(system));

            var logSource = $"{nameof(ActorSystem)}({system.Name})";
            var logClass = system.GetType();

            return new LoggingLoggingAdapter(system.EventStream, logSource, logClass);
        }
    }
}
