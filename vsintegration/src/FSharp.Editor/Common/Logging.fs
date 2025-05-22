namespace Microsoft.VisualStudio.FSharp.Editor.Logging

open System
open System.Diagnostics
open System.ComponentModel.Composition
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Editor

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

[<Export>]
type Logger [<ImportingConstructor>] ([<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider) =
    let outputWindow =
        serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>() |> Option.ofObj

    let createPane () =
        outputWindow
        |> Option.iter (fun x ->
            x.CreatePane(ref fsharpOutputGuid, "F# Language Service", Convert.ToInt32 true, Convert.ToInt32 false)
            |> ignore)

    do createPane ()

    let getPane () =
        match outputWindow |> Option.map (fun x -> x.GetPane(ref fsharpOutputGuid)) with
        | Some(0, pane) ->
            pane.Activate() |> ignore
            Some pane
        | _ -> None

    static let mutable globalServiceProvider: IServiceProvider option = None

    static member GlobalServiceProvider
        with get () =
            globalServiceProvider
            |> Option.defaultValue (ServiceProvider.GlobalProvider :> IServiceProvider)
        and set v = globalServiceProvider <- Some v

    member _.FSharpLoggingPane =
        getPane ()
        |> function
            | Some pane -> Some pane
            | None ->
                createPane ()
                getPane ()

    member self.Log(msgType: LogType, msg: string) =
        let time = DateTime.Now.ToString("hh:mm:ss tt")

        match self.FSharpLoggingPane, msgType with
        | None, _ -> ()
        | Some pane, LogType.Message ->
            String.Format("[{0}{1}] {2}{3}", "", time, msg, Environment.NewLine)
            |> pane.OutputString
            |> ignore
        | Some pane, LogType.Info ->
            String.Format("[{0}{1}] {2}{3}", "INFO ", time, msg, Environment.NewLine)
            |> pane.OutputString
            |> ignore
        | Some pane, LogType.Warn ->
            String.Format("[{0}{1}] {2}{3}", "WARN ", time, msg, Environment.NewLine)
            |> pane.OutputString
            |> ignore
        | Some pane, LogType.Error ->
            String.Format("[{0}{1}] {2}{3}", "ERROR ", time, msg, Environment.NewLine)
            |> pane.OutputString
            |> ignore

[<AutoOpen>]
module Logging =

    let inline debug msg = Printf.kprintf Debug.WriteLine msg

    let private logger = lazy Logger(Logger.GlobalServiceProvider)

    let private log logType msg =
        logger.Value.Log(logType, msg)
        System.Diagnostics.Trace.TraceInformation(msg)

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
                ActivityStarted = (fun a -> logMsg $"{indent a}{a.OperationName}     {collectTags a}")
            )

        ActivitySource.AddActivityListener(listener)

    let logCacheMetricsToOutput () =

        let timer = new System.Timers.Timer(1000.0, AutoReset = true)

        timer.Elapsed.Add(fun _ ->
            let stats = CacheMetrics.GetStatsUpdateForAllCaches(clearCounts = true)

            if stats <> "" then
                logMsg $"\n{stats}")

        timer.Start()

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
