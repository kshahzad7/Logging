// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Microsoft.Extensions.Logging.Testing.LoggedFactDiscoverer", "Microsoft.Extensions.Logging.Testing")]
    public class LoggedFactAttribute : FactAttribute, ILoggedTest
    {
        public LoggedFactAttribute() : this(LogLevel.Debug) { }

        public LoggedFactAttribute(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; }
    }
}
