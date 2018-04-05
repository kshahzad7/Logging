using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing.Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Microsoft.Extensions.Logging.Testing.Xunit.XunitLoggingTheoryDiscoverer", "Microsoft.Extensions.Logging.Testing")]
    public class LoggingTheoryAttribute : TheoryAttribute
    {
        public LoggingTheoryAttribute() : this(LogLevel.Debug) { }

        public LoggingTheoryAttribute(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; }
    }
}
