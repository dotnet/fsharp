module FSharp.Compiler.Service.Tests.CompletionTests

open FSharp.Compiler.EditorServices
open NUnit.Framework

let getCompletionInfo lineText (line, column) source =
    let parseResults, checkResults = getParseAndCheckResults source
    let plid = QuickParse.GetPartialLongNameEx(lineText, column)
    checkResults.GetDeclarationListInfo(Some parseResults, line, lineText, plid)

let getCompletionItemNames (completionInfo: DeclarationListInfo) =
    completionInfo.Items |> Array.map (fun item -> item.Name)

let assertHasItemWithNames names (completionInfo: DeclarationListInfo) =
    let itemNames = getCompletionItemNames completionInfo |> set

    for name in names do
        Assert.That(Set.contains name itemNames, name)


[<Test>]
let ``Expr - record - field 01 - anon module`` () =
    let info = getCompletionInfo "{ Fi }" (4, 3)  """
type Record = { Field: int }

{ Fi }
"""
    assertHasItemWithNames ["Field"] info

[<Test>]
let ``Expr - record - field 02 - anon module`` () =
    let info = getCompletionInfo "{ Fi }" (6, 3)  """
type Record = { Field: int }

let record = { Field = 1 }

{ Fi }
"""
    assertHasItemWithNames ["Field"] info

[<Test>]
let ``Expr - record - empty 01`` () =
    let info = getCompletionInfo "{  }" (4, 2) """
type Record = { Field: int }

{  }
"""
    assertHasItemWithNames ["Field"] info

[<Test>]
let ``Expr - record - empty 02`` () =
    let info = getCompletionInfo "{  }" (6, 2) """
type Record = { Field: int }

let record = { Field = 1 }

{  }
"""
    assertHasItemWithNames ["Field"; "record"] info
