namespace FSharp.Compiler.Service.Tests.HotReload

open Xunit

open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.IlxDeltaEmitter
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
    let ``synthesized type shape guard compares structural surface`` () =
        let baselineShape =
            {
                GenericArity = 0
                BaseType = Some "class Microsoft.FSharp.Core.FSharpFunc`2"
                InterfaceTypes = [ "class System.IDisposable" ]
                FieldTypeNames = [ "class System.String"; "valuetype System.Int32" ]
                MethodNameAndArities = [ ".ctor", 0; "Invoke", 0 ]
            }

        Assert.True(Option.isNone (tryFindSynthesizedTypeShapeMismatch baselineShape baselineShape))

        let expectMismatch fragment freshShape =
            match tryFindSynthesizedTypeShapeMismatch baselineShape freshShape with
            | Some message -> Assert.Contains(fragment, message)
            | None -> failwithf "Expected shape mismatch containing '%s'." fragment

        expectMismatch "generic arity" { baselineShape with GenericArity = 1 }
        expectMismatch "base type" { baselineShape with BaseType = Some "class System.Object" }
        expectMismatch "interface set" { baselineShape with InterfaceTypes = [] }
        expectMismatch "field type multiset" { baselineShape with FieldTypeNames = [ "class System.String" ] }
        expectMismatch "method set" { baselineShape with MethodNameAndArities = [ ".ctor", 0 ] }
