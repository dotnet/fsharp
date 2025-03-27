module FSharp.Compiler.Service.Tests.WarnScopeTests

open Xunit
open FSharp.Test
open FSharp.Test.Assert
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text

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
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    options, exprChecker

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
let ``ParseAndCheckProjectTest`` langVersion =
    let options, exprChecker = mkProjectOptionsAndChecker langVersion
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate
    checkDiagnostics onOffTest.errors[langVersion] (Array.toList wholeProjectResults.Diagnostics)
    
[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ``ParseAndCheckFileInProjectTest`` langVersion =
    let options, exprChecker = mkProjectOptionsAndChecker langVersion
    let sourceName = options.SourceFiles[0]
    let parseAndCheckFileInProject testDef =
        let source = SourceText.ofString testDef.source
        let _, checkAnswer = exprChecker.ParseAndCheckFileInProject(sourceName, 0,  source, options) |> Async.RunImmediate
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Expected error, got Aborted")
        | FSharpCheckFileAnswer.Succeeded checkResults ->
            checkDiagnostics testDef.errors[langVersion] (Array.toList checkResults.Diagnostics)
    [justNowarnTest; noNowarnTest; onOffTest] |> List.iter parseAndCheckFileInProject

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ``CheckFileInProjectTest`` langVersion =
    let projectOptions, exprChecker = mkProjectOptionsAndChecker langVersion
    let sourceName = projectOptions.SourceFiles[0]
    let parsingOptions = {FSharpParsingOptions.Default with SourceFiles = [|sourceName|]; LangVersionText = langVersion}
    let checkFileInProject testDef =
        let source = SourceText.ofString testDef.source
        let parseResults = exprChecker.ParseFile(sourceName, source, parsingOptions) |> Async.RunImmediate
        let checkAnswer = exprChecker.CheckFileInProject(parseResults, sourceName, 0, source, projectOptions) |> Async.RunImmediate
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Expected error, got Aborted")
        | FSharpCheckFileAnswer.Succeeded checkResults ->
            checkDiagnostics testDef.errors[langVersion] (Array.toList checkResults.Diagnostics)
    [justNowarnTest; noNowarnTest; onOffTest] |> List.iter checkFileInProject

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ``GetBackgroundCheckResultsForFileInProjectTest`` langVersion =
    let options, exprChecker = mkProjectOptionsAndChecker langVersion
    let sourceName = options.SourceFiles[0]
    let _wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate
    let _, checkResults = exprChecker.GetBackgroundCheckResultsForFileInProject(sourceName, options) |> Async.RunImmediate
    checkDiagnostics onOffTest.errors[langVersion] (Array.toList checkResults.Diagnostics)
