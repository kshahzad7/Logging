using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing.Xunit
{
    public class XunitLoggingTestCase : XunitTestCase
    {
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public XunitLoggingTestCase() : base()
        {
        }

        public XunitLoggingTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null) : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
        {
        }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            object[] updatedTestMethodArguments;
            if (TestMethodArguments == null)
            {
                updatedTestMethodArguments = new object[1];
            }
            else
            {
                updatedTestMethodArguments = new object[TestMethodArguments.Length + 1];
                Array.Copy(TestMethodArguments, updatedTestMethodArguments, TestMethodArguments.Length);
            }

            var classType = TestMethod.TestClass.Class.ToRuntimeType();
            var testOutputHelper = constructorArguments.Single(a => typeof(ITestOutputHelper).IsAssignableFrom(a.GetType())) as ITestOutputHelper;
            var logLevel = TestMethod.Method.GetCustomAttributes(typeof(LoggingFactAttribute)).Single().GetConstructorArguments().Single();

            using (AssemblyTestLog.ForAssembly(classType.GetTypeInfo().Assembly).StartTestLog(testOutputHelper, classType.FullName, out var loggerFactory, (LogLevel)logLevel, TestMethod.Method.Name))
            {
                updatedTestMethodArguments[updatedTestMethodArguments.Length - 1] = loggerFactory;
                return new XunitTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, updatedTestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();
            }
        }
    }
}
