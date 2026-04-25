module FcsIdeSmokeTest.Program

// Exercises IDE-style FCS APIs against an auto-ordered project to confirm
// IntelliSense, Go-to-Definition, Find All References, and the FS3885
// deprecation warning all flow through the IncrementalBuilder hook added
// in Track 05 Phase 2.

open System
open System.Diagnostics
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

let testDir =
    let d = Path.Combine(Path.GetTempPath(), "fcs-ide-fileorder-test")
    if Directory.Exists d then Directory.Delete(d, true)
    Directory.CreateDirectory d |> ignore
    d

let writeFile name (content: string) =
    let path = Path.Combine(testDir, name)
    File.WriteAllText(path, content)
    path

// FileB defines Test.B.value; FileA uses it; Main entry point uses FileA.
let bSource = """module Test.B
let value = 42
let greeting = "hi"
"""
let aSource = """module Test.A
let useB () = Test.B.value + 1
"""
let mainSource = """module Test.Main
[<EntryPoint>]
let main _ =
    printfn "%d" (Test.A.useB ())
    0
"""

let bPath = writeFile "FileB.fs" bSource
let aPath = writeFile "FileA.fs" aSource
let mainPath = writeFile "Main.fs" mainSource

// Wrong order on disk: A (uses B), B (defines), Main
let sourceFiles = [| aPath; bPath; mainPath |]

let checker = FSharpChecker.Create(keepAllBackgroundResolutions = true)

let projectOptions =
    { ProjectFileName = Path.Combine(testDir, "test.fsproj")
      ProjectId = None
      SourceFiles = sourceFiles
      OtherOptions = [|
          "--targetprofile:netcore"
          "--file-order-auto+"
      |]
      ReferencedProjects = [||]
      IsIncompleteTypeCheckEnvironment = false
      UseScriptResolutionRules = false
      LoadTime = DateTime.Now
      UnresolvedReferences = None
      OriginalLoadReferences = []
      Stamp = None }

let mutable failed = 0

let assertTrue label cond =
    if cond then
        printfn "  PASS: %s" label
    else
        printfn "  FAIL: %s" label
        failed <- failed + 1

let parseAndCheck (path: string) (source: string) =
    let sourceText = SourceText.ofString source
    checker.ParseAndCheckFileInProject(path, 0, sourceText, projectOptions)
    |> Async.RunSynchronously

[<EntryPoint>]
let main _ =
    printfn "=== Project-level check (sanity) ==="
    let proj = checker.ParseAndCheckProject(projectOptions) |> Async.RunSynchronously
    let projErrors =
        proj.Diagnostics
        |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
    assertTrue (sprintf "Project type-checks under --file-order-auto+ (errors=%d)" projErrors.Length) (projErrors.Length = 0)

    printfn ""
    printfn "=== IntelliSense: completions on `Test.B.` from FileA ==="
    // Probe completion as if the user typed `Test.B.` somewhere in FileA. We
    // synthesise a probe source that matches what an IDE would send mid-edit.
    let probeSource = aSource + "let probe = Test.B."
    let _, answer = parseAndCheck aPath probeSource
    match answer with
    | FSharpCheckFileAnswer.Aborted ->
        assertTrue "Type check returned results (not aborted)" false
    | FSharpCheckFileAnswer.Succeeded checkResults ->
        // The probe line is the last one; column = end of `Test.B.`.
        let probeLineNum =
            probeSource.Split('\n').Length  // 1-based last line
        let probeLine = "let probe = Test.B."
        // Column of the dot after "Test.B"
        let dotIdx = probeLine.LastIndexOf('.')
        let partialName = QuickParse.GetPartialLongNameEx(probeLine, dotIdx)
        let sw = Stopwatch.StartNew()
        let symbols =
            checkResults.GetDeclarationListSymbols(
                None, probeLineNum, probeLine, partialName)
        sw.Stop()
        let names =
            symbols
            |> List.collect id
            |> List.map (fun u -> u.Symbol.DisplayName)
            |> Set.ofList
        printfn "  completions returned in %dms (%d symbols)" sw.ElapsedMilliseconds (Set.count names)
        assertTrue "Completions include `value` from FileB" (Set.contains "value" names)
        assertTrue "Completions include `greeting` from FileB" (Set.contains "greeting" names)

    printfn ""
    printfn "=== Go-to-Definition: `Test.B.value` reference in FileA ==="
    let _, ansA = parseAndCheck aPath aSource
    match ansA with
    | FSharpCheckFileAnswer.Aborted ->
        assertTrue "FileA check returned results" false
    | FSharpCheckFileAnswer.Succeeded checkResults ->
        // Line 2 in FileA: `let useB () = Test.B.value + 1`
        let line = 2
        let lineText = "let useB () = Test.B.value + 1"
        // Column at end of `value` (the dotted identifier)
        let colEnd = lineText.IndexOf("value") + "value".Length
        let symUse = checkResults.GetSymbolUseAtLocation(line, colEnd, lineText, [ "Test"; "B"; "value" ])
        match symUse with
        | None -> assertTrue "GetSymbolUseAtLocation returned a use for Test.B.value" false
        | Some su ->
            let declRangeOpt =
                match su.Symbol with
                | :? FSharp.Compiler.Symbols.FSharpMemberOrFunctionOrValue as mfv -> mfv.DeclarationLocation |> Some
                | _ -> None
            match declRangeOpt with
            | None -> assertTrue "Symbol resolves to an MFV with a declaration location" false
            | Some r ->
                let targetIsFileB =
                    Path.GetFullPath(r.FileName).Equals(Path.GetFullPath(bPath), StringComparison.OrdinalIgnoreCase)
                assertTrue (sprintf "Definition lives in FileB.fs (got %s line %d)" (Path.GetFileName r.FileName) r.StartLine) targetIsFileB

    printfn ""
    printfn "=== Find All References: `Test.B.value` across project ==="
    let _, ansB = parseAndCheck bPath bSource
    match ansB with
    | FSharpCheckFileAnswer.Aborted ->
        assertTrue "FileB check returned results" false
    | FSharpCheckFileAnswer.Succeeded checkResults ->
        // Line 2 in FileB: `let value = 42`
        let lineText = "let value = 42"
        let colEnd = lineText.IndexOf("value") + "value".Length
        let symUse = checkResults.GetSymbolUseAtLocation(2, colEnd, lineText, [ "value" ])
        match symUse with
        | None -> assertTrue "GetSymbolUseAtLocation found Test.B.value definition" false
        | Some su ->
            printfn "  symbol: %s (full=%s)" su.Symbol.DisplayName su.Symbol.FullName
            let allUses = proj.GetUsesOfSymbol(su.Symbol)
            let byFile =
                allUses
                |> Array.groupBy (fun u -> Path.GetFileName u.Range.FileName)
                |> Map.ofArray
            for kvp in byFile do
                printfn "  %s: %d use(s)" kvp.Key kvp.Value.Length
            let refsInB = byFile |> Map.tryFind "FileB.fs" |> Option.map Array.length |> Option.defaultValue 0
            let refsInA = byFile |> Map.tryFind "FileA.fs" |> Option.map Array.length |> Option.defaultValue 0
            assertTrue "FindReferences hits FileB (definition site)" (refsInB >= 1)
            assertTrue "FindReferences hits FileA (use site)" (refsInA >= 1)

    printfn ""
    printfn "=== FS3885: `and` keyword deprecation under --file-order-auto+ ==="
    // Stand-up a separate single-file project to keep the deprecation case isolated.
    let andDir =
        let d = Path.Combine(Path.GetTempPath(), "fcs-ide-and-test")
        if Directory.Exists d then Directory.Delete(d, true)
        Directory.CreateDirectory d |> ignore
        d
    let andSource = """module AndTest
type Tree =
    | Leaf
    | Branch of Forest
and Forest = Tree list
"""
    let andPath = Path.Combine(andDir, "AndTest.fs")
    File.WriteAllText(andPath, andSource)
    let andOptions =
        { projectOptions with
            ProjectFileName = Path.Combine(andDir, "and.fsproj")
            SourceFiles = [| andPath |] }
    let andProj = checker.ParseAndCheckProject(andOptions) |> Async.RunSynchronously
    let warnings =
        andProj.Diagnostics
        |> Array.filter (fun d -> d.ErrorNumber = 3885)
    printfn "  FS3885 warnings: %d" warnings.Length
    assertTrue "FS3885 surfaces under auto-order when `and` is used" (warnings.Length >= 1)

    printfn ""
    if failed = 0 then
        printfn "ALL IDE SMOKE CHECKS PASSED"
        0
    else
        printfn "FAILURES: %d" failed
        1
