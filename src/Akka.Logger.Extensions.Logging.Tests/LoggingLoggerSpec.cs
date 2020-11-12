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
#pragma warning disable S2699 // Tests should include assertions
        public void SmokeTest()
#pragma warning restore S2699 // Tests should include assertions
        {
            var services = new ServiceCollection();
            services.AddLogging(logging => logging.AddXUnit(Output));
            using (var serviceProvider = services.BuildServiceProvider())
            {
                LoggingLogger.LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                var logger = Sys.GetLogger();
                logger.Debug("Log1 {arg1} {arg2}", 1, "a");
                logger.Info("Log2 {arg1} {arg2}", 2, "b");
                logger.Warning("Log3 {arg1} {arg2}", 3, "c");
                logger.Error(new Exception("test"), "Log4 {arg1} {arg2}", 4, "d");
            }
        }

        [Fact]
        public async Task SimpleTest()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock
                .Setup(logger => logger.Log(
                    LogLevel.Information,
                    0,
                    It.IsNotNull<It.IsAnyType>(),
                    null,
                    It.IsNotNull<Func<It.IsAnyType, Exception, string>>()
                    ));

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
                Times.AtLeastOnce());
            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    0,
                    It.IsNotNull<It.IsAnyType>(),
                    null,
                    It.IsNotNull<Func<It.IsAnyType, Exception, string>>()
                    ),
                Times.Once());
        }
    }
}
