module FSharp.Compiler.Service.Tests.WarnScopeTests

open Xunit
open FSharp.Test
open FSharp.Test.Assert
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text

module private ProjectForNoWarnHashDirective =

    let fileSource1 = """
module N.M
#nowarn "40"
let rec f = new System.EventHandler(fun _ _ -> f.Invoke(null,null))
"""
    
    let createOptions() = createProjectOptions [fileSource1] []
    
[<Fact>]
let ``Test NoWarn HashDirective`` () =
    let options = ProjectForNoWarnHashDirective.createOptions()
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate

    for e in wholeProjectResults.Diagnostics do
        printfn "ProjectForNoWarnHashDirective error: <<<%s>>>" e.Message

    wholeProjectResults.Diagnostics.Length |> shouldEqual 0

let private sourceForParseError = """
module N.M
#nowarn 0xy
()
"""
    
[<Fact>]
let ``RegressionTestForMissingParseError(TransparentCompiler)`` () =
    let options = createProjectOptions [sourceForParseError] []
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate
    wholeProjectResults.Diagnostics.Length |> shouldEqual 1
    wholeProjectResults.Diagnostics.[0].ErrorNumber |> shouldEqual 203
    wholeProjectResults.Diagnostics.[0].Range.StartLine |> shouldEqual 3

[<Fact>]
let ``RegressionTestForDuplicateParseError(BackgroundCompiler)`` () =
    let options = createProjectOptions [sourceForParseError] []
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    let sourceName = options.SourceFiles[0]
    let _wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate
    let _, checkResults = exprChecker.GetBackgroundCheckResultsForFileInProject(sourceName, options) |> Async.RunImmediate
    checkResults.Diagnostics.Length |> shouldEqual 1
    checkResults.Diagnostics.[0].ErrorNumber |> shouldEqual 203
    checkResults.Diagnostics.[0].Range.StartLine |> shouldEqual 3

type private Expected =
    | Err of errorNumber: int * lineNumber: int
    | Warn of errorNumber: int * lineNumber: int
type private TestDef = {source: string; errors: Map<string, Expected list>}

let private justNowarnTest = {source = """
module N.M
""
#nowarn "20"
""
""
""
()
"""; errors = Map["9.0", [Warn(20, 3)]; "preview", [Warn(20, 3)]]}

let private noNowarnTest = {source = """
module N.M
""
""
""
""
()
"""; errors = Map[
    "9.0", [Warn(20, 3); Warn(20, 4); Warn(20, 5); Warn(20, 6)]
    "preview", [Warn(20, 3); Warn(20, 4); Warn(20, 5); Warn(20, 6)
    ]]}

let private onOffTest = {source = """
module N.M
""
#nowarn "20"
""
#warnon "20"
""
#nowarn "20"
""
()
"""; errors = Map["9.0", [Err(3350, 6); Warn(20, 3)]; "preview", [Warn(20, 3); Warn(20, 7)]]}

let mkProjectOptionsAndChecker langVersion =
    let options = createProjectOptions [onOffTest.source] [$"--langversion:{langVersion}"]
    let checker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    options, checker

let private checkDiagnostics (expected: Expected list) (diagnostics: FSharpDiagnostic list) =
    let fail() =
        printfn $"expected:"
        for exp in expected do printfn $"{exp}"
        printfn $"actual:"
        for diag in diagnostics do printfn $"{diag.Severity} {diag.ErrorNumber} {diag.StartLine}"
        Assert.Fail "unexpected diagnostics"
    let unexpected(exp, diag: FSharpDiagnostic) =
        match exp with
        | Err(errno, line) ->
            diag.Severity <> FSharpDiagnosticSeverity.Error || errno <> diag.ErrorNumber || line <> diag.StartLine
        | Warn(errno, line) ->
            diag.Severity <> FSharpDiagnosticSeverity.Warning || errno <> diag.ErrorNumber || line <> diag.StartLine
    if diagnostics.Length <> expected.Length then fail()
    elif List.exists unexpected (List.zip expected diagnostics) then fail()

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ParseAndCheckProjectTest langVersion =
    let options, checker = mkProjectOptionsAndChecker langVersion
    let wholeProjectResults = checker.ParseAndCheckProject(options) |> Async.RunImmediate
    checkDiagnostics onOffTest.errors[langVersion] (Array.toList wholeProjectResults.Diagnostics)
    
[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ParseAndCheckFileInProjectTest langVersion =
    let options, checker = mkProjectOptionsAndChecker langVersion
    let sourceName = options.SourceFiles[0]
    let parseAndCheckFileInProject testDef =
        let source = SourceText.ofString testDef.source
        let _, checkAnswer = checker.ParseAndCheckFileInProject(sourceName, 0,  source, options) |> Async.RunImmediate
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Expected error, got Aborted")
        | FSharpCheckFileAnswer.Succeeded checkResults ->
            checkDiagnostics testDef.errors[langVersion] (Array.toList checkResults.Diagnostics)
    [justNowarnTest; noNowarnTest; onOffTest] |> List.iter parseAndCheckFileInProject

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let CheckFileInProjectTest langVersion =
    let projectOptions, checker = mkProjectOptionsAndChecker langVersion
    let sourceName = projectOptions.SourceFiles[0]
    let parsingOptions = {FSharpParsingOptions.Default with SourceFiles = [|sourceName|]; LangVersionText = langVersion}
    let checkFileInProject testDef =
        let source = SourceText.ofString testDef.source
        let parseResults = checker.ParseFile(sourceName, source, parsingOptions) |> Async.RunImmediate
        let checkAnswer = checker.CheckFileInProject(parseResults, sourceName, 0, source, projectOptions) |> Async.RunImmediate
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Expected error, got Aborted")
        | FSharpCheckFileAnswer.Succeeded checkResults ->
            checkDiagnostics testDef.errors[langVersion] (Array.toList checkResults.Diagnostics)
    [justNowarnTest; noNowarnTest; onOffTest] |> List.iter checkFileInProject

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let GetBackgroundCheckResultsForFileInProjectTest langVersion =
    let options, checker = mkProjectOptionsAndChecker langVersion
    let sourceName = options.SourceFiles[0]
    let _wholeProjectResults = checker.ParseAndCheckProject(options) |> Async.RunImmediate
    let _, checkResults = checker.GetBackgroundCheckResultsForFileInProject(sourceName, options) |> Async.RunImmediate
    checkDiagnostics onOffTest.errors[langVersion] (Array.toList checkResults.Diagnostics)

let private warnEdits = [
    "module X\n#nowarn 20\n0\n#warnon 20\n0", [Warn(20, 5)];
    "module X\n#nowarn 20\n0\n#warnon 21\n0", [];
    "module X\n#nowarn 20\n0\n#warnon 20\n0", [Warn(20, 5)];
]

let private createProjectOptions() =
    let args = mkProjectCommandLineArgs ("warnEdits.dll", [])
    checker.GetProjectOptionsFromCommandLineArgs ("warnEdits.fsproj", args)

#nowarn 57
[<Fact>]
let EditUndoCheckTest () =
    let sourceName, projName, outputName = "warnEdits.fs", "warnEdits.fsproj", "warnEdits.dll"
    let checker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    let emptyDocSource = DocumentSource.Custom(fun s -> async {return Some (SourceText.ofString "")})
    let args = mkProjectCommandLineArgs(outputName, [])
    let options = {checker.GetProjectOptionsFromCommandLineArgs(projName, args) with SourceFiles = [| sourceName |]}
    let snapshot = FSharpProjectSnapshot.FromOptions(options, emptyDocSource) |> Async.RunImmediate
    let parseAndCheckFileInProject i (sourceText, errors) =
        let getSource() = System.Threading.Tasks.Task.FromResult(SourceTextNew.ofString sourceText)
        let fileSnapshot = ProjectSnapshot.FSharpFileSnapshot(sourceName, string i, getSource)
        let snapshot = FSharpProjectSnapshot.Create(
            snapshot.ProjectFileName,
            snapshot.OutputFileName,
            snapshot.ProjectId,
            [fileSnapshot],
            snapshot.ReferencesOnDisk,
            snapshot.OtherOptions,
            snapshot.ReferencedProjects,
            snapshot.IsIncompleteTypeCheckEnvironment,
            snapshot.UseScriptResolutionRules,
            snapshot.LoadTime,
            snapshot.UnresolvedReferences,
            snapshot.OriginalLoadReferences,
            None
            )
        let _, checkAnswer = checker.ParseAndCheckFileInProject(sourceName, snapshot) |> Async.RunImmediate
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Expected error, got Aborted")
        | FSharpCheckFileAnswer.Succeeded checkResults ->
            checkDiagnostics errors (Array.toList checkResults.Diagnostics)
    warnEdits |> List.iteri parseAndCheckFileInProject
