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
    public class XunitLoggingTestInvoker : XunitTestInvoker
    {
        public XunitLoggingTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        protected override object CreateTestClass()
        {
            var testClass = base.CreateTestClass();

            if (typeof(LoggedTest).IsAssignableFrom(testClass.GetType()))
            {
                var classType = testClass.GetType();
                var testOutputHelper = ConstructorArguments.Single(a => typeof(ITestOutputHelper).IsAssignableFrom(a.GetType())) as ITestOutputHelper;
                var loggingFactAttribute = TestMethod.GetCustomAttribute(typeof(LoggingFactAttribute)) as LoggingFactAttribute;
                AssemblyTestLog.ForAssembly(classType.GetTypeInfo().Assembly).StartTestLog(testOutputHelper, classType.FullName, out var loggerFactory, loggingFactAttribute.LogLevel, TestMethod.Name);
                (testClass as LoggedTest).LoggerFactory = loggerFactory;
            }

            return testClass;
        }
    }
}
