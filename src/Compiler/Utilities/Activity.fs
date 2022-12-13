// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics
open System.IO
open System.Text

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

        let sw = new StreamWriter(path = pathToFile, append = true)
        let msgQueue = MailboxProcessor<string>.Start(fun inbox -> 
            async { 
                while true do                    
                    let! msg =  inbox.Receive() 
                    do! sw.WriteLineAsync(msg) |> Async.AwaitTask
            })

        let l =
            new ActivityListener(
                ShouldListenTo = (fun a -> a.Name = activitySourceName),
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