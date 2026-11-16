// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Benchmarks

open System
open System.Collections.Generic
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open BenchmarkDotNet.Attributes
open FSharp.Benchmarks.Common.Categories

[<AutoOpen>]
module private SymbolAndIwsamHelpers =

    let mkSdkProjectOptions (tempDir: string) (filePath: string) (outDllName: string) =
        let checker = FSharpChecker.Create(projectCacheSize = 200)
        let probePath = Path.Combine(tempDir, "ResolveSdkProbe.fsx")

        let probeOptions, probeDiags =
            checker.GetProjectOptionsFromScript(probePath, SourceText.ofString "let x = 1", assumeDotNetFramework = false, useSdkRefs = true)
            |> Async.RunSynchronously

        let probeErrors =
            probeDiags |> List.filter (fun (d: FSharpDiagnostic) -> d.Severity = FSharpDiagnosticSeverity.Error)

        if not probeErrors.IsEmpty then
            failwithf "Script SDK resolution had errors: %A" probeErrors

        let refs = probeOptions.OtherOptions |> Array.filter (fun o -> o.StartsWith("-r:", StringComparison.Ordinal))
        let outDll = Path.Combine(tempDir, outDllName)

        {
            ProjectFileName = Path.Combine(tempDir, Path.ChangeExtension(outDllName, ".fsproj"))
            ProjectId = None
            SourceFiles = [| filePath |]
            OtherOptions =
                [|
                    yield "--noframework"
                    yield "--target:library"
                    yield "--optimize-"
                    yield "--out:" + outDll
                    yield! refs
                |]
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime(2020, 1, 1)
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = Some 0L
        }

    let mkSymbolAndInterfaceSource (variantLiteral: int) =
        let sb = System.Text.StringBuilder()
        sb.AppendLine("module Bench.Symbols") |> ignore
        sb.AppendLine("") |> ignore
        sb.AppendLine("open System") |> ignore
        sb.AppendLine("open System.Collections.Generic") |> ignore
        sb.AppendLine("") |> ignore
        sb.AppendLine("type IntList = int list") |> ignore
        sb.AppendLine("type Pair<'a, 'b> = 'a * 'b") |> ignore
        sb.AppendLine("type Lookup<'v> = IReadOnlyDictionary<string, 'v>") |> ignore
        sb.AppendLine("") |> ignore
        sb.AppendLine("type RecordItem =") |> ignore
        sb.AppendLine("    { Id: int") |> ignore
        sb.AppendLine("      Name: string") |> ignore
        sb.AppendLine("      Tags: IntList }") |> ignore
        sb.AppendLine("") |> ignore

        for i in 0..59 do
            let lit = if i = 59 then variantLiteral else (i + 1)
            sb.AppendLine($"let computeSym%02d{i}") |> ignore
            sb.AppendLine("    (d: IDisposable)") |> ignore
            sb.AppendLine("    (xs: IEnumerable<int>)") |> ignore
            sb.AppendLine("    (ys: IReadOnlyList<string>)") |> ignore
            sb.AppendLine("    (cmp: IComparable<int>)") |> ignore
            sb.AppendLine("    (eq: IEquatable<string>)") |> ignore
            sb.AppendLine("    (fmt: IFormattable)") |> ignore
            sb.AppendLine("    (pair: Pair<int, string>)") |> ignore
            sb.AppendLine("    (xs2: IntList)") |> ignore
            sb.AppendLine("    (lookup: Lookup<int>)") |> ignore
            sb.AppendLine("    : IntList =") |> ignore
            sb.AppendLine("    let c = cmp.CompareTo(xs2.Head)") |> ignore
            sb.AppendLine("    let _s = fmt.ToString(null, null)") |> ignore
            sb.AppendLine("    let m = if eq.Equals(ys[0]) then lookup.Count else 0") |> ignore
            sb.AppendLine($"    [ c; m; (if isNull d then 0 else 1); (xs |> Seq.length); fst pair; %d{lit} ]") |> ignore
            sb.AppendLine("") |> ignore

        sb.ToString()

[<MemoryDiagnoser>]
[<BenchmarkCategory(ShortCategory)>]
type SymbolAndIwsamBenchmarks() =

    let tempDir = Path.Combine(Path.GetTempPath(), $"fcs-sym-iwsam-{Guid.NewGuid():N}")
    let filePath = Path.Combine(tempDir, "SymbolAndIwsam.fs")
    let mutable checker = Unchecked.defaultof<FSharpChecker>
    let mutable options = Unchecked.defaultof<FSharpProjectOptions>
    let mutable sourceTextA = Unchecked.defaultof<ISourceText>
    let mutable sourceTextB = Unchecked.defaultof<ISourceText>
    let mutable checkedEntities: IList<FSharpEntity> = [||]
    let mutable step = 0

    [<GlobalSetup>]
    member this.Setup() =
        Directory.CreateDirectory(tempDir) |> ignore
        let srcA = mkSymbolAndInterfaceSource 101
        let srcB = mkSymbolAndInterfaceSource 202
        sourceTextA <- SourceText.ofString srcA
        sourceTextB <- SourceText.ofString srcB
        File.WriteAllText(filePath, srcA)

        checker <- FSharpChecker.Create(projectCacheSize = 200, keepAssemblyContents = true)
        options <- mkSdkProjectOptions tempDir filePath "SymbolAndIwsam.dll"

        let _, answer = checker.ParseAndCheckFileInProject(filePath, 0, sourceTextA, options) |> Async.RunSynchronously

        match answer with
        | FSharpCheckFileAnswer.Aborted -> failwith "Setup check aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

            if errors.Length > 0 then
                failwithf "Setup check had errors: %A" errors

            checkedEntities <- results.PartialAssemblySignature.Entities

            let memberCount = checkedEntities |> Seq.sumBy (fun e -> e.MembersFunctionsAndValues.Count)

            if memberCount < 60 then
                failwithf "Expected at least 60 module functions, got %d" memberCount

            if this.InspectSymbolsRepeatedly() <= 0 then
                failwith "Expected positive symbol inspection checksum"

    [<Benchmark>]
    member _.InspectSymbolsRepeatedly() : int =
        let mutable acc = 0

        for entity in checkedEntities do
            for m in entity.MembersFunctionsAndValues do
                for _pass in 1..3 do
                    let groups = m.CurriedParameterGroups

                    for group in groups do
                        for p in group do
                            let ty = p.Type

                            if ty.IsAbbreviation then
                                acc <- acc + 1

                            if ty.HasTypeDefinition || ty.IsTupleType || ty.IsFunctionType || ty.IsAnonRecordType then
                                acc <- acc + ty.GenericArguments.Count

                    let retTy = m.ReturnParameter.Type

                    if retTy.IsAbbreviation then
                        acc <- acc + 1

                    if retTy.HasTypeDefinition then
                        acc <- acc + 2

        acc

    [<Benchmark>]
    member _.WarmCheckFileWithInterfaces() : int =
        step <- step + 1
        let srcText = if step &&& 1 = 0 then sourceTextA else sourceTextB
        let _, answer = checker.ParseAndCheckFileInProject(filePath, step, srcText, options) |> Async.RunSynchronously

        match answer with
        | FSharpCheckFileAnswer.Aborted -> failwith "Check aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

            if errors.Length > 0 then
                failwithf "Check had errors: %A" errors

            results.Diagnostics.Length

    [<GlobalCleanup>]
    member _.Cleanup() =
        try
            Directory.Delete(tempDir, true)
        with _ ->
            ()
