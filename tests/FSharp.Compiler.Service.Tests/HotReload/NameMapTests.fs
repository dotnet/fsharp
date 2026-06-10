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
        atIndex >= 0
        && atIndex + 1 < name.Length
        && Char.IsDigit name[atIndex + 1]

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
    let ``LoadSnapshot canonicalizes hot reload ordinals for replay`` () =
        let map = FSharpSynthesizedTypeMaps()

        let outOfOrderSnapshot =
            [| struct ("closure", [| "closure@hotreload-10"; "closure@hotreload"; "closure@hotreload-2"; "closure@hotreload-1" |]) |]

        map.LoadSnapshot outOfOrderSnapshot
        map.BeginSession()

        let replayed = [| for _ in 0 .. 3 -> map.GetOrAddName "closure" |]
        let expected = [| "closure@hotreload"; "closure@hotreload-1"; "closure@hotreload-2"; "closure@hotreload-10" |]

        Assert.Equal<string>(expected, replayed)

    [<Fact>]
    let ``LoadSnapshot validates name prefix`` () =
        let map = FSharpSynthesizedTypeMaps()

        // Valid snapshots with different suffixes should work
        let validSnapshot = [|
            struct ("test", [| "test@hotreload"; "test@hotreload-1" |])
            struct ("Name", [| "Name@" |])  // Simple marker suffix
            struct ("Circle", [| "Circle@DebugTypeProxy" |])  // Debug proxy
        |]
        map.LoadSnapshot validSnapshot // Should not throw

    [<Fact>]
    let ``LoadSnapshot accepts legacy basic names`` () =
        let map = FSharpSynthesizedTypeMaps()

        // Some historical snapshots contain exact compiler-generated basic names
        // (for example "@_instance") instead of the newer "basicName@..." form.
        let legacySnapshot = [|
            struct ("@_instance", [| "@_instance" |])
            struct ("cached", [| "cached"; "cached@hotreload" |])
        |]

        map.LoadSnapshot legacySnapshot // Should not throw

    [<Fact>]
    let ``LoadSnapshot rejects basicName mismatch`` () =
        let map = FSharpSynthesizedTypeMaps()

        // Name doesn't start with basicName@
        let mismatchedSnapshot = [| struct ("foo", [| "bar@hotreload" |]) |]
        let ex = Assert.Throws<System.ArgumentException>(fun () -> map.LoadSnapshot mismatchedSnapshot)
        Assert.Contains("foo@", ex.Message)
        Assert.Contains("bar@hotreload", ex.Message)

    [<Fact>]
    let ``LoadSnapshot rejects name without marker`` () =
        let map = FSharpSynthesizedTypeMaps()

        // Name missing the @ marker entirely
        let invalidSnapshot = [| struct ("test", [| "testhotreload" |]) |]
        let ex = Assert.Throws<System.ArgumentException>(fun () -> map.LoadSnapshot invalidSnapshot)
        Assert.Contains("test@", ex.Message)
