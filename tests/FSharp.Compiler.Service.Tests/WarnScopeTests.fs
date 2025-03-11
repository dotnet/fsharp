module FSharp.Compiler.Service.Tests.WarnScopeTests

open Xunit
open FSharp.Test
open FSharp.Test.Assert
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Service.Tests.Common

module internal ProjectForPCP =

    let fileSource1 = """
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
    
[<InlineData("9.0")>]
// [<InlineData("preview")>]
[<Theory>]
let ``Test WarnScopes`` langVersion =
    let options = createProjectOptions [ProjectForPCP.fileSource1] [$"--langversion:{langVersion}"]
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=CompilerAssertHelpers.UseTransparentCompiler)
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate
    let shouldBeErr n line (diagnostic: FSharpDiagnostic) =
        diagnostic.ErrorNumber |> shouldEqual n
        diagnostic.Range.StartLine |> shouldEqual line
    wholeProjectResults.Diagnostics.Length |> shouldEqual 2
    if langVersion = "9.0" then
        wholeProjectResults.Diagnostics.[0] |> shouldBeErr 3350 6
        wholeProjectResults.Diagnostics.[1] |> shouldBeErr 20 3
    else
        wholeProjectResults.Diagnostics.Length |> shouldEqual 2
        wholeProjectResults.Diagnostics.[0] |> shouldBeErr 20 3
        wholeProjectResults.Diagnostics.[1] |> shouldBeErr 20 7
    