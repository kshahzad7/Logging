// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing
{
    internal class LoggedTestFramework : XunitTestFramework
    {
        public LoggedTestFramework(IMessageSink messageSink) : base(messageSink)
        {
        }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new LoggedTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }
    }
}
