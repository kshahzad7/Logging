// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Microsoft.Extensions.Logging.Testing.LoggedTheoryDiscoverer", "Microsoft.Extensions.Logging.Testing")]
    public class LoggedTheoryAttribute : TheoryAttribute, ILoggedTest
    {
        public LoggedTheoryAttribute() : this(LogLevel.Debug) { }

        public LoggedTheoryAttribute(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; }
    }
}
