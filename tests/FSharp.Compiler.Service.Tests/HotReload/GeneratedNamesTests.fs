namespace FSharp.Compiler.Service.Tests.HotReload

open Xunit

open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.Text

module GeneratedNamesTests =

    let zeroRange = Range.range0

    [<Fact>]
    let ``NiceNameGenerator without map uses legacy suffix`` () =
        let compilerState = CompilerGlobalState()
        clearCompilerGeneratedNameMap (compilerState :> obj)
        let generator = compilerState.NiceNameGenerator

        let first = generator.FreshCompilerGeneratedName("lambda", zeroRange)
        let second = generator.FreshCompilerGeneratedName("lambda", zeroRange)

        Assert.Equal("lambda@1", first)
        Assert.Equal("lambda@1-1", second)

    [<Fact>]
    let ``NiceNameGenerator with synthesized map replays snapshot`` () =
        let compilerState = CompilerGlobalState()
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()
        setCompilerGeneratedNameMap (compilerState :> obj) (map :> ICompilerGeneratedNameMap)

        let generator = compilerState.NiceNameGenerator

        let first = generator.FreshCompilerGeneratedName("closure", zeroRange)
        let second = generator.FreshCompilerGeneratedName("closure", zeroRange)

        let snapshot =
            map.Snapshot
            |> Seq.find (fun struct (key, _) -> key = "closure")
            |> (fun struct (_, names) -> names)

        map.BeginSession()

        let replayFirst = generator.FreshCompilerGeneratedName("closure", zeroRange)
        let replaySecond = generator.FreshCompilerGeneratedName("closure", zeroRange)

        Assert.Equal("closure@hotreload", first)
        Assert.Equal("closure@hotreload-1", second)
        Assert.Equal<string[]>(snapshot, [| first; second |])
        Assert.Equal<string[]>(snapshot, [| replayFirst; replaySecond |])

    [<Fact>]
    let ``NiceNameGenerator counters not incremented during hot reload mode`` () =
        // This test verifies that when hot reload is enabled, the internal
        // basicNameCounts counter is NOT incremented. This prevents counter drift
        // between the per-file basicNameCounts and the global map ordinals.
        let compilerState = CompilerGlobalState()
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()
        setCompilerGeneratedNameMap (compilerState :> obj) (map :> ICompilerGeneratedNameMap)

        let generator = compilerState.NiceNameGenerator

        // Generate names while hot reload is enabled
        let _ = generator.FreshCompilerGeneratedName("test", zeroRange)
        let _ = generator.FreshCompilerGeneratedName("test", zeroRange)

        // Disable hot reload - fallback names should start fresh.
        clearCompilerGeneratedNameMap (compilerState :> obj)

        let first = generator.FreshCompilerGeneratedName("test", zeroRange)
        let second = generator.FreshCompilerGeneratedName("test", zeroRange)

        Assert.Equal("test@1", first)
        Assert.Equal("test@1-1", second)

    [<Fact>]
    let ``NiceNameGenerator without map keys ordinals by file index`` () =
        let compilerState = CompilerGlobalState()
        clearCompilerGeneratedNameMap (compilerState :> obj)
        let generator = compilerState.NiceNameGenerator
        let start = Position.mkPos 42 0
        let fileOneRange = Range.mkRange "/tmp/generated-names-file-one.fs" start start
        let fileTwoRange = Range.mkRange "/tmp/generated-names-file-two.fs" start start

        let fileOneFirst = generator.FreshCompilerGeneratedName("closure", fileOneRange)
        let fileOneSecond = generator.FreshCompilerGeneratedName("closure", fileOneRange)
        let fileTwoFirst = generator.FreshCompilerGeneratedName("closure", fileTwoRange)
        let fileOneThird = generator.FreshCompilerGeneratedName("closure", fileOneRange)

        Assert.Equal("closure@42", fileOneFirst)
        Assert.Equal("closure@42-1", fileOneSecond)
        Assert.Equal("closure@42", fileTwoFirst)
        Assert.Equal("closure@42-2", fileOneThird)

    [<Fact>]
    let ``IncrementOnly remains one-based and file-index scoped`` () =
        let compilerState = CompilerGlobalState()
        clearCompilerGeneratedNameMap (compilerState :> obj)
        let generator = compilerState.NiceNameGenerator
        let start = Position.mkPos 7 0
        let fileOneRange = Range.mkRange "/tmp/increment-only-one.fs" start start
        let fileTwoRange = Range.mkRange "/tmp/increment-only-two.fs" start start

        let first = generator.IncrementOnly("@T", fileOneRange)
        let second = generator.IncrementOnly("@T", fileOneRange)
        let third = generator.IncrementOnly("@T", fileTwoRange)

        Assert.Equal(1, first)
        Assert.Equal(2, second)
        Assert.Equal(1, third)
