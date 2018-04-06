using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing.Xunit
{
    class XunitLoggingFactDiscoverer : FactDiscoverer
    {
        public XunitLoggingFactDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        public override IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            IXunitTestCase testCase;
            if (testMethod.Method.GetParameters().Any())
            {
                testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods are not allowed to have parameters other than an ILoggerFactory. Did you mean to use [LoggingTheory]?");
            }
            else if (testMethod.Method.IsGenericMethodDefinition)
            {
                testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods are not allowed to be generic.");
            }
            else
            {
                testCase = new XunitLoggingTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
            }

            return new[] { testCase };
        }

    }
}
