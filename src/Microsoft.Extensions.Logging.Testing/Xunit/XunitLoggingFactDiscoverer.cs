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

            var testClass = testMethod.TestClass.Class;
            while (testClass != null)
            {
                if (testClass.Name == typeof(LoggedTest).FullName)
                {
                    break;
                }
                testClass = testClass.BaseType;
            }
            if (testClass == null)
            {
                testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods can only be used on methods of a class that inherits LoggedTest.");
            }

            //var trait = testMethod.Method.GetCustomAttributes(typeof(LoggingFactAttribute)).Single().

            var parameters = testMethod.Method.GetParameters();
            if (parameters.Count() == 1 && parameters.Single().ParameterType.Name == typeof(ILoggerFactory).FullName)
            {
                testCase = new XunitTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
            }
            else if (testMethod.Method.GetParameters().Any())
            {
                testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods are not allowed to have parameters other than an ILoggerFactory. Did you mean to use [LoggingTheory]?");
            }
            else if (testMethod.Method.IsGenericMethodDefinition)
            {
                testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods are not allowed to be generic.");
            }
            else
            {
                testCase = CreateTestCase(discoveryOptions, testMethod, factAttribute);
            }

            return new[] { testCase };
        }

    }
}
