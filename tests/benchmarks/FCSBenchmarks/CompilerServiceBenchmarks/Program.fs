open BenchmarkDotNet.Configs
open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks
open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

open FSharp.Compiler.Diagnostics.Activity

[<EntryPoint>]
let main args =
    let b = DecentlySizedStandAloneFileBenchmark()
    
    // eventually this would need to only export to the OLTP collector, and even then only if configured. always-on is no good.
    // when this configuration becomes opt-in, we'll also need to safely check activities around every StartActivity call, because those could
    // be null
    use tracerProvider =
        Sdk.CreateTracerProviderBuilder()
           .AddSource(activitySourceName)
           .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName ="program", serviceVersion = "42.42.42.44"))
           .AddOtlpExporter()
           .AddZipkinExporter()
           .Build();
    use mainActivity = activitySource.StartActivity("main")

    let forceCleanup() =
        mainActivity.Dispose()
        activitySource.Dispose()
        tracerProvider.Dispose()
    
    b.Setup()
    b.Run()
    forceCleanup()
    0
