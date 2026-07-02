namespace FSharp.Compiler.Service.Tests.HotReload

open System
open Xunit

open FSharp.Compiler.SynthesizedTypeMaps

module NameMapTests =

    [<Fact>]
    let ``name map replays recorded sequence`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        let first = map.GetOrAddName "lambda"
        let second = map.GetOrAddName "lambda"

        map.BeginSession()

        let replayFirst = map.GetOrAddName "lambda"
        let replaySecond = map.GetOrAddName "lambda"

        Assert.Equal(first, replayFirst)
        Assert.Equal(second, replaySecond)

    let private hasLineNumberSuffix (name: string) =
        let atIndex = name.IndexOf('@')
        atIndex >= 0 && atIndex + 1 < name.Length && Char.IsDigit name[atIndex + 1]

    [<Fact>]
    let ``generated names avoid source line suffixes`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        let name = map.GetOrAddName "closure"
        let another = map.GetOrAddName "closure"

        Assert.False(hasLineNumberSuffix name, $"Expected '{name}' to avoid line-number suffixes.")
        Assert.False(hasLineNumberSuffix another, $"Expected '{another}' to avoid line-number suffixes.")

    [<Fact>]
    let ``snapshot reload restores recorded names`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        let first = map.GetOrAddName "anon"
        let second = map.GetOrAddName "anon"

        let snapshot = map.Snapshot |> Seq.toArray

        let replay = FSharpSynthesizedTypeMaps()
        replay.LoadSnapshot snapshot
        replay.BeginSession()

        let replayFirst = replay.GetOrAddName "anon"
        let replaySecond = replay.GetOrAddName "anon"

        Assert.Equal<string>(first, replayFirst)
        Assert.Equal<string>(second, replaySecond)

    [<Fact>]
    let ``line-normalized replay preserves generation-zero pipe name`` () =
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()

        let baselineName = map.GetOrAddName "Pipe #1 stage #2 at line 28"

        map.BeginSession()

        let replayedName = map.GetOrAddName "Pipe #1 stage #2 at line 30"

        Assert.Equal("Pipe #1 stage #2 at line 28@hotreload", baselineName)
        Assert.Equal(baselineName, replayedName)
        Assert.Contains("line 28", replayedName)
        Assert.DoesNotContain("line 30", replayedName)

    [<Fact>]
    let ``LoadSnapshot normalizes old raw pipe keys`` () =
        let map = FSharpSynthesizedTypeMaps()

        let oldSnapshot =
            [| struct ("Pipe #1 stage #2 at line 28", [| "Pipe #1 stage #2 at line 28@hotreload" |]) |]

        map.LoadSnapshot oldSnapshot
        map.BeginSession()

        let replayedName = map.GetOrAddName "Pipe #1 stage #2 at line 30"
        let snapshot = map.Snapshot |> Seq.toArray
        let struct (snapshotKey, snapshotNames) = Assert.Single snapshot

        Assert.Equal("Pipe #1 stage #2 at line 28@hotreload", replayedName)
        Assert.Equal("Pipe #1 stage #2", snapshotKey)
        Assert.Equal<string[]>([| "Pipe #1 stage #2 at line 28@hotreload" |], snapshotNames)

    [<Fact>]
    let ``LoadSnapshot fills normalized pipe replay holes with birth-line names`` () =
        let map = FSharpSynthesizedTypeMaps()

        let gappedSnapshot =
            [| struct (
                "Pipe #1 stage #2",
                [| "Pipe #1 stage #2 at line 28@hotreload-2"; "Pipe #1 stage #2 at line 28@hotreload" |]
            ) |]

        map.LoadSnapshot gappedSnapshot
        map.BeginSession()

        let replayed = [| for _ in 0 .. 2 -> map.GetOrAddName "Pipe #1 stage #2 at line 30" |]

        Assert.Equal<string[]>(
            [| "Pipe #1 stage #2 at line 28@hotreload"
               "Pipe #1 stage #2 at line 28@hotreload-1"
               "Pipe #1 stage #2 at line 28@hotreload-2" |],
            replayed
        )

    [<Fact>]
    let ``LoadSnapshot canonicalizes hot reload ordinals for replay`` () =
        let map = FSharpSynthesizedTypeMaps()

        let outOfOrderSnapshot =
            [| struct ("closure", [| "closure@hotreload-10"; "closure@hotreload"; "closure@hotreload-2"; "closure@hotreload-1" |]) |]

        map.LoadSnapshot outOfOrderSnapshot
        map.BeginSession()

        // Replay is ordinal-positioned. A gapped bucket keeps every surviving name
        // at its exact allocation slot and re-computes the missing slots' names.
        let replayed = [| for _ in 0 .. 10 -> map.GetOrAddName "closure" |]

        let expected =
            [| "closure@hotreload"
               yield! [| for i in 1 .. 10 -> $"closure@hotreload-{i}" |] |]

        Assert.Equal<string[]>(expected, replayed)
        Assert.Equal("closure@hotreload-10", replayed[10])

    [<Fact>]
    let ``LoadSnapshot preserves occurrence-keyed generation-zero names`` () =
        let map = FSharpSynthesizedTypeMaps()

        let snapshot =
            [| struct ("f", [| "f@hotreload-2"; "f@hotreload#g0_o0"; "f@hotreload-1" |]) |]

        map.LoadSnapshot snapshot
        map.BeginSession()

        let replayed = [| for _ in 0 .. 2 -> map.GetOrAddName "f" |]
        Assert.Equal<string[]>([| "f@hotreload#g0_o0"; "f@hotreload-1"; "f@hotreload-2" |], replayed)

    [<Fact>]
    let ``LoadSnapshot validates name prefix`` () =
        let map = FSharpSynthesizedTypeMaps()

        let validSnapshot =
            [| struct ("test", [| "test@hotreload"; "test@hotreload-1" |])
               struct ("Name", [| "Name@" |])
               struct ("Circle", [| "Circle@DebugTypeProxy" |]) |]

        map.LoadSnapshot validSnapshot

    [<Fact>]
    let ``LoadSnapshot accepts legacy basic names`` () =
        let map = FSharpSynthesizedTypeMaps()

        let legacySnapshot =
            [| struct ("@_instance", [| "@_instance" |])
               struct ("cached", [| "cached"; "cached@hotreload" |]) |]

        map.LoadSnapshot legacySnapshot

    [<Fact>]
    let ``LoadSnapshot rejects basicName mismatch`` () =
        let map = FSharpSynthesizedTypeMaps()

        let mismatchedSnapshot = [| struct ("foo", [| "bar@hotreload" |]) |]
        let ex = Assert.Throws<ArgumentException>(fun () -> map.LoadSnapshot mismatchedSnapshot)
        Assert.Contains("snapshot key 'foo'", ex.Message)
        Assert.Contains("bar@hotreload", ex.Message)

    [<Fact>]
    let ``LoadSnapshot rejects name without marker`` () =
        let map = FSharpSynthesizedTypeMaps()

        let invalidSnapshot = [| struct ("test", [| "testhotreload" |]) |]
        let ex = Assert.Throws<ArgumentException>(fun () -> map.LoadSnapshot invalidSnapshot)
        Assert.Contains("snapshot key 'test'", ex.Message)
        Assert.Contains("testhotreload", ex.Message)

    [<Fact>]
    let ``LoadRecordedSnapshot preserves allocation-key slots`` () =
        let map = FSharpSynthesizedTypeMaps()

        let recordedSnapshot =
            [| struct ("allocation", [| "final@hotreload#g0_o0"; "allocation@hotreload-1" |]) |]

        map.LoadRecordedSnapshot recordedSnapshot
        map.BeginSession()

        Assert.True(map.UsesRecordedSnapshot)
        Assert.Equal("final@hotreload#g0_o0", map.GetOrAddName "allocation")
        Assert.Equal("allocation@hotreload-1", map.GetOrAddName "allocation")
