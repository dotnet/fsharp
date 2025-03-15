module FSharp.Compiler.Service.Tests.WarnScopeTests

open Xunit
open FSharp.Test
open FSharp.Test.Assert
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text

let private warnScopeOnOffSource = """
module N.M
""
#nowarn "20"
""
#warnon "20"
""
#nowarn "20"
""
()
"""

let mkProjectOptionsAndChecker langVersion =
    let options = createProjectOptions [warnScopeOnOffSource] [$"--langversion:{langVersion}"]
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    options, exprChecker

let checkDiagnostics langVersion (diagnostics: FSharpDiagnostic array) =
    let shouldBeErr n line (diagnostic: FSharpDiagnostic) =
        diagnostic.ErrorNumber |> shouldEqual n
        diagnostic.Range.StartLine |> shouldEqual line
    diagnostics.Length |> shouldEqual 2
    if langVersion = "9.0" then
        diagnostics.[0] |> shouldBeErr 3350 6
        diagnostics.[1] |> shouldBeErr 20 3
    else
        diagnostics.Length |> shouldEqual 2
        diagnostics.[0] |> shouldBeErr 20 3
        diagnostics.[1] |> shouldBeErr 20 7

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ``ParseAndCheckProjectTest`` langVersion =
    let options, exprChecker = mkProjectOptionsAndChecker langVersion
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate
    checkDiagnostics langVersion wholeProjectResults.Diagnostics
    
[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ``ParseAndCheckFileInProjectTest`` langVersion =
    let options, exprChecker = mkProjectOptionsAndChecker langVersion
    let source = SourceText.ofString warnScopeOnOffSource
    let sourceName = options.SourceFiles[0]
    let _, checkAnswer = exprChecker.ParseAndCheckFileInProject(sourceName, 0,  source, options) |> Async.RunImmediate
    match checkAnswer with
    | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Expected error, got Aborted")
    | FSharpCheckFileAnswer.Succeeded checkResults -> checkDiagnostics langVersion checkResults.Diagnostics

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ``CheckFileInProjectTest`` langVersion =
    let projectOptions, exprChecker = mkProjectOptionsAndChecker langVersion
    let source = SourceText.ofString warnScopeOnOffSource
    let sourceName = projectOptions.SourceFiles[0]
    let parsingOptions = {FSharpParsingOptions.Default with SourceFiles = [|sourceName|]; LangVersionText = langVersion}
    let parseResults = exprChecker.ParseFile(sourceName, source, parsingOptions) |> Async.RunImmediate
    let diagnosticsOptions = parsingOptions.DiagnosticOptions
    let checkAnswer = exprChecker.CheckFileInProject(parseResults, sourceName, 0, source, projectOptions) |> Async.RunImmediate
    match checkAnswer with
    | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Expected error, got Aborted")
    | FSharpCheckFileAnswer.Succeeded checkResults -> checkDiagnostics langVersion checkResults.Diagnostics

[<InlineData("9.0")>]
[<InlineData("preview")>]
[<Theory>]
let ``GetBackgroundCheckResultsForFileInProjectTest`` langVersion =
    let options, exprChecker = mkProjectOptionsAndChecker langVersion
    let sourceName = options.SourceFiles[0]
    let _wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate
    let _, checkResults = exprChecker.GetBackgroundCheckResultsForFileInProject(sourceName, options) |> Async.RunImmediate
    checkDiagnostics langVersion checkResults.Diagnostics
