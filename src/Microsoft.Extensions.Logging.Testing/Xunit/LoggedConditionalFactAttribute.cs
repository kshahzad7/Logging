// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Microsoft.Extensions.Logging.Testing.LoggedConditionalFactDiscoverer", "Microsoft.Extensions.Logging.Testing")]
    public class LoggedConditionalFactAttribute : LoggedFactAttribute
    {
    }
}
