using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing.Xunit
{
    public class XunitLoggingTestMethodRunner : XunitTestMethodRunner
    {
        private IMessageSink DiagnosticMessageSink { get; }
        private object[] ConstructorArguments { get; }

        public XunitLoggingTestMethodRunner(ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments) : base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)
        {
            DiagnosticMessageSink = diagnosticMessageSink;
            ConstructorArguments = constructorArguments;
        }

        protected override Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
        {
            //throw new Exception($"Test case has {testCase.TestMethodArguments.Length} arguments");
            return testCase.RunAsync(DiagnosticMessageSink, MessageBus, ConstructorArguments, new ExceptionAggregator(Aggregator), CancellationTokenSource);
        }
    }
}
