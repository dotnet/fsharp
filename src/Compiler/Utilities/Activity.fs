// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Collections.Concurrent
open System.Threading.Tasks


[<RequireQualifiedAccess>]
module Activity =

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

        let envStatsStart = "#stats_start"
        let envStatsEnd = "#stats_end"

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
            |]


    type private EnvironmentStats = { Handles : int; Threads : int; WorkingSetMB : int64; GarbageCollectionsPerGeneration : int[]}

    let private collectEnvironmentStats () = 
        let p = Process.GetCurrentProcess()
        {        
            Handles = p.HandleCount
            Threads = p.Threads.Count
            WorkingSetMB = p.WorkingSet64 / 1_000_000L
            GarbageCollectionsPerGeneration = [| for i in 0..GC.MaxGeneration  -> GC.CollectionCount i |]
        }


    let private activitySourceName = "fsc"
    let private activitySource = new ActivitySource(activitySourceName)

    let start (name: string) (tags: (string * string) seq) : IDisposable =
        let activity = activitySource.StartActivity(name)

        match activity with
        | null -> ()
        | activity ->
            for key, value in tags do
                activity.AddTag(key, value) |> ignore

        activity

    let startNoTags (name: string) : IDisposable = activitySource.StartActivity(name)


    let private profiledSourceName = "fsc_with_env_stats"
    let private profiledSource = new ActivitySource(profiledSourceName)

    let startAndMeasureEnvironmentStats (name : string) : IDisposable = profiledSource.StartActivity(name)

    let addStatsMeasurementListener () =
        let l =
            new ActivityListener(
                ShouldListenTo = (fun a -> a.Name = profiledSourceName),
                Sample = (fun _ -> ActivitySamplingResult.AllData),
                ActivityStarted = (fun a -> a.AddTag(Tags.envStatsStart, collectEnvironmentStats()) |> ignore),
                ActivityStopped = (fun a -> a.AddTag(Tags.envStatsEnd, collectEnvironmentStats()) |> ignore)
            )
        ActivitySource.AddActivityListener(l)

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
        appendWithLeadingComma (a.Duration.TotalSeconds.ToString("000.0000"))
        appendWithLeadingComma (a.Id)
        appendWithLeadingComma (a.ParentId)

        let rec rootID (act: Activity) =
            if isNull act.ParentId then act.Id else rootID act.Parent

        appendWithLeadingComma (rootID a)

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

        let messages = new BlockingCollection<string>(new ConcurrentQueue<string>())

        let l =
            new ActivityListener(
                ShouldListenTo = (fun a -> a.Name = activitySourceName || a.Name = profiledSourceName),
                Sample = (fun _ -> ActivitySamplingResult.AllData),
                ActivityStopped = (fun a -> messages.Add(createCsvRow a))
            )

        ActivitySource.AddActivityListener(l)

        let writerTask =
            Task.Factory.StartNew(fun () ->
                use sw = new StreamWriter(path = pathToFile, append = true)

                for msg in messages.GetConsumingEnumerable() do
                    sw.WriteLine(msg))

        { new IDisposable with
            member this.Dispose() =
                messages.CompleteAdding()
                writerTask.Wait()
        }
