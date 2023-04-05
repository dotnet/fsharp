﻿namespace Microsoft.VisualStudio.FSharp.Editor.Logging

open System
open System.Diagnostics
open System.ComponentModel.Composition
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Editor

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
        | Some (0, pane) ->
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
    let private log logType msg = logger.Value.Log(logType, msg)

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

module Activity =
    let listen filter =
        let indent (activity: Activity) =
            let rec loop (activity: Activity) n =
                if activity.Parent <> null then
                    loop (activity.Parent) (n + 1)
                else
                    n

            String.replicate (loop activity 0) "    "

        let collectTags (activity: Activity) =
            [ for tag in activity.Tags -> $"{tag.Key}: %A{tag.Value}" ]
            |> String.concat ", "

        let listener =
            new ActivityListener(
                ShouldListenTo = (fun source -> source.Name = FSharp.Compiler.Diagnostics.ActivityNames.FscSourceName),
                Sample =
                    (fun context ->
                        if context.Name.Contains(filter) then
                            ActivitySamplingResult.AllDataAndRecorded
                        else
                            ActivitySamplingResult.None),
                ActivityStarted = (fun a -> logMsg $"{indent a}{a.OperationName}     {collectTags a}")
            )

        ActivitySource.AddActivityListener(listener)

    let listenToAll () = listen ""
