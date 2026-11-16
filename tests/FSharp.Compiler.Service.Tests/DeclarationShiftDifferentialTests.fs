// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.DeclarationShiftDifferentialTests

open System
open System.Collections.Generic
open System.IO
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.Declarations
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.Service.Tests.Common
open Xunit

[<Struct>]
type private RangeKey =
    {
        File: string
        StartLine: int
        StartCol: int
        EndLine: int
        EndCol: int
    }

let private rangeKey (r: range) =
    {
        File = Path.GetFileName r.FileName
        StartLine = r.StartLine
        StartCol = r.StartColumn
        EndLine = r.EndLine
        EndCol = r.EndColumn
    }

let private fmtRange (r: range) =
    $"%s{Path.GetFileName r.FileName}(%d{r.StartLine},%d{r.StartColumn}-%d{r.EndLine},%d{r.EndColumn})"

let private fmtRangeOpt (r: range option) =
    match r with
    | Some m -> fmtRange m
    | None -> "<none>"

let private normalizeNewlines (s: string) = s.Replace("\r\n", "\n")

let private compilerRoot =
    Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "src", "Compiler"))

// Self-contained prefix of FSharp.Compiler.Service.fsproj (before generated string resources)
// covering Utilities and early Facilities modules/types with real production F# code.
let private prefixFiles =
    [|
        "Utilities/NullHelpers.fs"
        "Utilities/sformat.fsi"
        "Utilities/sformat.fs"
        "Utilities/sr.fsi"
        "Utilities/sr.fs"
        "Facilities/RichText.fsi"
        "Facilities/RichText.fs"
        "Utilities/Activity.fsi"
        "Utilities/Activity.fs"
        "Utilities/Caches.fsi"
        "Utilities/Caches.fs"
        "Utilities/illib.fsi"
        "Utilities/illib.fs"
        "Utilities/ResizeArray.fsi"
        "Utilities/ResizeArray.fs"
        "Utilities/HashMultiMap.fsi"
        "Utilities/HashMultiMap.fs"
        "Utilities/EditDistance.fsi"
        "Utilities/EditDistance.fs"
        "Utilities/TaggedCollections.fsi"
        "Utilities/TaggedCollections.fs"
        "Utilities/Cancellable.fsi"
        "Utilities/Cancellable.fs"
        "Utilities/FileSystem.fsi"
        "Utilities/FileSystem.fs"
        "Utilities/ildiag.fsi"
        "Utilities/ildiag.fs"
        "Utilities/zmap.fsi"
        "Utilities/zmap.fs"
        "Utilities/zset.fsi"
        "Utilities/zset.fs"
        "Utilities/XmlAdapters.fsi"
        "Utilities/XmlAdapters.fs"
        "Utilities/InternalCollections.fsi"
        "Utilities/InternalCollections.fs"
        "Utilities/QueueList.fsi"
        "Utilities/QueueList.fs"
        "Utilities/lib.fsi"
        "Utilities/lib.fs"
        "Utilities/DependencyGraph.fs"
        "Utilities/LruCache.fsi"
        "Utilities/LruCache.fs"
        "Utilities/ImmutableArray.fsi"
        "Utilities/ImmutableArray.fs"
        "Utilities/rational.fsi"
        "Utilities/rational.fs"
        "Utilities/PathMap.fsi"
        "Utilities/PathMap.fs"
        "Utilities/RidHelpers.fs"
        "Utilities/range.fsi"
        "Utilities/range.fs"
        "Utilities/ReadOnlySpan.fsi"
        "Utilities/ReadOnlySpan.fs"
        "Facilities/Hashing.fsi"
        "Facilities/Hashing.fs"
        "Facilities/BuildGraph.fsi"
        "Facilities/BuildGraph.fs"
    |]
    |> Array.map (fun rel -> Path.Combine(compilerRoot, rel.Replace('/', Path.DirectorySeparatorChar)))

// Diverse subset of real implementation files (paired with .fsi and unpaired) exercised by line insertions.
let private targetRelPaths =
    set
        [
            "Utilities/NullHelpers.fs"
            "Facilities/RichText.fs"
            "Utilities/Activity.fs"
            "Utilities/Caches.fs"
            "Utilities/ResizeArray.fs"
            "Utilities/HashMultiMap.fs"
            "Utilities/EditDistance.fs"
            "Utilities/Cancellable.fs"
            "Utilities/ildiag.fs"
            "Utilities/zmap.fs"
            "Utilities/zset.fs"
            "Utilities/InternalCollections.fs"
            "Utilities/QueueList.fs"
            "Utilities/DependencyGraph.fs"
            "Utilities/LruCache.fs"
            "Utilities/ImmutableArray.fs"
            "Utilities/rational.fs"
            "Utilities/PathMap.fs"
            "Utilities/range.fs"
            "Utilities/ReadOnlySpan.fs"
            "Facilities/Hashing.fs"
            "Facilities/BuildGraph.fs"
        ]

let private mkCompilerPrefixOptions (sourceFiles: string[]) =
    let projTemp = Path.Combine(Path.GetTempPath(), $"fcs-shift-diff-{Guid.NewGuid():N}")
    let dllName = Path.Combine(projTemp, "CompilerPrefix.dll")
    let projFile = Path.Combine(projTemp, "CompilerPrefix.fsproj")

    let extraArgs =
        [|
            yield "--define:COMPILER"
#if NETCOREAPP
            yield "--define:NET5_0_OR_GREATER"
            yield "--define:NET7_0_OR_GREATER"
            yield "--define:NET8_0_OR_GREATER"
#endif
            yield "--checknulls+"
            yield "--warnaserror-"
            yield "--nowarn:44;57;75;64;1204"
        |]

    let args = Array.append (mkProjectCommandLineArgsSilent (dllName, [||])) extraArgs
    { checker.GetProjectOptionsFromCommandLineArgs(projFile, args) with SourceFiles = sourceFiles }

let private checkAllDeclarations (ctx: ProjectContext) (before: FileSignature list) (parse: FSharpParseFileResults) =
    let start, env0 = DeclarationChecker.BeginFile(ctx, before, parse)
    let decls = DeclarationChecker.Split parse
    let results = Array.zeroCreate<DeclarationResult> decls.Length
    let mutable env = env0

    for i in 0 .. decls.Length - 1 do
        let r = DeclarationChecker.CheckDeclaration(start, env, decls[i], CancellationToken.None)
        results[i] <- r
        env <- DeclarationChecker.ExtendEnv(env, r)

    let fileRes = DeclarationChecker.FinishFile(start, results, parse)
    start, env0, decls, results, fileRes


let private diagKeys (diags: FSharpDiagnostic[]) =
    diags
    |> Array.map (fun d ->
        struct (rangeKey d.Range, d.Severity, d.ErrorNumber, d.Message))
    |> Array.sort

let private symbolUseKeys (results: FSharpCheckFileResults) =
    results.GetAllUsesOfAllSymbolsInFile()
    |> Seq.map (fun u ->
        match u.Symbol with
        | :? FSharpActivePatternCase as case ->
            $"%s{fmtRange u.Range} def=%b{u.IsFromDefinition} %s{u.Symbol.FullName}: tag %s{fmtRange case.DeclarationLocation}"
        | sym ->
            $"%s{fmtRange u.Range} def=%b{u.IsFromDefinition} %s{sym.FullName}: decl=%s{fmtRangeOpt sym.DeclarationLocation} sig=%s{fmtRangeOpt sym.SignatureLocation} impl=%s{fmtRangeOpt sym.ImplementationLocation}")
    |> Seq.sort
    |> Array.ofSeq

let private classificationKeys (results: FSharpCheckFileResults) =
    results.GetSemanticClassification(None)
    |> Array.map (fun item -> struct (rangeKey item.Range, int item.Type))
    |> Array.sort

let private fingerprintPairs (res: FileResult) =
    res.Fingerprints
    |> Seq.map (fun kv -> kv.Key, kv.Value)
    |> Seq.sortBy fst
    |> Array.ofSeq

type DeclarationShiftDifferentialTests(output: ITestOutputHelper) =

    [<Fact>]
    member _.``Line insertions across real compiler files match fresh whole-file checks``() =
        let opts = mkCompilerPrefixOptions prefixFiles
        let incCtx = DeclarationChecker.CreateProject opts |> Async.RunSynchronously
        let oracleCtx = DeclarationChecker.CreateProject opts |> Async.RunSynchronously

        let targetFiles =
            targetRelPaths
            |> Set.map (fun rel -> Path.GetFullPath(Path.Combine(compilerRoot, rel.Replace('/', Path.DirectorySeparatorChar))))

        let mutable incBefore: FileSignature list = []
        let mutable oracleBefore: FileSignature list = []

        let mutable filesTested = 0
        let mutable insertionSitesTested = 0
        let mutable declarationsShifted = 0
        let mutable declarationsRefused = 0
        let totalShiftMissesStart = ProjectContext.ShiftFinishMisses

        for filePath in prefixFiles do
            let rawText = File.ReadAllText filePath
            let normalizedText = normalizeNewlines rawText

            if filePath.EndsWith(".fsi", StringComparison.OrdinalIgnoreCase) then
                let incParse = DeclarationChecker.Parse(incCtx, filePath, normalizedText)
                let incSig = DeclarationChecker.CheckSignatureFile(incCtx, incBefore, incParse)
                Assert.False(incSig.Failed, $"Signature file failed to check: %s{filePath}")
                incBefore <- incBefore @ [ incSig ]

                let oraParse = DeclarationChecker.Parse(oracleCtx, filePath, normalizedText)
                let oraSig = DeclarationChecker.CheckSignatureFile(oracleCtx, oracleBefore, oraParse)
                Assert.False(oraSig.Failed, $"Oracle signature file failed to check: %s{filePath}")
                oracleBefore <- oracleBefore @ [ oraSig ]
            else
                let isTarget = targetFiles.Contains(Path.GetFullPath filePath)

                if not isTarget then
                    let incParse = DeclarationChecker.Parse(incCtx, filePath, normalizedText)
                    let incRes = DeclarationChecker.CheckFile(incCtx, incBefore, incParse)

                    let incErrors =
                        incRes.Diagnostics
                        |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

                    Assert.True(Array.isEmpty incErrors, $"Prefix file %s{filePath} had errors: %A{incErrors}")
                    incBefore <- incBefore @ [ incRes.Signature ]

                    let oraParse = DeclarationChecker.Parse(oracleCtx, filePath, normalizedText)
                    let oraRes = DeclarationChecker.CheckFile(oracleCtx, oracleBefore, oraParse)
                    oracleBefore <- oracleBefore @ [ oraRes.Signature ]
                else
                    filesTested <- filesTested + 1
                    let parse0 = DeclarationChecker.Parse(incCtx, filePath, normalizedText)
                    let start, env0, decls0, results, file0 = checkAllDeclarations incCtx incBefore parse0

                    let initErrors =
                        file0.Diagnostics
                        |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

                    Assert.True(Array.isEmpty initErrors, $"Target file %s{filePath} had initial errors: %A{initErrors}")
                    // Materialize CheckResults before shifting to exercise signature-conformance undo on .fsi pairs.
                    file0.CheckResults |> ignore

                    let mutable curText = normalizedText
                    let mutable curDecls = decls0
                    let mutable prevMaxEndLine0 = if decls0.Length > 0 then decls0[0].Range.EndLine else 0

                    for idx in 1 .. decls0.Length - 1 do
                        let curDecl = curDecls[idx]

                        if not curDecl.IsModuleHeader && curDecl.Range.StartLine > prevMaxEndLine0 then
                            let lines = curText.Split('\n')
                            // Walk up over contiguous XML doc lines above this declaration without crossing the prior declaration.
                            let mutable insertIdx0 = curDecl.Range.StartLine - 1

                            while insertIdx0 > prevMaxEndLine0
                                  && lines[insertIdx0 - 1].TrimStart().StartsWith("///", StringComparison.Ordinal) do
                                insertIdx0 <- insertIdx0 - 1

                            if insertIdx0 > prevMaxEndLine0 && insertIdx0 > 0 then
                                insertionSitesTested <- insertionSitesTested + 1
                                let editedLines = Array.insertAt insertIdx0 "" lines
                                let editedText = String.concat "\n" editedLines
                                let editedParse = DeclarationChecker.Parse(incCtx, filePath, editedText)
                                let editedDecls = DeclarationChecker.Split editedParse

                                Assert.Equal<DeclarationKey[]>(
                                    curDecls |> Array.map _.Key,
                                    editedDecls |> Array.map _.Key
                                )

                                let invalidated = HashSet<DeclarationRef>()
                                let invalidatedHeaders = HashSet<DeclarationKey>()
                                let envs = Array.zeroCreate<DeclarationEnvironment> editedDecls.Length
                                let mutable validEnvCount = 0

                                let getEnvBefore targetIdx =
                                    if validEnvCount = 0 then
                                        envs[0] <- env0
                                        validEnvCount <- 1

                                    while validEnvCount <= targetIdx do
                                        let prevIdx = validEnvCount - 1
                                        envs[validEnvCount] <- DeclarationChecker.ExtendEnv(envs[prevIdx], results[prevIdx])
                                        validEnvCount <- validEnvCount + 1

                                    envs[targetIdx]

                                let replaceResult m newRes =
                                    results[m] <- newRes

                                    if validEnvCount > m + 1 then
                                        validEnvCount <- m + 1

                                let recheckSingle m =
                                    let env = getEnvBefore m
                                    let r = DeclarationChecker.CheckDeclaration(start, env, editedDecls[m], CancellationToken.None)
                                    invalidated.Add results[m].Ref |> ignore

                                    if editedDecls[m].IsModuleHeader then
                                        invalidatedHeaders.Add editedDecls[m].Key |> ignore

                                    replaceResult m r

                                let recheckAtOrRollback m =
                                    let depStart =
                                        [ 0 .. m - 1 ]
                                        |> List.tryFind (fun d -> results[d].DependsOnLaterDeclarations)

                                    let augmented = DeclarationChecker.Content(editedDecls[m]).Augments

                                    let augStart =
                                        if augmented.Length = 0 then
                                            None
                                        else
                                            [ 0 .. m - 1 ]
                                            |> List.tryFind (fun d ->
                                                editedDecls[d].Within = editedDecls[m].Within
                                                && not (invalidated.Contains results[d].Ref)
                                                && (DeclarationChecker.Content(editedDecls[d]).Defines
                                                    |> Array.exists (fun defName -> Array.contains defName augmented)))

                                    match depStart, augStart with
                                    | Some dDep, Some dAug ->
                                        for k in min dDep dAug .. editedDecls.Length - 1 do
                                            recheckSingle k

                                        editedDecls.Length
                                    | Some dDep, None ->
                                        for k in dDep .. editedDecls.Length - 1 do
                                            recheckSingle k

                                        editedDecls.Length
                                    | None, Some dAug ->
                                        for k in dAug .. m do
                                            recheckSingle k

                                        m + 1
                                    | None, None ->
                                        recheckSingle m
                                        m + 1

                                let mutable j = 0

                                while j < editedDecls.Length do
                                    let oldDecl = curDecls[j]
                                    let newDecl = editedDecls[j]
                                    let prevRes = results[j]

                                    let needsRecheck =
                                        not (newDecl.IsUnchangedFrom oldDecl)
                                        || (match newDecl.Within with
                                            | Some h -> invalidatedHeaders.Contains h
                                            | None -> false)
                                        || (prevRes.TypedUses |> Array.exists invalidated.Contains)
                                        || (invalidated.Count > 0 && prevRes.DependsOnLaterDeclarations)

                                    if needsRecheck then
                                        j <- recheckAtOrRollback j
                                    elif newDecl.Range.StartLine = oldDecl.Range.StartLine then
                                        j <- j + 1
                                    else
                                        match DeclarationChecker.Shift(prevRes, newDecl) with
                                        | ValueSome shifted ->
                                            replaceResult j shifted
                                            declarationsShifted <- declarationsShifted + 1
                                            j <- j + 1
                                        | ValueNone ->
                                            declarationsRefused <- declarationsRefused + 1
                                            j <- recheckAtOrRollback j

                                let incFileRes = DeclarationChecker.FinishFile(start, results, editedParse)
                                let incCheckRes = incFileRes.CheckResults

                                let oracleParse = DeclarationChecker.Parse(oracleCtx, filePath, editedText)
                                let _, _, _, _, oracleFileRes = checkAllDeclarations oracleCtx oracleBefore oracleParse
                                let oracleCheckRes = oracleFileRes.CheckResults

                                let shortName = Path.GetFileName filePath
                                let site = $"%s{shortName} step decl[%d{idx}]=%s{curDecl.Key.Name} line %d{curDecl.Range.StartLine}"

                                let incFileDiags = diagKeys incFileRes.Diagnostics
                                let oraFileDiags = diagKeys oracleFileRes.Diagnostics

                                Assert.True(
                                    (incFileDiags = oraFileDiags),
                                    $"FileResult diagnostics differed at %s{site}:\ninc: %A{incFileDiags}\nora: %A{oraFileDiags}"
                                )

                                let incCheckDiags = diagKeys incCheckRes.Diagnostics
                                let oraCheckDiags = diagKeys oracleCheckRes.Diagnostics

                                Assert.True(
                                    (incCheckDiags = oraCheckDiags),
                                    $"CheckResults diagnostics differed at %s{site}:\ninc: %A{incCheckDiags}\nora: %A{oraCheckDiags}"
                                )

                                Assert.Equal(oracleFileRes.Signature.Fingerprint, incFileRes.Signature.Fingerprint)

                                let incPrints = fingerprintPairs incFileRes
                                let oraPrints = fingerprintPairs oracleFileRes

                                Assert.Equal(oraPrints.Length, incPrints.Length)

                                for (incKey, incPrint), (oraKey, oraPrint) in Array.zip incPrints oraPrints do
                                    Assert.Equal(oraKey, incKey)

                                    if not (incPrint.StartsWith("?", StringComparison.Ordinal) && oraPrint.StartsWith("?", StringComparison.Ordinal))
                                       && incPrint <> oraPrint then
                                        Assert.Fail(
                                            $"Per-declaration fingerprint differed at %s{site} for key %A{incKey}:\ninc: %s{incPrint}\nora: %s{oraPrint}"
                                        )

                                let incUses = symbolUseKeys incCheckRes
                                let oraUses = symbolUseKeys oracleCheckRes

                                if incUses <> oraUses then
                                    let incSet = HashSet<string>(incUses)
                                    let oraSet = HashSet<string>(oraUses)
                                    let missing = oraUses |> Array.filter (fun u -> not (incSet.Contains u)) |> Array.truncate 10
                                    let extra = incUses |> Array.filter (fun u -> not (oraSet.Contains u)) |> Array.truncate 10
                                    Assert.Fail(
                                        $"Symbol uses differed at %s{site}:\nMissing (%d{missing.Length} shown):\n%s{String.concat Environment.NewLine missing}\nExtra (%d{extra.Length} shown):\n%s{String.concat Environment.NewLine extra}"
                                    )

                                let incClass = classificationKeys incCheckRes
                                let oraClass = classificationKeys oracleCheckRes
                                Assert.True((incClass = oraClass), $"Semantic classification differed at %s{site}")

                                curText <- editedText
                                curDecls <- editedDecls

                        let updatedDecl = curDecls[idx]

                        if updatedDecl.Range.EndLine > prevMaxEndLine0 then
                            prevMaxEndLine0 <- updatedDecl.Range.EndLine

                    // Restore unedited source as the latest parse so downstream .fsi ImplementationLocation matches unedited files.
                    let restoreParse = DeclarationChecker.Parse(incCtx, filePath, normalizedText)
                    let restoreRes = DeclarationChecker.CheckFile(incCtx, incBefore, restoreParse)
                    restoreRes.CheckResults |> ignore
                    incBefore <- incBefore @ [ restoreRes.Signature ]

                    let oraOrigParse = DeclarationChecker.Parse(oracleCtx, filePath, normalizedText)
                    let oraOrigRes = DeclarationChecker.CheckFile(oracleCtx, oracleBefore, oraOrigParse)
                    oraOrigRes.CheckResults |> ignore
                    oracleBefore <- oracleBefore @ [ oraOrigRes.Signature ]

        let shiftMisses = ProjectContext.ShiftFinishMisses - totalShiftMissesStart
        Assert.Equal(0L, shiftMisses)
        Assert.True(filesTested >= 20, $"Expected at least 20 compiler files tested, got %d{filesTested}")
        Assert.True(insertionSitesTested >= 100, $"Expected at least 100 line-insertion sites tested, got %d{insertionSitesTested}")
        Assert.True(declarationsShifted >= 500, $"Expected at least 500 shifted declarations, got %d{declarationsShifted}")

        let summary =
            $"DeclarationShiftDifferential: filesTested=%d{filesTested}, insertionSitesTested=%d{insertionSitesTested}, declarationsShifted=%d{declarationsShifted}, declarationsRefused=%d{declarationsRefused}, shiftFinishMisses=%d{shiftMisses}"

        output.WriteLine summary
        Console.WriteLine summary
