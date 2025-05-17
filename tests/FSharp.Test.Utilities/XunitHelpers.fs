#if XUNIT_EXTRAS
#nowarn "0044"
#endif

namespace FSharp.Test

open System
open Xunit.Sdk
open Xunit.Abstractions

open TestFramework

open FSharp.Compiler.Caches
open FSharp.Compiler.Diagnostics

open OpenTelemetry.Resources
open OpenTelemetry.Trace
open OpenTelemetry.Metrics

/// Disables custom internal parallelization added with XUNIT_EXTRAS.
/// Execute test cases in a class or a module one by one instead of all at once. Allow other collections to run simultaneously.
[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method, AllowMultiple = false)>]
type RunTestCasesInSequenceAttribute() = inherit Attribute()

// Helper for stress testing.
// Runs a test case many times in parallel.
// Example usage: [<Theory; Stress(Count = 1000)>]
type StressAttribute([<ParamArray>] data: obj array) =
    inherit DataAttribute()
    member val Count = 1 with get, set
    override this.GetData _ = Seq.init this.Count (fun i -> [| yield! data; yield box i |])

#if XUNIT_EXTRAS

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
            use _ = Activity.startNoTags test.DisplayName
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

    let sha = Security.Cryptography.SHA256.Create()

    // We add extra trait to each test, of the form "batch=n" where n is between 1 and 4.
    // It can be used to filter on in multi-agent testing in CI
    // with dotnet test filter switch, for example "--filter batch=1"
    // That way each agent can run test for a batch of tests. 
    let NumberOfBatchesInMultiAgentTesting = 4u

    let addBatchTrait (testCase: ITestCase) =
        // Get a batch number stable between multiple test runs. 
        // UniqueID is ideal here, it does not change across many compilations of the same code
        // and it will split theories with member data into many batches.
        let data = Text.Encoding.UTF8.GetBytes testCase.UniqueID
        let hashCode = BitConverter.ToUInt32(sha.ComputeHash(data), 0)
        let batch = hashCode % NumberOfBatchesInMultiAgentTesting + 1u
        testCase.Traits.Add("batch", ResizeArray [ string batch ])

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
        TestCaseCustomizations.addBatchTrait testCase

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
        TestCaseCustomizations.addBatchTrait testCase

#endif


type OpenTelemetryExport(testRunName, enable) =
    // On Windows forwarding localhost to wsl2 docker container sometimes does not work. Use IP address instead.
    let otlpEndpoint = Uri("http://127.0.0.1:4317")
    
    // Configure OpenTelemetry export. 
    let providers : IDisposable list =
        if not enable then [] else
            [
            // Configure OpenTelemetry tracing export. Traces can be viewed in Jaeger or other compatible tools.
            OpenTelemetry.Sdk.CreateTracerProviderBuilder()
                .AddSource(ActivityNames.FscSourceName)
                .ConfigureResource(fun r -> r.AddService("F#") |> ignore)
                .AddOtlpExporter(fun o ->
                    o.Endpoint <- otlpEndpoint
                    o.Protocol <- OpenTelemetry.Exporter.OtlpExportProtocol.Grpc
                    // Empirical values to ensure no traces are lost and no significant delay at the end of test run.
                    o.TimeoutMilliseconds <- 200
                    o.BatchExportProcessorOptions.MaxQueueSize <- 16384
                    o.BatchExportProcessorOptions.ScheduledDelayMilliseconds <- 100
                )
                .Build()

            // Configure OpenTelemetry metrics export. Metrics can be viewed in Prometheus or other compatible tools.
            OpenTelemetry.Sdk.CreateMeterProviderBuilder()
                .AddMeter(CacheMetrics.Meter.Name)
                .AddMeter("System.Runtime")
                .ConfigureResource(fun r -> r.AddService(testRunName) |> ignore)
                .AddOtlpExporter(fun e m ->
                    e.Endpoint <- otlpEndpoint
                    e.Protocol <- OpenTelemetry.Exporter.OtlpExportProtocol.Grpc
                    m.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds <- 1000
                )
                .Build()
            ]

    interface IDisposable with
        member this.Dispose() =
            for p in providers do p.Dispose()

/// `XunitTestFramework` providing parallel console support and conditionally enabling optional xUnit customizations.
type FSharpXunitFramework(sink: IMessageSink) =
    inherit XunitTestFramework(sink)
            
    override this.CreateExecutor (assemblyName) =
        { new XunitTestFrameworkExecutor(assemblyName, this.SourceInformationProvider, this.DiagnosticMessageSink) with
            
            // Because xUnit v2 lacks assembly fixture, this is a good place to ensure things get called right at the start of the test run.
            // This gets executed once per test assembly.
            override x.RunTestCases(testCases, executionMessageSink, executionOptions) =

            #if !NETCOREAPP
                // We need AssemblyResolver already here, because OpenTelemetry loads some assemblies dynamically.
                AssemblyResolver.addResolver ()
            #endif

                // Override cache capacity to reduce memory usage in CI.
                Cache.OverrideCapacityForTesting()

                let testRunName = $"RunTests_{assemblyName.Name} {Runtime.InteropServices.RuntimeInformation.FrameworkDescription}"

                use _ = new OpenTelemetryExport(testRunName, Environment.GetEnvironmentVariable("FSHARP_OTEL_EXPORT") <> null)   
                
                logConfig initialConfig
                log "Installing TestConsole redirection"
                TestConsole.install()
              
                begin
                    use _ = Activity.startNoTags testRunName
                    // We can't just call base.RunTestCases here, because it's implementation is async void.
                    use runner = new XunitTestAssemblyRunner (x.TestAssembly, testCases, x.DiagnosticMessageSink, executionMessageSink, executionOptions)
                    runner.RunAsync().Wait()
                end

                cleanUpTemporaryDirectoryOfThisTestRun ()
        }

#if XUNIT_EXTRAS
    // Rewrites discovered test cases to support extra parallelization and capturing console as test output.
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
