using System;

using Akka.Event;

using FluentAssertions;

using Hocon;

using Xunit;
using Xunit.Abstractions;

namespace Akka.Logger.Extensions.Logging.Tests
{
    public class LoggingLogMessageFormatterSpec
        : TestKit.Xunit2.TestKit
    {
        private static readonly Config Config = @"akka.loglevel = DEBUG";

        public LoggingLogMessageFormatterSpec(ITestOutputHelper output)
            : base(Config, output: output)
        {
        }

        [Theory]
        [InlineData(LogLevel.DebugLevel, "test case {0}", new object[] { 1 })]
        [InlineData(LogLevel.DebugLevel, "test case {myNum}", new object[] { 1 })]
        [InlineData(LogLevel.DebugLevel, "test case {myNum} {myStr}", new object[] { 1, "foo" })]
        public void ShouldHandleSerilogFormats(LogLevel level, string formatStr, object[] args)
        {
            var logger = Sys.GetLogger();

            Sys.EventStream.Subscribe(TestActor, typeof(LogEvent));

            Action logWrite = () =>
            {
                logger.Log(level, formatStr, args);

                var logEvent = ExpectMsg<LogEvent>();
                logEvent.LogLevel().Should().Be(level);
                logEvent.ToString().Should().NotBeEmpty();
            };

            logWrite.Should().NotThrow<FormatException>();
        }
    }
}
