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
        let cpuDelta = "cpuDelta"
        let realDelta = "realDelta"
        let gc0 = "gc0"
        let gc1 = "gc1"
        let gc2 = "gc2"
        let outputDllFile = "outputDllFile"

        let AllKnownTags = [|fileName;project;qualifiedNameOfFile;userOpName;length;cache;cpuDelta;realDelta;gc0;gc1;gc2;outputDllFile|]

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

    let private escapeStringForCsv (o:obj) =
        if isNull o then
            ""
        else 
            let mutable txtVal = o.ToString()
            let hasComma = txtVal.IndexOf(',') > -1
            let hasQuote = txtVal.IndexOf('"') > -1
        
            if hasQuote then
                txtVal <- txtVal.Replace("\"","\\\"")
            
            if hasQuote || hasComma then
                "\"" + txtVal + "\""
            else
                txtVal
        

    let private createCsvRow (a : Activity) = 
        let endTime = a.StartTimeUtc + a.Duration
        let startTimeString = a.StartTimeUtc.ToString("HH-mm-ss.ffff")
        let endTimeString = endTime.ToString("HH-mm-ss.ffff")
        let duration = a.Duration.TotalMilliseconds       

        let sb = new StringBuilder(128)

        Printf.bprintf sb "%s,%s,%s,%f,%s,%s" a.DisplayName startTimeString endTimeString duration a.Id a.ParentId
        Tags.AllKnownTags |> Array.iter (fun t -> 
            sb.Append(',') |> ignore
            sb.Append(escapeStringForCsv(a.GetTagItem(t))) |> ignore)

        sb.ToString()

    let addCsvFileListener pathToFile =
        if pathToFile |> File.Exists |> not then
            File.WriteAllLines(pathToFile,["Name,StartTime,EndTime,Duration,Id,ParentId," + String.concat "," Tags.AllKnownTags])

        let messages = new BlockingCollection<string>(new ConcurrentQueue<string>())

        let l = new ActivityListener(
            ShouldListenTo = (fun a -> a.Name = activitySourceName), 
            Sample = (fun _ -> ActivitySamplingResult.AllData),
            ActivityStopped = (fun a -> messages.Add(createCsvRow a)))

        ActivitySource.AddActivityListener(l)
        
        let writerTask = 
            Task.Factory.StartNew(fun () ->
                use sw = new StreamWriter(path = pathToFile, append = true)       
                for msg in messages.GetConsumingEnumerable() do
                    sw.WriteLine(msg))

        {new IDisposable with
             member this.Dispose() = 
                messages.CompleteAdding()
                writerTask.Wait()}