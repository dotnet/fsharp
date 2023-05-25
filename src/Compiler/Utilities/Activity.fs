// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics
open System.IO
open System.Text

module ActivityNames =
    [<Literal>]
    let FscSourceName = "fsc"

    [<Literal>]
    let ProfiledSourceName = "fsc_with_env_stats"

    let AllRelevantNames = [| FscSourceName; ProfiledSourceName |]

[<RequireQualifiedAccess>]
module internal Activity =

    module Tags =
        let fileName = "fileName"
        let project = "project"
        let qualifiedNameOfFile = "qualifiedNameOfFile"
        let userOpName = "userOpName"
        let length = "length"
        let cache = "cache"
        let cpuDelta = "cpuDelta(s)"
        let realDelta = "realDelta(s)"
        let gc0 = "gc0"
        let gc1 = "gc1"
        let gc2 = "gc2"
        let outputDllFile = "outputDllFile"
        let buildPhase = "buildPhase"

        let AllKnownTags =
            [|
                fileName
                project
                qualifiedNameOfFile
                userOpName
                length
                cache
                cpuDelta
                realDelta
                gc0
                gc1
                gc2
                outputDllFile
                buildPhase
            |]

    module Events =
        let cacheHit = "cacheHit"

    type System.Diagnostics.Activity with

        member this.RootId =
            let rec rootID (act: Activity) =
                if isNull act.ParentId then act.Id else rootID act.Parent

            rootID this

        member this.Depth =
            let rec depth (act: Activity) acc =
                if isNull act.ParentId then
                    acc
                else
                    depth act.Parent (acc + 1)

            depth this 0

    let private activitySource = new ActivitySource(ActivityNames.FscSourceName)

    let start (name: string) (tags: (string * string) seq) : IDisposable =
        let activity = activitySource.CreateActivity(name, ActivityKind.Internal)

        match activity with
        | null -> activity
        | activity ->
            for key, value in tags do
                activity.AddTag(key, value) |> ignore

            activity.Start()

    let startNoTags (name: string) : IDisposable = activitySource.StartActivity(name)

    let addEvent name =
        if Activity.Current <> null && Activity.Current.Source = activitySource then
            Activity.Current.AddEvent(ActivityEvent(name)) |> ignore

    module Profiling =

        module Tags =
            let workingSetMB = "workingSet(MB)"
            let gc0 = "gc0"
            let gc1 = "gc1"
            let gc2 = "gc2"
            let handles = "handles"
            let threads = "threads"

            let profilingTags = [| workingSetMB; gc0; gc1; gc2; handles; threads |]

        let private profiledSource = new ActivitySource(ActivityNames.ProfiledSourceName)

        let startAndMeasureEnvironmentStats (name: string) : IDisposable = profiledSource.StartActivity(name)

        type private GCStats = int[]

        let private collectGCStats () : GCStats =
            [| for i in 0 .. GC.MaxGeneration -> GC.CollectionCount i |]

        let private addStatsMeasurementListener () =
            let gcStatsInnerTag = "#gc_stats_internal"

            let l =
                new ActivityListener(
                    ShouldListenTo = (fun a -> a.Name = ActivityNames.ProfiledSourceName),
                    Sample = (fun _ -> ActivitySamplingResult.AllData),
                    ActivityStarted = (fun a -> a.AddTag(gcStatsInnerTag, collectGCStats ()) |> ignore),
                    ActivityStopped =
                        (fun a ->
                            let statsBefore = a.GetTagItem(gcStatsInnerTag) :?> GCStats
                            let statsAfter = collectGCStats ()
                            let p = Process.GetCurrentProcess()
                            a.AddTag(Tags.workingSetMB, p.WorkingSet64 / 1_000_000L) |> ignore
                            a.AddTag(Tags.handles, p.HandleCount) |> ignore
                            a.AddTag(Tags.threads, p.Threads.Count) |> ignore

                            for i = 0 to statsAfter.Length - 1 do
                                a.AddTag($"gc{i}", statsAfter[i] - statsBefore[i]) |> ignore)

                )

            ActivitySource.AddActivityListener(l)
            l

        let addConsoleListener () =
            let statsMeasurementListener = addStatsMeasurementListener ()

            let reportingStart = DateTime.UtcNow
            let nameColumnWidth = 36

            let header =
                "|"
                + "Phase name".PadRight(nameColumnWidth)
                + "|Elapsed |Duration| WS(MB)|  GC0  |  GC1  |  GC2  |Handles|Threads|"

            let consoleWriterListener =
                new ActivityListener(
                    ShouldListenTo = (fun a -> a.Name = ActivityNames.ProfiledSourceName),
                    Sample = (fun _ -> ActivitySamplingResult.AllData),
                    ActivityStopped =
                        (fun a ->
                            Console.Write('|')
                            let indentedName = new String('>', a.Depth) + a.DisplayName
                            Console.Write(indentedName.PadRight(nameColumnWidth))

                            let elapsed = (a.StartTimeUtc + a.Duration - reportingStart).TotalSeconds
                            Console.Write("|{0,8:N4}|{1,8:N4}|", elapsed, a.Duration.TotalSeconds)

                            for t in Tags.profilingTags do
                                Console.Write("{0,7}|", a.GetTagItem(t))

                            Console.WriteLine())
                )

            Console.WriteLine(new String('-', header.Length))
            Console.WriteLine(header)
            Console.WriteLine(header |> String.map (fun c -> if c = '|' then c else '-'))

            ActivitySource.AddActivityListener(consoleWriterListener)

            { new IDisposable with
                member this.Dispose() =
                    statsMeasurementListener.Dispose()
                    consoleWriterListener.Dispose()
                    Console.WriteLine(new String('-', header.Length))
            }

    module CsvExport =

        let private escapeStringForCsv (o: obj) =
            if isNull o then
                ""
            else
                let mutable txtVal = o.ToString()
                let hasComma = txtVal.IndexOf(',') > -1
                let hasQuote = txtVal.IndexOf('"') > -1

                if hasQuote then
                    txtVal <- txtVal.Replace("\"", "\\\"")

                if hasQuote || hasComma then
                    "\"" + txtVal + "\""
                else
                    txtVal

        let private createCsvRow (a: Activity) =
            let sb = new StringBuilder(128)

            let appendWithLeadingComma (s: string) =
                sb.Append(',') |> ignore
                sb.Append(s) |> ignore

            // "Name,StartTime,EndTime,Duration,Id,ParentId"
            sb.Append(a.DisplayName) |> ignore
            appendWithLeadingComma (a.StartTimeUtc.ToString("HH-mm-ss.ffff"))
            appendWithLeadingComma ((a.StartTimeUtc + a.Duration).ToString("HH-mm-ss.ffff"))
            appendWithLeadingComma (a.Duration.TotalSeconds.ToString("000.0000", System.Globalization.CultureInfo.InvariantCulture))
            appendWithLeadingComma (a.Id)
            appendWithLeadingComma (a.ParentId)
            appendWithLeadingComma (a.RootId)

            Tags.AllKnownTags
            |> Array.iter (fun t -> a.GetTagItem(t) |> escapeStringForCsv |> appendWithLeadingComma)

            sb.ToString()

        let addCsvFileListener pathToFile =
            if pathToFile |> File.Exists |> not then
                File.WriteAllLines(
                    pathToFile,
                    [
                        "Name,StartTime,EndTime,Duration(s),Id,ParentId,RootId,"
                        + String.concat "," Tags.AllKnownTags
                    ]
                )

            let sw = new StreamWriter(path = pathToFile, append = true)

            let msgQueue =
                MailboxProcessor<string>.Start
                    (fun inbox ->
                        async {
                            while true do
                                let! msg = inbox.Receive()
                                do! sw.WriteLineAsync(msg) |> Async.AwaitTask
                        })

            let l =
                new ActivityListener(
                    ShouldListenTo = (fun a -> ActivityNames.AllRelevantNames |> Array.contains a.Name),
                    Sample = (fun _ -> ActivitySamplingResult.AllData),
                    ActivityStopped = (fun a -> msgQueue.Post(createCsvRow a))
                )

            ActivitySource.AddActivityListener(l)

            { new IDisposable with
                member this.Dispose() =
                    l.Dispose() // Unregister from listening new activities first
                    (msgQueue :> IDisposable).Dispose() // Wait for the msg queue to be written out
                    sw.Dispose() // Only then flush the messages and close the file
            }
