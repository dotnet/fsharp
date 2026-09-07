// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module FSharp.Compiler.Service.Tests.IncrementalBuilderRaceTests

open System
open System.IO
open System.Collections.Concurrent
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Test.Assert
open Xunit

[<Fact>]
let ``Concurrent requests after a file change type check each file once`` () =
    // A private checker because we subscribe to FileChecked. The incremental builder is what is under test.
    let checker = FSharpChecker.Create(useTransparentCompiler = false)

    let dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
    Directory.CreateDirectory dir |> ignore

    try
        let fileNames = [| for i in 1 .. 5 -> Path.Combine(dir, $"File{i}.fs") |]
        fileNames |> Array.iteri (fun i fileName -> File.WriteAllText(fileName, $"module File{i + 1}\nlet x = 1\n"))

        let dllName = Path.Combine(dir, "Project.dll")
        let projFileName = Path.Combine(dir, "Project.fsproj")
        let args = mkProjectCommandLineArgs (dllName, fileNames)
        let options = { checker.GetProjectOptionsFromCommandLineArgs(projFileName, args) with SourceFiles = fileNames }

        let checkCounts = ConcurrentDictionary<string, int>()
        checker.FileChecked.Add(fun (fileName, _) -> checkCounts.AddOrUpdate(fileName, 1, (fun _ n -> n + 1)) |> ignore)

        let checkedFiles () =
            checkCounts |> Seq.map (fun kv -> Path.GetFileName kv.Key, kv.Value) |> Seq.sortBy fst |> List.ofSeq

        checker.ParseAndCheckProject options |> Async.RunSynchronouslyImmediate |> ignore
        checkedFiles () |> shouldEqual [ for i in 1 .. 5 -> $"File{i}.fs", 1 ]

        // Invalidate the whole chain by touching the first file, then hit the builder with many concurrent requests.
        // Each of them used to re-stamp the files from the same stale snapshot and build its own chain of bound models.
        checkCounts.Clear()
        File.SetLastWriteTimeUtc(fileNames[0], DateTime.UtcNow.AddSeconds 2.0)

        Seq.init 50 (fun _ -> checker.ParseAndCheckProject options |> Async.Ignore)
        |> Async.Parallel
        |> Async.RunSynchronouslyImmediate
        |> ignore

        checkedFiles () |> shouldEqual [ for i in 1 .. 5 -> $"File{i}.fs", 1 ]
    finally
        try Directory.Delete(dir, true) with _ -> ()
