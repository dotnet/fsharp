module FSharp.Compiler.Service.Tests.HotReload.ArchitectureGuardTests

open System.IO
open Xunit

let private repoRoot =
    Path.Combine(__SOURCE_DIRECTORY__, "../../..") |> Path.GetFullPath

let private readCompilerFile relativePath =
    Path.Combine(repoRoot, relativePath) |> File.ReadAllText

[<Fact>]
let ``fsc does not directly depend on hot reload implementation modules`` () =
    let source = readCompilerFile "src/Compiler/Driver/fsc.fs"

    Assert.DoesNotContain("open FSharp.Compiler.HotReload\n", source)
    Assert.DoesNotContain("open FSharp.Compiler.HotReloadBaseline\n", source)
    Assert.DoesNotContain("open FSharp.Compiler.HotReloadPdb\n", source)
    Assert.DoesNotContain("open FSharp.Compiler.HotReloadEmitHook\n", source)
    Assert.DoesNotContain("open FSharp.Compiler.CompilerEmitHookState\n", source)
    Assert.Contains("open FSharp.Compiler.CompilerEmitHookBootstrap\n", source)

[<Fact>]
let ``compiler global state only depends on generated-name abstraction`` () =
    let source = readCompilerFile "src/Compiler/TypedTree/CompilerGlobalState.fs"

    Assert.DoesNotContain("open FSharp.Compiler.SynthesizedTypeMaps\n", source)
    Assert.Contains("open FSharp.Compiler.GeneratedNames\n", source)
    Assert.DoesNotContain("member _.CompilerGeneratedNameMap", source)
    Assert.Contains("tryGetCompilerGeneratedNameMap", source)

[<Fact>]
let ``compiler config exposes generic emit hook contract only`` () =
    let source = readCompilerFile "src/Compiler/Driver/CompilerConfig.fsi"

    Assert.DoesNotContain("IHotReloadEmitHook", source)
    Assert.DoesNotContain("HotReloadEmitArtifacts", source)
    Assert.DoesNotContain("setAmbientCompilerEmitHook", source)
    Assert.DoesNotContain("clearAmbientCompilerEmitHook", source)
    Assert.DoesNotContain("resolveCompilerEmitHook", source)
    Assert.Contains("type ICompilerEmitHook", source)
    Assert.Contains("val defaultCompilerEmitHook", source)

[<Fact>]
let ``compiler emit hook bootstrap remains explicit-only`` () =
    let source = readCompilerFile "src/Compiler/Driver/CompilerEmitHookBootstrap.fs"

    Assert.Contains("tcConfigB.compilerEmitHook <- Some hotReloadCompilerEmitHook", source)
    Assert.DoesNotContain("setAmbientCompilerEmitHook", source)

[<Fact>]
let ``hot reload service no longer mutates ambient emit-hook state`` () =
    let source = readCompilerFile "src/Compiler/Service/service.fs"

    Assert.DoesNotContain("createHotReloadCompilerEmitHook", source)
    Assert.DoesNotContain("setAmbientCompilerEmitHook", source)
    Assert.DoesNotContain("clearAmbientCompilerEmitHook", source)

[<Fact>]
let ``checker compile injects explicit hook-only argument for active hot reload sessions`` () =
    let source = readCompilerFile "src/Compiler/Service/service.fs"

    Assert.Contains("--enable:hotreloadhook", source)
    Assert.Contains("ensureHotReloadSessionHookArgument", source)

[<Fact>]
let ``hot reload checker path uses service-owned enc instance`` () =
    let source = readCompilerFile "src/Compiler/Service/service.fs"

    Assert.Contains("let editAndContinueService = FSharpEditAndContinueLanguageService(sessionStore)", source)
    Assert.DoesNotContain("FSharpEditAndContinueLanguageService.Instance", source)

let private sliceBetween (source: string) (startMarker: string) (endMarker: string) =
    let startIndex = source.IndexOf(startMarker, System.StringComparison.Ordinal)
    Assert.True(startIndex >= 0, $"Could not find marker '{startMarker}'.")

    let endIndex = source.IndexOf(endMarker, startIndex, System.StringComparison.Ordinal)
    Assert.True(endIndex > startIndex, $"Could not find end marker '{endMarker}' after '{startMarker}'.")

    source.Substring(startIndex, endIndex - startIndex)

[<Fact>]
let ``typed tree diff opDigest stays wildcard free`` () =
    let source = readCompilerFile "src/Compiler/TypedTree/TypedTreeDiff.fs"
    let opDigestSource = sliceBetween source "let private opDigest" "type private LoweredShapeCollector"

    Assert.DoesNotContain("| _ ->", opDigestSource)

[<Fact>]
let ``typed tree diff no longer relies on state-machine declaring-type string heuristic`` () =
    let source = readCompilerFile "src/Compiler/TypedTree/TypedTreeDiff.fs"

    Assert.DoesNotContain("isLikelyStateMachineDeclaringType", source)
    Assert.DoesNotContain("\"AsyncBuilder\"", source)
    Assert.DoesNotContain("\"TaskBuilder\"", source)
    Assert.DoesNotContain("\"Resumable\"", source)
    Assert.DoesNotContain("\"QueryBuilder\"", source)

[<Fact>]
let ``typed tree diff uses structural lowered-shape evidence only`` () =
    let source = readCompilerFile "src/Compiler/TypedTree/TypedTreeDiff.fs"

    Assert.Contains("if vref.LogicalName.Equals(\"MoveNext\", StringComparison.Ordinal) then", source)
    Assert.Contains("traitConstraintShapeDigest denv traitInfo", source)
    Assert.Contains("formatLoweredShapeDigest", source)
    Assert.Contains("hasLoweredShapeDigestSegmentValues", source)
    Assert.DoesNotContain("isLikelyQueryOperationName", source)
    Assert.DoesNotContain("isLikelyStateMachineOperationName", source)
    Assert.DoesNotContain("heuristic=[", source)
    Assert.DoesNotContain("vref.IsModuleBinding", source)

[<Fact>]
let ``compiler emit hook state no longer carries ambient mutable hook`` () =
    let source = readCompilerFile "src/Compiler/Driver/CompilerEmitHookState.fs"

    Assert.DoesNotContain("ambientCompilerEmitHook", source)
    Assert.DoesNotContain("setAmbientCompilerEmitHook", source)
    Assert.DoesNotContain("clearAmbientCompilerEmitHook", source)
    Assert.Contains("Option.defaultValue defaultCompilerEmitHook", source)

[<Fact>]
let ``driver hot reload implementation references stay behind boundary files`` () =
    let driverDir = Path.Combine(repoRoot, "src/Compiler/Driver")

    let allowlist =
        set
            [ "CompilerEmitHookBootstrap.fs"
              "CompilerEmitHookState.fs"
              "HotReloadEmitHook.fs" ]

    let forbiddenPatterns =
        [ "open FSharp.Compiler.HotReload\n"
          "open FSharp.Compiler.HotReloadBaseline\n"
          "open FSharp.Compiler.HotReloadPdb\n"
          "open FSharp.Compiler.HotReloadEmitHook\n"
          "open FSharp.Compiler.HotReloadState\n"
          "FSharp.Compiler.HotReload."
          "FSharp.Compiler.HotReloadState."
          "FSharpEditAndContinueLanguageService.Instance" ]

    for path in Directory.GetFiles(driverDir, "*.fs") do
        let fileName = Path.GetFileName(path)

        if not (allowlist.Contains fileName) then
            let source = File.ReadAllText(path)

            for pattern in forbiddenPatterns do
                Assert.DoesNotContain(pattern, source)

[<Fact>]
let ``ilx delta emitter phases stay explicit`` () =
    let source = readCompilerFile "src/Compiler/CodeGen/IlxDeltaEmitter.fs"
    let emitDeltaSource =
        sliceBetween
            source
            "let emitDelta (request: IlxDeltaRequest) : IlxDelta ="
            "        let typeReferenceRowList, memberReferenceRowList, assemblyReferenceRowList ="

    Assert.Contains("let private buildMethodAndParameterRows", source)
    Assert.Contains("let private buildPropertyEventAndSemanticsRows", source)
    Assert.Contains("let private buildCustomAttributeRows", source)
    Assert.Contains("let private finalizeDeltaArtifacts", source)
    Assert.Contains("let private buildAddedOrChangedMethods", source)
    Assert.Contains("let private buildDeltaToUpdatedMethodTokenMap", source)
    Assert.Contains("let private createDefinitionTokenRemapper", source)
    Assert.Contains("let private createMetadataReferenceRemapper", source)
    Assert.Contains("let definitionTokenRemapper =", emitDeltaSource)
    Assert.Contains("createDefinitionTokenRemapper", emitDeltaSource)
    Assert.Contains("let metadataReferenceRemapper =", emitDeltaSource)
    Assert.Contains("createMetadataReferenceRemapper", emitDeltaSource)
    Assert.Contains("RemapDefinitionToken = definitionTokenRemapper.RemapDefinitionToken", emitDeltaSource)
    Assert.Contains("definitionTokenRemapper.RemapPropertyAssociationToken", emitDeltaSource)
    Assert.Contains("definitionTokenRemapper.RemapEventAssociationToken", emitDeltaSource)
    Assert.Contains("let remapEntityToken = metadataReferenceRemapper.RemapEntityToken", emitDeltaSource)
    Assert.Contains("let remapAssemblyRefToken = metadataReferenceRemapper.RemapAssemblyRefToken", emitDeltaSource)
    Assert.Contains("buildMethodAndParameterRows", emitDeltaSource)
    Assert.Contains("buildPropertyEventAndSemanticsRows", emitDeltaSource)
    Assert.Contains("buildCustomAttributeRows", emitDeltaSource)
    Assert.Contains("        finalizeDeltaArtifacts", source)

[<Fact>]
let ``metadata reference remap context stays reference-focused`` () =
    let source = readCompilerFile "src/Compiler/CodeGen/IlxDeltaEmitter.fs"
    let contextSource =
        sliceBetween
            source
            "type private MetadataReferenceRemapContext ="
            "let private createMetadataReferenceRemapper"

    Assert.Contains("RemapDefinitionToken: int -> int", contextSource)
    Assert.DoesNotContain("TypeTokenMap: Dictionary<int, int>", contextSource)
    Assert.DoesNotContain("FieldTokenMap: Dictionary<int, int>", contextSource)
    Assert.DoesNotContain("MethodTokenMap: Dictionary<int, int>", contextSource)
    Assert.DoesNotContain("PropertyTokenMap: Dictionary<int, int>", contextSource)
    Assert.DoesNotContain("EventTokenMap: Dictionary<int, int>", contextSource)

[<Fact>]
let ``delta builder fallback keeps staged signature disambiguation`` () =
    let source = readCompilerFile "src/Compiler/HotReload/DeltaBuilder.fs"

    Assert.Contains("methodKeyMatchesSymbol symbol key", source)
    Assert.Contains("let parameterMatchedCandidates =", source)
    Assert.Contains("let returnMatchedCandidates =", source)
    Assert.Contains("normalizeSymbolParameterTypeIdentities", source)
    Assert.Contains("if List.isEmpty resolvedTypeTokens then", source)
    Assert.Contains("resolvedTypeNames |> List.exists (typeNamesEquivalent key.DeclaringType)", source)
    Assert.Contains("Ok rawCandidates", source)
    Assert.DoesNotContain("| _ -> MethodResolved", source)
