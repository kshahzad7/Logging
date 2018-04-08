// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging.Testing.Tests
{
    public class LoggedTestXunitTests : LoggedTest
    {
        public LoggedTestXunitTests(ITestOutputHelper output) : base(output)
        {
        }

        [LoggedFact]
        public void LoggedFactGetsInitializedLoggerFactory()
        {
            Assert.NotNull(LoggerFactory);
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation("Hello world");
        }

        [LoggedTheory]
        [InlineData("Hello world")]
        public void LoggedTheoryGetsInitializedLoggerFactory(string argument)
        {
            Assert.NotNull(LoggerFactory);
            // Use the test argument
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
        }

        [LoggedTheory]
        [InlineData(null)]
        public void LoggedTheoryNullArgumentsAreEscaped(string argument)
        {
            Assert.NotNull(LoggerFactory);
            Assert.Equal($"{nameof(LoggedTheoryNullArgumentsAreEscaped)}_null", TestMethodTestName);
            // Use the test argument
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
        }

        [LoggedConditionalFact]
        public void ConditionalLoggedFactGetsInitializedLoggerFactory()
        {
            Assert.NotNull(LoggerFactory);
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation("Hello world");
        }

        [LoggedConditionalTheory]
        [InlineData("Hello world")]
        public void ConditionalLoggedTheoryGetsInitializedLoggerFactory(string argument)
        {
            Assert.NotNull(LoggerFactory);
            // Use the test argument
            LoggerFactory.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
        }

        [Fact]
        public void FactDoesNotGetInitializedLoggerFactory()
        {
            Assert.Null(LoggerFactory);
        }

        [Theory]
        [InlineData("Hello world")]
        public void TheoryDoesNotGetInitializedLoggerFactory(string argument)
        {
            Assert.Null(LoggerFactory);
            // Use the test argument
            LoggerFactory?.CreateLogger(nameof(AssemblyTestLogTests)).LogInformation(argument);
        }
    }
}
