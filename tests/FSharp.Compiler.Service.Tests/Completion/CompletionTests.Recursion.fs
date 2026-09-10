module FSharp.Compiler.Service.Tests.CompletionRecursionTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``CompletionInDifferentEnvs1`` () =
    let info =
        Checker.getCompletionInfo
            """let f1 num =
    let rec completeword d =
        d + d
(**)comple{caret}"""

    assertHasItemWithNames [ "completeword" ] info
