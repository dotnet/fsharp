namespace Microsoft.VisualStudio.FSharp.Editor.DebugHelpers

open System
open System.Diagnostics
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open FSharp.Compiler.Diagnostics

[<RequireQualifiedAccess>]
type LogType =
    | Info
    | Warn
    | Error
    | Message

    override x.ToString() =
        match x with
        | Message -> "Message"
        | Info -> "Information"
        | Warn -> "Warning"
        | Error -> "Error"

module Config =
    [<Literal>]
    let fsharpOutputGuidString = "E721F849-446C-458C-997A-99E14A04CFD3"

    let fsharpOutputGuid = Guid fsharpOutputGuidString

open Config
open System.Diagnostics.Metrics
open System.Text
open Microsoft.VisualStudio.Threading
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

module FSharpOutputPane =

    let private pane =
        AsyncLazy(
            fun () ->
                task {
                    do! ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync()
                    let! window = AsyncServiceProvider.GlobalProvider.GetServiceAsync<SVsOutputWindow, IVsOutputWindow>()

                    window.CreatePane(ref fsharpOutputGuid, "F# Language Service", Convert.ToInt32 true, Convert.ToInt32 false)
                    |> ignore

                    match window.GetPane(ref fsharpOutputGuid) with
                    | 0, pane -> return pane
                    | _ -> return failwith "Could not get F# output pane"
                }
            , ThreadHelper.JoinableTaskFactory
        )

    let inline debug msg = Printf.kprintf Debug.WriteLine msg

    let private log logType msg =
        task {
            System.Diagnostics.Trace.TraceInformation(msg)
            let! pane = pane.GetValueAsync()

            do! ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync()

            match logType with
            | LogType.Message -> $"{msg}"
            | LogType.Info -> $"[INFO] {msg}"
            | LogType.Warn -> $"[WARN] {msg}"
            | LogType.Error -> $"[ERROR] {msg}"
            |> pane.OutputStringThreadSafe
            |> ignore
        }
        |> ignore

    let logMsg msg = log LogType.Message msg
    let logInfo msg = log LogType.Info msg
    let logWarning msg = log LogType.Warn msg
    let logError msg = log LogType.Error msg

    let logMsgf msg =
        Printf.kprintf (log LogType.Message) msg

    let logInfof msg = Printf.kprintf (log LogType.Info) msg
    let logWarningf msg = Printf.kprintf (log LogType.Warn) msg
    let logErrorf msg = Printf.kprintf (log LogType.Error) msg

    let logException (ex: Exception) =
        logErrorf "Exception Message: %s\nStack Trace: %s" ex.Message ex.StackTrace

    let logExceptionWithContext (ex: Exception, context) =
        logErrorf "Context: %s\nException Message: %s\nStack Trace: %s" context ex.Message ex.StackTrace

module FSharpServiceTelemetry =
    open FSharp.Compiler.Caches
    open System.Threading.Tasks

    let listen filter =
        let indent (activity: Activity) =
            let rec loop (activity: Activity) n =
                if activity.Parent <> null then
                    loop (activity.Parent) (n + 1)
                else
                    n

            String.replicate (loop activity 0) "    "

        let collectTags (activity: Activity) =
            [ for tag in activity.Tags -> $"{tag.Key}: {tag.Value}" ] |> String.concat ", "

        let listener =
            new ActivityListener(
                ShouldListenTo = (fun source -> source.Name = ActivityNames.FscSourceName),
                Sample =
                    (fun context ->
                        if context.Name.Contains(filter) then
                            ActivitySamplingResult.AllData
                        else
                            ActivitySamplingResult.None),
                ActivityStarted = (fun a -> FSharpOutputPane.logMsg $"{indent a}{a.OperationName}     {collectTags a}")
            )

        ActivitySource.AddActivityListener(listener)

    let periodicallyDisplayMetrics =
        cancellableTask {
            use _ = CacheMetrics.ListenToAll()
            use _ = FSharp.Compiler.DiagnosticsLogger.StackGuardMetrics.Listen()

            while true do
                do! Task.Delay(TimeSpan.FromSeconds 10.0)
                FSharpOutputPane.logMsg (CacheMetrics.StatsToString())
                FSharpOutputPane.logMsg (FSharp.Compiler.DiagnosticsLogger.StackGuardMetrics.StatsToString())
        }

#if DEBUG
    open OpenTelemetry.Resources
    open OpenTelemetry.Trace
    open OpenTelemetry.Metrics

    let otelExport () =
        // On Windows forwarding localhost to wsl2 docker container sometimes does not work. Use IP address instead.
        let otlpEndpoint = Uri("http://127.0.0.1:4317")

        let meterProvider =
            // Configure OpenTelemetry metrics. Metrics can be viewed in Prometheus or other compatible tools.
            OpenTelemetry.Sdk
                .CreateMeterProviderBuilder()
                .ConfigureResource(fun r -> r.AddService("F#") |> ignore)
                .AddMeter(CacheMetrics.Meter.Name)
                .AddMeter("System.Runtime")
                .AddOtlpExporter(fun e m ->
                    e.Endpoint <- otlpEndpoint
                    m.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds <- 1000
                    m.TemporalityPreference <- MetricReaderTemporalityPreference.Cumulative)
                .Build()

        let tracerProvider =
            // Configure OpenTelemetry export. Traces can be viewed in Jaeger or other compatible tools.
            OpenTelemetry.Sdk
                .CreateTracerProviderBuilder()
                .AddSource(ActivityNames.FscSourceName)
                .ConfigureResource(fun r -> r.AddService("F#") |> ignore)
                .AddOtlpExporter(fun e -> e.Endpoint <- otlpEndpoint)
                .Build()

        let a = Activity.startNoTags "FSharpPackage"

        fun () ->
            a.Dispose()
            tracerProvider.ForceFlush(5000) |> ignore
            tracerProvider.Dispose()
            meterProvider.Dispose()

    let listenToAll () = listen ""
#endif
