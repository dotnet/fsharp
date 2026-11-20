// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Benchmarks

open System
open System.IO
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization
open FSharp.Compiler.Xml
open BenchmarkDotNet.Attributes
open FSharp.Benchmarks.Common.Categories

[<AutoOpen>]
module private TooltipPartsHelpers =

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

    let mkTooltipSource (docLabel: string) =
        let sb = StringBuilder()
        sb.AppendLine("module Bench.TooltipParts") |> ignore
        sb.AppendLine("") |> ignore

        for i in 0..59 do
            let doc = if i = 30 then docLabel else $"Step %02d{i}"
            sb.AppendLine($"/// {doc}") |> ignore

            if i = 30 then
                sb.AppendLine($"let compute%02d{i} (x: int) =") |> ignore
                sb.AppendLine("    let items = [ x; x + 1; x + 2 ] |> List.map (fun v -> v * 2 + 101)") |> ignore
                sb.AppendLine("    items |> List.sum") |> ignore
            elif i >= 31 && i <= 35 then
                sb.AppendLine($"let compute%02d{i} (x: int) =") |> ignore
                sb.AppendLine("    let baseVal = compute30 x") |> ignore
                sb.AppendLine($"    let items = [ baseVal; x + %d{i}; x + %d{i + 1} ]") |> ignore
                sb.AppendLine("    items |> List.sum") |> ignore
            else
                sb.AppendLine($"let compute%02d{i} (x: int) =") |> ignore
                sb.AppendLine($"    let items = [ x; x + %d{i + 1}; x + %d{i + 2} ] |> List.map (fun v -> v * 2 + 1)") |> ignore
                sb.AppendLine("    items |> List.sum") |> ignore

            sb.AppendLine("") |> ignore

        sb.ToString()

    let tryFindBindingDocInParse (parse: FSharpParseFileResults) (owner: FSharpDocOwner) : XmlDoc voption =
        if not (String.Equals(owner.FileName, parse.FileName, StringComparison.OrdinalIgnoreCase)) then
            ValueNone
        else
            let targetPath = owner.Path
            let mutable found = ValueNone

            let rec searchDecls (modulePrefix: string) (decls: SynModuleDecl list) =
                for decl in decls do
                    if found.IsNone then
                        match decl with
                        | SynModuleDecl.Let(bindings = bindings) ->
                            for SynBinding(xmlDoc = xmlDoc; headPat = pat) in bindings do
                                if found.IsNone then
                                    match pat with
                                    | SynPat.LongIdent(longDotId = SynLongIdent(id = ident :: _)) ->
                                        let candidatePath = "V:" + modulePrefix + "." + ident.idText

                                        if String.Equals(candidatePath, targetPath, StringComparison.Ordinal) then
                                            found <- ValueSome(xmlDoc.ToXmlDoc(false, None))
                                    | _ -> ()
                        | SynModuleDecl.NestedModule(moduleInfo = compInfo; decls = innerDecls) ->
                            let nested = compInfo.LongIdent |> List.map _.idText |> String.concat "."
                            let nextPrefix = if String.IsNullOrEmpty modulePrefix then nested else modulePrefix + "." + nested
                            searchDecls nextPrefix innerDecls
                        | _ -> ()

            match parse.ParseTree with
            | ParsedInput.ImplFile(ParsedImplFileInput(contents = modules)) ->
                for SynModuleOrNamespace(longId = lid; decls = decls) in modules do
                    let modPath = lid |> List.map _.idText |> String.concat "."
                    searchDecls modPath decls
            | _ -> ()

            found

    let docXmlTextOfTooltip (ToolTipText elements) (composeOpt: (FSharpDocSource -> FSharpXmlDoc) option) (sources: FSharpDocSource option list) =
        let mutable rest = sources
        let sb = StringBuilder()

        let next () =
            match rest with
            | head :: tail ->
                rest <- tail
                head
            | [] -> None

        for elem in elements do
            match elem with
            | ToolTipElement.Group items ->
                for item in items do
                    let xmlDoc =
                        match composeOpt, next () with
                        | Some compose, Some src -> compose src
                        | _ -> item.XmlDoc

                    match xmlDoc with
                    | FSharpXmlDoc.FromXmlText doc -> sb.Append(doc.GetXmlText()) |> ignore
                    | FSharpXmlDoc.FromXmlFile(_, xmlSig) -> sb.Append(xmlSig) |> ignore
                    | FSharpXmlDoc.None -> ()
            | _ -> ()

        sb.ToString()

[<MemoryDiagnoser>]
[<BenchmarkCategory(ShortCategory)>]
type TooltipPartsBenchmarks() =

    let tempDir = Path.Combine(Path.GetTempPath(), $"fcs-tooltip-bench-{Guid.NewGuid():N}")
    let wholeFilePath = Path.Combine(tempDir, "WholeTooltipBench.fs")
    let partsFilePath = Path.Combine(tempDir, "PartsTooltipBench.fs")

    let mutable wholeChecker = Unchecked.defaultof<FSharpChecker>
    let mutable partsChecker = Unchecked.defaultof<FSharpChecker>
    let mutable wholeOptions = Unchecked.defaultof<FSharpProjectOptions>
    let mutable partsParsingOptions = Unchecked.defaultof<FSharpParsingOptions>

    let mutable commentTextA = Unchecked.defaultof<ISourceText>
    let mutable commentTextB = Unchecked.defaultof<ISourceText>
    let mutable baseCheckResults = Unchecked.defaultof<FSharpCheckFileResults>

    let mutable hoverLine = 0
    let mutable hoverColEnd = 0
    let mutable hoverLineText = ""

    let mutable wholeStep = 0
    let mutable partsStep = 0

    [<GlobalSetup>]
    member _.Setup() =
        Directory.CreateDirectory(tempDir) |> ignore

        let commentA = mkTooltipSource "Summary Alpha for compute30."
        let commentB = mkTooltipSource "Summary Beta for compute30."
        commentTextA <- SourceText.ofString commentA
        commentTextB <- SourceText.ofString commentB

        File.WriteAllText(wholeFilePath, commentA)
        File.WriteAllText(partsFilePath, commentA)

        let lines = commentA.Replace("\r\n", "\n").Split('\n')
        let callLineIdx = lines |> Array.findIndex (fun l -> l.Contains("compute30 x", StringComparison.Ordinal))
        hoverLine <- callLineIdx + 1
        hoverLineText <- lines[callLineIdx]
        hoverColEnd <- hoverLineText.IndexOf("compute30", StringComparison.Ordinal) + "compute30".Length

        wholeChecker <- FSharpChecker.Create(projectCacheSize = 200, keepAssemblyContents = true)
        partsChecker <- FSharpChecker.Create(projectCacheSize = 200, keepAssemblyContents = true)
        wholeOptions <- mkSdkProjectOptions tempDir wholeFilePath "WholeTooltipBench.dll"
        let partsOptions = mkSdkProjectOptions tempDir partsFilePath "PartsTooltipBench.dll"
        let parsedOpts, _ = partsChecker.GetParsingOptionsFromProjectOptions(partsOptions)
        partsParsingOptions <- parsedOpts

        match wholeChecker.ParseAndCheckFileInProject(wholeFilePath, 0, commentTextA, wholeOptions) |> Async.RunSynchronously with
        | _, FSharpCheckFileAnswer.Succeeded r when r.Diagnostics.Length = 0 -> ()
        | _, other -> failwithf "Warmup wholeChecker failed: %A" other

        match partsChecker.ParseAndCheckFileInProject(partsFilePath, 0, commentTextA, partsOptions) |> Async.RunSynchronously with
        | _, FSharpCheckFileAnswer.Succeeded r when r.Diagnostics.Length = 0 -> baseCheckResults <- r
        | _, other -> failwithf "Warmup partsChecker failed: %A" other

        let parseB =
            partsChecker.ParseFile(partsFilePath, commentTextB, partsParsingOptions, cache = false)
            |> Async.RunSynchronously

        let tip, sources =
            baseCheckResults.GetToolTipParts(hoverLine, hoverColEnd, hoverLineText, [ "compute30" ], FSharpTokenTag.Identifier)

        let recomposed =
            docXmlTextOfTooltip tip (Some(fun src -> src.Compose(tryFindBindingDocInParse parseB))) sources

        if not (recomposed.Contains("Summary Beta", StringComparison.Ordinal)) then
            failwithf "Expected GetToolTipParts to recompose updated comment 'Summary Beta', got: %s" recomposed

    [<Benchmark(Baseline = true)>]
    member _.CommentEdit_WholeFile_GetToolTip() : int =
        wholeStep <- wholeStep + 1
        let srcText = if wholeStep &&& 1 = 1 then commentTextB else commentTextA

        let _, answer =
            wholeChecker.ParseAndCheckFileInProject(wholeFilePath, wholeStep, srcText, wholeOptions)
            |> Async.RunSynchronously

        match answer with
        | FSharpCheckFileAnswer.Aborted -> failwith "Whole-file tooltip check aborted"
        | FSharpCheckFileAnswer.Succeeded res ->
            let (ToolTipText elements) =
                res.GetToolTip(hoverLine, hoverColEnd, hoverLineText, [ "compute30" ], FSharpTokenTag.Identifier)

            elements.Length

    [<Benchmark>]
    member _.CommentEdit_ParseOnly_GetToolTipParts() : int =
        partsStep <- partsStep + 1
        let srcText = if partsStep &&& 1 = 1 then commentTextB else commentTextA

        let parseRes =
            partsChecker.ParseFile(partsFilePath, srcText, partsParsingOptions, cache = false)
            |> Async.RunSynchronously

        let tip, sources =
            baseCheckResults.GetToolTipParts(hoverLine, hoverColEnd, hoverLineText, [ "compute30" ], FSharpTokenTag.Identifier)

        let text =
            docXmlTextOfTooltip tip (Some(fun src -> src.Compose(tryFindBindingDocInParse parseRes))) sources

        text.Length

    [<GlobalCleanup>]
    member _.Cleanup() =
        try
            Directory.Delete(tempDir, true)
        with _ ->
            ()
