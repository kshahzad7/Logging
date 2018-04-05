using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing.Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Microsoft.Extensions.Logging.Testing.Xunit.XunitLoggingFactDiscoverer", "Microsoft.Extensions.Logging.Testing")]
    public class LoggingFactAttribute : FactAttribute
    {
        public LoggingFactAttribute() : this(LogLevel.Debug) { }

        public LoggingFactAttribute(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; }
    }
}
