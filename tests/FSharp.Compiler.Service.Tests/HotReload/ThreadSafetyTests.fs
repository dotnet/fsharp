namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.Threading
open System.Threading.Tasks
open Xunit

open FSharp.Compiler.SynthesizedTypeMaps

module ThreadSafetyTests =

    /// Helper to run actions concurrently and wait for all to complete
    let runConcurrently (count: int) (action: int -> unit) =
        let tasks = Array.init count (fun i -> Task.Run(fun () -> action i))
        Task.WaitAll(tasks)

    let parseHotReloadOrdinal (basicName: string) (name: string) =
        let prefix = basicName + "@hotreload"
        let numberedPrefix = prefix + "-"

        if String.Equals(name, prefix, StringComparison.Ordinal) then
            0
        elif name.StartsWith(numberedPrefix, StringComparison.Ordinal) then
            match Int32.TryParse(name.Substring(numberedPrefix.Length)) with
            | true, value when value > 0 -> value
            | _ -> failwithf "Invalid hot reload synthesized name '%s' for basic name '%s'." name basicName
        else
            failwithf "Unexpected synthesized name '%s' for basic name '%s'." name basicName

    [<Fact>]
    let ``concurrent GetOrAddName calls allocate unique sequential ordinals`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        let results = System.Collections.Concurrent.ConcurrentBag<string>()
        let errors = System.Collections.Concurrent.ConcurrentBag<exn>()
        let iterations = 1000

        runConcurrently iterations (fun _ ->
            try
                let name = map.GetOrAddName "concurrent"
                results.Add(name)
            with ex ->
                errors.Add(ex))

        Assert.Empty(errors)

        let names = results |> Seq.toArray
        Assert.Equal(iterations, names.Length)

        let ordinals = names |> Array.map (parseHotReloadOrdinal "concurrent")
        let uniqueOrdinals = ordinals |> Array.distinct |> Array.sort

        Assert.Equal(iterations, uniqueOrdinals.Length)
        Assert.Equal<int[]>([| 0 .. iterations - 1 |], uniqueOrdinals)

    [<Fact>]
    let ``concurrent GetOrAddName with multiple basic names is safe`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        let results = System.Collections.Concurrent.ConcurrentBag<string * string>()
        let errors = System.Collections.Concurrent.ConcurrentBag<exn>()
        let iterationsPerName = 50
        let basicNames = [| "lambda"; "closure"; "statemachine" |]

        runConcurrently (basicNames.Length * iterationsPerName) (fun i ->
            try
                let basicName = basicNames[i % basicNames.Length]
                let name = map.GetOrAddName basicName
                results.Add((basicName, name))
            with ex ->
                errors.Add(ex))

        // No exceptions should occur
        Assert.Empty(errors)

        let grouped = results |> Seq.groupBy fst |> Seq.toArray

        for (basicName, names) in grouped do
            // All names should be valid for this basic name
            for (_, name) in names do
                Assert.StartsWith(basicName + "@", name)

    [<Fact>]
    let ``concurrent BeginSession and GetOrAddName`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        // Pre-populate some names
        for _ in 1..10 do
            map.GetOrAddName "test" |> ignore

        let errors = System.Collections.Concurrent.ConcurrentBag<exn>()

        // Run concurrent operations - some reset, some add
        runConcurrently 100 (fun i ->
            try
                if i % 10 = 0 then
                    map.BeginSession()
                else
                    map.GetOrAddName "test" |> ignore
            with ex ->
                errors.Add(ex))

        // No exceptions should occur
        Assert.Empty(errors)

    [<Fact>]
    let ``concurrent LoadSnapshot and GetOrAddName`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        // Create a valid snapshot
        let snapshot = [| struct ("test", [| "test@hotreload"; "test@hotreload-1" |]) |]

        let errors = System.Collections.Concurrent.ConcurrentBag<exn>()

        runConcurrently 100 (fun i ->
            try
                if i % 20 = 0 then
                    map.LoadSnapshot snapshot
                    map.BeginSession()
                else
                    map.GetOrAddName "test" |> ignore
            with ex ->
                errors.Add(ex))

        Assert.Empty(errors)

    [<Fact>]
    let ``stress test with 1000 concurrent operations`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        let operationCount = 1000
        let errors = System.Collections.Concurrent.ConcurrentBag<exn>()
        let basicNames = [| "a"; "b"; "c"; "d"; "e" |]

        runConcurrently operationCount (fun i ->
            try
                let basicName = basicNames[i % basicNames.Length]
                map.GetOrAddName basicName |> ignore
            with ex ->
                errors.Add(ex))

        Assert.Empty(errors)

        // Verify we can still use the map correctly after stress
        map.BeginSession()
        let name = map.GetOrAddName "verify"
        Assert.StartsWith("verify@", name)

    [<Fact>]
    let ``concurrent Snapshot calls are safe`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        // Populate the map
        for _ in 1..50 do
            map.GetOrAddName "snapshot" |> ignore

        let snapshots = System.Collections.Concurrent.ConcurrentBag<struct (string * string[])[]>()
        let errors = System.Collections.Concurrent.ConcurrentBag<exn>()

        runConcurrently 50 (fun i ->
            try
                if i % 5 = 0 then
                    map.GetOrAddName "snapshot" |> ignore
                let snapshot = map.Snapshot |> Seq.toArray
                snapshots.Add(snapshot)
            with ex ->
                errors.Add(ex))

        Assert.Empty(errors)

        // All snapshots should be valid
        for snapshot in snapshots do
            Assert.NotEmpty(snapshot)
