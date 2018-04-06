using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing.Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Microsoft.Extensions.Logging.Testing.Xunit.XunitLoggingTheoryDiscoverer", "Microsoft.Extensions.Logging.Testing")]
    public class LoggingTheoryAttribute : LoggingFactAttribute
    {
        public LoggingTheoryAttribute() : base() { }

        public LoggingTheoryAttribute(LogLevel logLevel) : base(logLevel) { }
    }
}
