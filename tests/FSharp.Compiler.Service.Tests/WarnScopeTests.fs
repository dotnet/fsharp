module FSharp.Compiler.Service.Tests.WarnScopeTests

open Xunit
open FSharp.Compiler.Service.Tests.Common

let sourceForWarnScopes =
    """
module A
match None with None -> ()
#nowarn "25"
match None with None -> ()
#warnon "25"
match None with None -> ()
#nowarn "25"
match None with None -> ()
    """

[<Fact>]
let WarnScopesWorkAsExpected () =
    let file = "WarnScopesInScript.fs"
    let parseResult, typeCheckResults = parseAndCheckScript(file, sourceForWarnScopes)
    Assert.Equal(parseResult.Diagnostics.Length, 0)
    Assert.Equal(typeCheckResults.Diagnostics.Length, 2)
    Assert.Equal(typeCheckResults.Diagnostics[0].ErrorNumber, 25)
    Assert.Equal(typeCheckResults.Diagnostics[1].ErrorNumber, 25)
    Assert.Equal(typeCheckResults.Diagnostics[0].Range.StartLine, 3)
    Assert.Equal(typeCheckResults.Diagnostics[1].Range.StartLine, 7)
    


