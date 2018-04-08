// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing
{
    internal class LoggedTestInvoker : XunitTestInvoker
    {
        public LoggedTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        protected override object CreateTestClass()
        {
            var testClass = base.CreateTestClass();

            if (testClass is LoggedTest loggedTestClass)
            {
                var classType = loggedTestClass.GetType();
                var testOutputHelper = ConstructorArguments.Single(a => typeof(ITestOutputHelper).IsAssignableFrom(a.GetType())) as ITestOutputHelper;
                var loggedTest = TestMethod.GetCustomAttribute(typeof(FactAttribute)) as ILoggedTest;
                var testName = TestMethodArguments.Aggregate(TestMethod.Name, (a, b) => $"{a}_{(b ?? "null")}");

                AssemblyTestLog.ForAssembly(classType.GetTypeInfo().Assembly).StartTestLog(testOutputHelper, classType.FullName, out var loggerFactory, loggedTest.LogLevel, out var resolvedTestName, testName);
                loggedTestClass.LoggerFactory = loggerFactory;
                loggedTestClass.TestMethodTestName = resolvedTestName;
            }

            return testClass;
        }
    }
}
