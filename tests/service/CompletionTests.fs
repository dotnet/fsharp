module FSharp.Compiler.Service.Tests.CompletionTests

open FSharp.Compiler.EditorServices
open FsUnit
open NUnit.Framework

[<Test>]
let ``Expr - record - field 01 - anon module`` () =
    let parseResults, checkResults = getParseAndCheckResults """
type Record = { Field: int}

{ Fi }
"""
    let lineText = "{ Fi }"
    let plid = QuickParse.GetPartialLongNameEx(lineText, 3)
    let info = checkResults.GetDeclarationListInfo(Some parseResults, 4, lineText, plid)

    info.Items |> Array.exists (fun item -> item.Name = "Field") |> shouldEqual true
