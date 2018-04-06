// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing
{
    public class LoggedTestInvoker : XunitTestInvoker
    {
        public LoggedTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        protected override object CreateTestClass()
        {
            var testClass = base.CreateTestClass();

            if (typeof(LoggedTest).IsAssignableFrom(testClass.GetType()))
            {
                var classType = testClass.GetType();
                var testOutputHelper = ConstructorArguments.Single(a => typeof(ITestOutputHelper).IsAssignableFrom(a.GetType())) as ITestOutputHelper;
                var loggingFactAttribute = TestMethod.GetCustomAttribute(typeof(LoggedFactAttribute)) as LoggedFactAttribute;
                var testName = TestMethodArguments.Aggregate(TestMethod.Name, (a, b) => $"{a}_{b}");

                AssemblyTestLog.ForAssembly(classType.GetTypeInfo().Assembly).StartTestLog(testOutputHelper, classType.FullName, out var loggerFactory, loggingFactAttribute.LogLevel, testName);
                (testClass as LoggedTest).LoggerFactory = loggerFactory;
            }

            return testClass;
        }
    }
}
