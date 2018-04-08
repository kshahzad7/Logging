// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Testing.xunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging.Testing.Tests
{
    public class LoggedTestXunitTests : LoggedTest
    {
        public LoggedTestXunitTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void LoggedFactGetsInitializedLoggerFactory()
        {
            Assert.NotNull(LoggerFactory);
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation("Hello world");
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogDebug("Debug Hello world");
        }

        [Theory]
        [InlineData("Hello world")]
        public void LoggedTheoryGetsInitializedLoggerFactory(string argument)
        {
            Assert.NotNull(LoggerFactory);
            // Use the test argument
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogDebug("Debug Hello world");
        }

        [Fact]
        [LogLevel(LogLevel.Information)]
        public void LoggedFactGetsInitializedLoggerFactory_Information()
        {
            Assert.NotNull(LoggerFactory);
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation("Hello world");
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogDebug("Debug Hello world");
        }

        [Theory]
        [InlineData("Hello world")]
        [LogLevel(LogLevel.Information)]
        public void LoggedTheoryGetsInitializedLoggerFactory_Information(string argument)
        {
            Assert.NotNull(LoggerFactory);
            // Use the test argument
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogDebug("Debug Hello world");
        }

        [Theory]
        [InlineData(null)]
        public void LoggedTheoryNullArgumentsAreEscaped(string argument)
        {
            Assert.NotNull(LoggerFactory);
            Assert.Equal($"{nameof(LoggedTheoryNullArgumentsAreEscaped)}_null", TestMethodTestName);
            // Use the test argument
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
        }

        [ConditionalFact]
        public void ConditionalLoggedFactGetsInitializedLoggerFactory()
        {
            Assert.NotNull(LoggerFactory);
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation("Hello world");
        }

        [ConditionalTheory]
        [InlineData("Hello world")]
        public void ConditionalLoggedTheoryGetsInitializedLoggerFactory(string argument)
        {
            Assert.NotNull(LoggerFactory);
            // Use the test argument
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
        }
    }
}
