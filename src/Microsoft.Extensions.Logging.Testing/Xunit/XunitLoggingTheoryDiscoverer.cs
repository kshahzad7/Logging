using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.Extensions.Logging.Testing.Xunit
{
    class XunitLoggingTheoryDiscoverer : TheoryDiscoverer
    {
        public XunitLoggingTheoryDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        public virtual IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        {
            //IXunitTestCase testCase;

            //var testClass = testMethod.TestClass.Class;
            //var parameters = testMethod.Method.GetParameters();
            //while (testClass != null)
            //{
            //    if (testClass.Name == typeof(LoggedTest).FullName)
            //    {
            //        break;
            //    }
            //    testClass = testClass.BaseType;
            //}
            //if (testClass == null)
            //{
            //    testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods can only be used on methods of a class that inherits LoggedTest.");
            //}
            //else if (parameters.Count() == 1 && parameters.Single().ParameterType.Name == typeof(ILoggerFactory).FullName)
            //{
            //    testCase = new XunitLoggingTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
            //}
            //else if (testMethod.Method.GetParameters().Any())
            //{
            //    testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods are not allowed to have parameters other than an ILoggerFactory. Did you mean to use [LoggingTheory]?");
            //}
            //else if (testMethod.Method.IsGenericMethodDefinition)
            //{
            //    testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, "[LoggingFact] methods are not allowed to be generic.");
            //}
            //else
            //{
            //    testCase = CreateTestCase(discoveryOptions, testMethod, factAttribute);
            //}

            //return new[] { testCase };

            // Special case Skip, because we want a single Skip (not one per data item); plus, a skipped test may
            // not actually have any data (which is quasi-legal, since it's skipped).
            var skipReason = theoryAttribute.GetNamedArgument<string>("Skip");
            if (skipReason != null)
            {
                return CreateTestCasesForSkip(discoveryOptions, testMethod, theoryAttribute, skipReason);
            }

            if (discoveryOptions.PreEnumerateTheoriesOrDefault())
            {
                try
                {
                    var dataAttributes = testMethod.Method.GetCustomAttributes(typeof(DataAttribute));
                    var results = new List<IXunitTestCase>();

                    foreach (var dataAttribute in dataAttributes)
                    {
                        var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                        IDataDiscoverer discoverer;
                        try
                        {
                            discoverer = ExtensibilityPointFactory.GetDataDiscoverer(DiagnosticMessageSink, discovererAttribute);
                        }
                        catch (InvalidCastException)
                        {
                            var reflectionAttribute = dataAttribute as IReflectionAttributeInfo;

                            if (reflectionAttribute != null)
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        testMethod,
                                        $"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not implement IDataDiscoverer."
                                    )
                                );
                            else
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        testMethod,
                                        $"A data discoverer specified on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not implement IDataDiscoverer."
                                    )
                                );

                            continue;
                        }

                        if (discoverer == null)
                        {
                            var reflectionAttribute = dataAttribute as IReflectionAttributeInfo;

                            if (reflectionAttribute != null)
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        testMethod,
                                        $"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not exist."
                                    )
                                );
                            else
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        testMethod,
                                        $"A data discoverer specified on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not exist."
                                    )
                                );

                            continue;
                        }

                        skipReason = dataAttribute.GetNamedArgument<string>("Skip");

                        if (!discoverer.SupportsDiscoveryEnumeration(dataAttribute, testMethod.Method))
                            return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);

                        var data = discoverer.GetData(dataAttribute, testMethod.Method);
                        if (data == null)
                        {
                            results.Add(
                                new ExecutionErrorTestCase(
                                    DiagnosticMessageSink,
                                    discoveryOptions.MethodDisplayOrDefault(),
                                    testMethod,
                                    $"Test data returned null for {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}. Make sure it is statically initialized before this test method is called."
                                )
                            );

                            continue;
                        }

                        foreach (var dataRow in data)
                        {
                            // Determine whether we can serialize the test case, since we need a way to uniquely
                            // identify a test and serialization is the best way to do that. If it's not serializable,
                            // this will throw and we will fall back to a single theory test case that gets its data at runtime.
                            if (!XunitSerializationInfo.CanSerializeObject(dataRow))
                            {
                                DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Non-serializable data ('{dataRow.GetType().FullName}') found for '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case."));
                                return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
                            }

                            var testCases =
                                skipReason != null
                                    ? CreateTestCasesForSkippedDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, skipReason)
                                    : CreateTestCasesForDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow);

                            results.AddRange(testCases);
                        }
                    }

                    if (results.Count == 0)
                        results.Add(new ExecutionErrorTestCase(DiagnosticMessageSink,
                                                               discoveryOptions.MethodDisplayOrDefault(),
                                                               testMethod,
                                                               $"No data found for {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}"));

                    return results;
                }
                catch (Exception ex)    // If something goes wrong, fall through to return just the XunitTestCase
                {
                    DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Exception thrown during theory discovery on '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case.{Environment.NewLine}{ex}"));
                }
            }

            return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        }

        private bool IsSerializable(object [] arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument.GetType().IsArray && !IsSerializable(argument as object[]))
                {
                    return false;
                }
                else if (!argument.GetType().IsSerializable)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
