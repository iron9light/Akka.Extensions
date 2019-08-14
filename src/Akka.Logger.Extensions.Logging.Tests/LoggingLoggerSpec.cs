using System;
using System.Threading.Tasks;

using Akka.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace Akka.Logger.Extensions.Logging.Tests
{
    public class LoggingLoggerSpec
        : TestKit.Xunit2.TestKit
    {
        private static readonly Config Config = @"akka.loglevel = DEBUG
                                                 akka.loggers=[""Akka.Logger.Extensions.Logging.LoggingLogger, Akka.Logger.Extensions.Logging""]";

        public LoggingLoggerSpec(ITestOutputHelper output)
            : base(Config, output: output)
        {
        }

        [Fact]
        public void SmokeTest()
        {
            var services = new ServiceCollection();
            services.AddLogging(logging => logging.AddXUnit(Output));
            var serviceProvider = services.BuildServiceProvider();

            LoggingLogger.LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var logger = Sys.GetLogger();
            logger.Info("Log1 {arg1} {arg2}", 1, "a");
        }

        [Fact]
        public async Task SimpleTest()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock
                .Setup(logger => logger.Log<object>(
                    LogLevel.Information,
                    0,
                    It.IsNotNull<object>(),
                    null,
                    It.IsNotNull<Func<object, Exception, string>>()));

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock
                .Setup(loggerFactory => loggerFactory.CreateLogger(It.IsNotNull<string>()))
                .Returns(loggerMock.Object);
            LoggingLogger.LoggerFactory = loggerFactoryMock.Object;

            var logger = Sys.GetLogger();
            logger.Info("Log1 {arg1} {arg2}", 1, "a");

            await Task.Delay(TimeSpan.FromSeconds(1));

            loggerFactoryMock.Verify(
                loggerFactory => loggerFactory.CreateLogger(It.IsNotNull<string>()),
                Times.Once());
            loggerMock.Verify(
                logger => logger.Log<object>(
                    LogLevel.Information,
                    0,
                    It.IsNotNull<object>(),
                    null,
                    It.IsNotNull<Func<object, Exception, string>>()),
                Times.Once());
        }
    }
}
