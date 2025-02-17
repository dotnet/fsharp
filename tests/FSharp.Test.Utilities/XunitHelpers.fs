#if XUNIT_EXTRAS
#nowarn "0044"
#endif

namespace FSharp.Test

open System
open Xunit.Sdk
open Xunit.Abstractions

open TestFramework

open FSharp.Compiler.Diagnostics

open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

/// Disables custom internal parallelization added with XUNIT_EXTRAS.
/// Execute test cases in a class or a module one by one instead of all at once. Allow other collections to run simultaneously.
[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method, AllowMultiple = false)>]
type RunTestCasesInSequenceAttribute() = inherit Attribute()

#if !XUNIT_EXTRAS
/// Installs console support for parallel test runs and conditionally enables optional xUnit customizations.
type FSharpXunitFramework(sink: IMessageSink) =
    inherit XunitTestFramework(sink)
    do
        // Because xUnit v2 lacks assembly fixture, the next best place to ensure things get called
        // right at the start of the test run is here in the constructor.
        // This gets executed once per test assembly.
        logConfig initialConfig
        log "FSharpXunitFramework installing TestConsole redirection"
        TestConsole.install()
#if !NETCOREAPP
        AssemblyResolver.addResolver ()
#endif

    interface IDisposable with
        member _.Dispose() =
            match Environment.GetEnvironmentVariable("FSHARP_RETAIN_TESTBUILDS") with
            | null -> cleanUpTemporaryDirectoryOfThisTestRun  ()
            | _ -> ()
            base.Dispose()

#else

// To use xUnit means to customize it. The following abomination adds 2 features:
// - Capturing full console output individually for each test case, viewable in Test Explorer as test stdout.
// - Internally parallelize test classes and theories. Test cases and theory cases included in a single class or F# module can execute simultaneously

/// Passes captured console output to xUnit.
type ConsoleCapturingTestRunner(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource) =
    inherit XunitTestRunner(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)

    member _.BaseInvokeTestMethodAsync aggregator = base.InvokeTestMethodAsync aggregator
    override this.InvokeTestAsync (aggregator: ExceptionAggregator) =
        task {
            use capture = new TestConsole.ExecutionCapture()
            use _ = Activity.start test.DisplayName [  ]
            let! executionTime = this.BaseInvokeTestMethodAsync aggregator
            let output =
                seq {
                    capture.OutText
                    if not (String.IsNullOrEmpty capture.ErrorText) then
                        ""
                        "=========== Standard Error ==========="
                        ""
                        capture.ErrorText
                } |> String.concat Environment.NewLine
            return executionTime, output
        }

module TestCaseCustomizations =
    // Internally parallelize test classes and theories.
    // Based on https://www.meziantou.net/parallelize-test-cases-execution-in-xunit.htm
    // The trick is to assign a unique test collection to each case.
    // Since test collection is xUnit's unit of parallelization, it will execute everything in parallel including theory cases.
    let rewriteTestMethod (testCase: ITestCase) : ITestMethod =
        let canFullyParallelize =
            // does not belong to a defined collection
            isNull testCase.TestMethod.TestClass.TestCollection.CollectionDefinition
            && testCase.TestMethod.TestClass.Class.GetCustomAttributes(typeof<Xunit.CollectionAttribute>) |> Seq.isEmpty
            // is not marked with `[<RunInSequence>]` attribute
            && testCase.TestMethod.Method.GetCustomAttributes(typeof<RunTestCasesInSequenceAttribute>) |> Seq.isEmpty
            && testCase.TestMethod.TestClass.Class.GetCustomAttributes(typeof<RunTestCasesInSequenceAttribute>) |> Seq.isEmpty

        if canFullyParallelize then
            let oldTestMethod = testCase.TestMethod
            let oldTestClass = oldTestMethod.TestClass
            let oldTestCollection = oldTestMethod.TestClass.TestCollection

            // Create a new collection with a unique id for the test case.
            let newTestCollection =
                    new TestCollection(
                        oldTestCollection.TestAssembly,
                        oldTestCollection.CollectionDefinition,
                        oldTestCollection.DisplayName,
                        Guid.NewGuid()
                    )

            let newTestClass = new TestClass(newTestCollection, oldTestClass.Class)
            TestMethod(newTestClass, oldTestMethod.Method)
        else
            testCase.TestMethod

type CustomTestCase =
    inherit XunitTestCase
    // xUinit demands this constructor for deserialization.
    new() = { inherit XunitTestCase() }
    
    new(sink: IMessageSink, md, mdo, testMethod, testMethodArgs) = { inherit XunitTestCase(sink, md, mdo, testMethod, testMethodArgs) }

    override testCase.RunAsync (_, bus, args, aggregator, cts) =
        let  runner : XunitTestCaseRunner =
            { new XunitTestCaseRunner(testCase, testCase.DisplayName, testCase.SkipReason, args, testCase.TestMethodArguments, bus, aggregator, cts) with
                override this.CreateTestRunner(test, bus, testCase, args, testMethod, methodArgs, skipReason, attrs, aggregator, cts) =
                    ConsoleCapturingTestRunner(test, bus, testCase, args, testMethod, methodArgs, skipReason, attrs, aggregator, cts)
            }
        runner.RunAsync()

    // Initialize is ensured by xUnit to run once before any property access.
    override testCase.Initialize () =
        base.Initialize()
        testCase.TestMethod <- TestCaseCustomizations.rewriteTestMethod testCase

type CustomTheoryTestCase =
    inherit XunitTheoryTestCase
    new() = { inherit XunitTheoryTestCase() }
    
    new(sink: IMessageSink, md, mdo, testMethod) = { inherit XunitTheoryTestCase(sink, md, mdo, testMethod) }

    override testCase.RunAsync (sink, bus, args, aggregator, cts) =
        let  runner : XunitTestCaseRunner =
            { new XunitTheoryTestCaseRunner(testCase, testCase.DisplayName, testCase.SkipReason, args, sink, bus, aggregator, cts) with
                override this.CreateTestRunner(test, bus, testCase, args, testMethod, methodArgs, skipReason, attrs, aggregator, cts) =
                    ConsoleCapturingTestRunner(test, bus, testCase, args, testMethod, methodArgs, skipReason, attrs, aggregator, cts)
            }
        runner.RunAsync()

    override testCase.Initialize () =
        base.Initialize()
        testCase.TestMethod <- TestCaseCustomizations.rewriteTestMethod testCase

/// `XunitTestFramework` providing parallel console support and conditionally enabling optional xUnit customizations.
type FSharpXunitFramework(sink: IMessageSink) =
    inherit XunitTestFramework(sink)

    let traceProvider =
        Sdk.CreateTracerProviderBuilder()
                .AddSource(ActivityNames.FscSourceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault().AddService(serviceName="F#", serviceVersion = "1.0.0"))
                .AddOtlpExporter()
                .Build()

    interface IDisposable with
        member _.Dispose() =
            cleanUpTemporaryDirectoryOfThisTestRun ()
            traceProvider.ForceFlush() |> ignore
            traceProvider.Dispose()
            base.Dispose()
            
    // Group test run under single activity, to make traces more readable.
    // Otherwise this overriden method is not necessary and can be removed.
    override this.CreateExecutor (assemblyName) = 
        { new XunitTestFrameworkExecutor(assemblyName, this.SourceInformationProvider, this.DiagnosticMessageSink) with
            override _.RunTestCases(testCases, executionMessageSink, executionOptions) =
                use _ = Activity.start $"{assemblyName.Name} {Runtime.InteropServices.RuntimeInformation.FrameworkDescription}" []

                // Because xUnit v2 lacks assembly fixture, the next best place to ensure things get called
                // right at the start of the test run is here or in the FSharpXunitFramework constructor.
                // This gets executed once per test assembly.
                printfn $"Running tests in {assemblyName.Name} with XUNIT_EXTRAS"
                logConfig initialConfig
                log "Installing TestConsole redirection"
                TestConsole.install()

#if !NETCOREAPP
                AssemblyResolver.addResolver ()
#endif
                base.RunTestCases(testCases, executionMessageSink, executionOptions)
        }

    override this.CreateDiscoverer (assemblyInfo) =
        { new XunitTestFrameworkDiscoverer(assemblyInfo, this.SourceInformationProvider, this.DiagnosticMessageSink) with
            override _.FindTestsForType (testClass, includeSourceInformation, messageBus, options) =
                // Intercepts test discovery messages to augment test cases with additional capabilities.
                let customizingBus =
                   { new IMessageBus with
                        member _.QueueMessage (message: IMessageSinkMessage) =
                            match message with
                            | :? ITestCaseDiscoveryMessage as discoveryMessage ->
                                let customized: ITestCase =
                                    match discoveryMessage.TestCase with
                                    | :? XunitTheoryTestCase ->
                                        new CustomTheoryTestCase(
                                            sink,
                                            options.MethodDisplayOrDefault(),
                                            options.MethodDisplayOptionsOrDefault(),
                                            discoveryMessage.TestCase.TestMethod,
                                            SourceInformation = discoveryMessage.TestCase.SourceInformation
                                        )
                                    | :? XunitTestCase ->
                                        new CustomTestCase(
                                            sink,
                                            options.MethodDisplayOrDefault(),
                                            options.MethodDisplayOptionsOrDefault(),
                                            discoveryMessage.TestCase.TestMethod,
                                            discoveryMessage.TestCase.TestMethodArguments,
                                            SourceInformation = discoveryMessage.TestCase.SourceInformation
                                        )
                                    | testCase -> testCase
                                messageBus.QueueMessage(TestCaseDiscoveryMessage customized)
                            | _ ->
                                messageBus.QueueMessage message
                        member _.Dispose () = messageBus.Dispose() }
                base.FindTestsForType(testClass, includeSourceInformation, customizingBus, options)
        }

#endif
