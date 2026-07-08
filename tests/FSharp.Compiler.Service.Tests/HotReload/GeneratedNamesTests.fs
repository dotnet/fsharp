namespace FSharp.Compiler.Service.Tests.HotReload

open Xunit

open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.Text

module GeneratedNamesTests =

    let zeroRange = Range.range0

    let private expectPositionalName input expectedName expectedOrdinal =
        match tryNormalizeSynthesizedTypeNameForPositionalPairing input with
        | Some actual ->
            Assert.Equal(expectedName, actual.NormalizedBasicName)
            Assert.Equal<int list>(expectedOrdinal, actual.Ordinal)
        | None -> failwithf "Expected '%s' to normalize for positional pairing." input

    let private expectNoPositionalName input =
        Assert.True(
            Option.isNone (tryNormalizeSynthesizedTypeNameForPositionalPairing input),
            sprintf "Expected '%s' not to normalize for positional pairing." input
        )

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
            |> fun struct (_, names) -> names

        map.BeginSession()

        let replayFirst = generator.FreshCompilerGeneratedName("closure", zeroRange)
        let replaySecond = generator.FreshCompilerGeneratedName("closure", zeroRange)

        Assert.Equal("closure@hotreload", first)
        Assert.Equal("closure@hotreload-1", second)
        Assert.Equal<string[]>(snapshot, [| first; second |])
        Assert.Equal<string[]>(snapshot, [| replayFirst; replaySecond |])

    [<Fact>]
    let ``NiceNameGenerator counters not incremented during replay mode`` () =
        let compilerState = CompilerGlobalState()
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()
        setCompilerGeneratedNameMap (compilerState :> obj) (map :> ICompilerGeneratedNameMap)

        let generator = compilerState.NiceNameGenerator

        generator.FreshCompilerGeneratedName("test", zeroRange) |> ignore
        generator.FreshCompilerGeneratedName("test", zeroRange) |> ignore

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
    let ``PerFileNamingScope uses map before per-file buckets`` () =
        let compilerState = CompilerGlobalState()
        let map = FSharpSynthesizedTypeMaps()
        map.BeginSession()
        setCompilerGeneratedNameMap (compilerState :> obj) (map :> ICompilerGeneratedNameMap)

        let start = Position.mkPos 42 0
        let fileRange = Range.mkRange "/tmp/generated-names-file-scope.fs" start start
        let scope = compilerState.NewFileScope fileRange

        let first = scope.Fresh("closure", fileRange)
        let second = scope.Fresh("closure", fileRange)

        clearCompilerGeneratedNameMap (compilerState :> obj)
        let fallback = scope.Fresh("closure", fileRange)

        Assert.Equal("closure@hotreload", first)
        Assert.Equal("closure@hotreload-1", second)
        Assert.Equal("closure@42", fallback)

    [<Fact>]
    let ``Per-file naming scope remains one-based and file-index scoped`` () =
        let compilerState = CompilerGlobalState()
        clearCompilerGeneratedNameMap (compilerState :> obj)
        let start = Position.mkPos 7 0
        let fileOneRange = Range.mkRange "/tmp/per-file-scope-one.fs" start start
        let fileTwoRange = Range.mkRange "/tmp/per-file-scope-two.fs" start start

        let fileOneScope = compilerState.NewFileScope(fileOneRange)
        let fileTwoScope = compilerState.NewFileScope(fileTwoRange)

        let first = fileOneScope.Fresh("closure", fileOneRange)
        let second = fileOneScope.Fresh("closure", fileTwoRange)
        let third = fileTwoScope.Fresh("closure", fileOneRange)

        Assert.Equal("closure@7", first)
        Assert.Equal("closure@7-1", second)
        Assert.Equal("closure@7", third)

    [<Fact>]
    let ``positional synthesized name normalization recognizes pipe and ordinal labels`` () =
        expectPositionalName "Pipe #1 input at line 28@28" "Pipe #1 input" [ 28; 0 ]
        expectPositionalName "Pipe #1 stage #2 at line 28@28" "Pipe #1 stage #2" [ 28; 0 ]
        expectPositionalName "Pipe #1 stage #2 at line 28" "Pipe #1 stage #2" [ 28; 0 ]
        expectPositionalName "Pipe #1 stage #2 at line 28@hotreload-1" "Pipe #1 stage #2" [ 28; 1 ]
        expectPositionalName "endpoints@hotreload" "endpoints" [ 0 ]
        expectPositionalName "endpoints@hotreload-2" "endpoints" [ 2 ]
        expectPositionalName "endpoints@42-1" "endpoints" [ 42; 1 ]

    [<Fact>]
    let ``positional synthesized name normalization rejects unrelated generated-looking names`` () =
        expectNoPositionalName ""
        expectNoPositionalName "not generated"
        expectNoPositionalName "Pipe #1 stage #2 line 28@28"
        expectNoPositionalName "Pipe #1 stage #2 at line 28@29"
        expectNoPositionalName "endpoints@hotreload#g0_o0"

    [<Fact>]
    let ``generation-suffixed name parsing recognizes generation and occurrence`` () =
        Assert.True(IsHotReloadGenerationSuffixedName "f@hotreload#g2_o3_4")
        Assert.Equal(Some 2, TryGetHotReloadNameGeneration "f@hotreload#g2_o3_4")

        match TryNormalizeHotReloadGenerationName "Pipe #1 stage #2 at line 28@hotreload#g0_o1_2" with
        | Some actual ->
            Assert.Equal("Pipe #1 stage #2", actual.NormalizedBasicName)
            Assert.Equal(0, actual.Generation)
            Assert.Equal<int list>([ 1; 2 ], actual.OccurrenceOrdinal)
        | None -> failwith "Expected generation-suffixed name to normalize."

    [<Fact>]
    let ``generation-suffixed name parsing rejects malformed names`` () =
        Assert.Equal(None, TryGetHotReloadNameGeneration "")
        Assert.Equal(None, TryGetHotReloadNameGeneration "f@hotreload#g_o3")
        Assert.Equal(None, TryNormalizeHotReloadGenerationName "f@hotreload#g1_o")
        Assert.Equal(None, TryNormalizeHotReloadGenerationName "f@bad@hotreload#g1_o0")
