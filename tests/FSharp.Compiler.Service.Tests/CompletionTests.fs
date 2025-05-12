﻿module FSharp.Compiler.Service.Tests.CompletionTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.EditorServices
open Xunit

let getCompletionInfo source =
    let source, lineText, pos = getCursorPosAndPrepareSource source
    let parseResults, checkResults = getParseAndCheckResultsPreview source
    let plid = QuickParse.GetPartialLongNameEx(lineText, pos.Column)
    checkResults.GetDeclarationListInfo(Some parseResults, pos.Line, lineText, plid)

let getCompletionItemNames (completionInfo: DeclarationListInfo) =
    completionInfo.Items |> Array.map (fun item -> item.NameInCode)

let assertHasItemWithNames names (completionInfo: DeclarationListInfo) =
    let itemNames = getCompletionItemNames completionInfo |> set

    for name in names do
        Assert.True(Set.contains name itemNames, $"{name} not found in {itemNames}")

[<Fact>]
let ``Expr - After record decl 01`` () =
    let info = getCompletionInfo """
type Record = { Field: int }

{ Fi{caret} }
"""
    assertHasItemWithNames ["ignore"] info

[<Fact>]
let ``Expr - After record decl 02`` () =
    let info = getCompletionInfo """
type Record = { Field: int }

{caret}
"""
    assertHasItemWithNames ["ignore"] info

[<Fact>]
let ``Expr - record - field 01 - anon module`` () =
    let info = getCompletionInfo """
type Record = { Field: int }

{ Fi{caret} }
"""
    assertHasItemWithNames ["Field"] info

[<Fact>]
let ``Expr - record - field 02 - anon module`` () =
    let info = getCompletionInfo """
type Record = { Field: int }

let record = { Field = 1 }

{ Fi{caret} }
"""
    assertHasItemWithNames ["Field"] info

[<Fact>]
let ``Expr - record - empty 01`` () =
    let info = getCompletionInfo """
type Record = { Field: int }

{ {caret} }
"""
    assertHasItemWithNames ["Field"] info

[<Fact>]
let ``Expr - record - empty 02`` () =
    let info = getCompletionInfo """
type Record = { Field: int }

let record = { Field = 1 }

{ {caret} }
"""
    assertHasItemWithNames ["Field"; "record"] info

[<Fact>]
let ``Underscore dot lambda - completion`` () =
    let info = getCompletionInfo """
let myFancyFunc (x:string) = 
    x 
    |> _.Len{caret}"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Underscore dot lambda - method completion`` () =
    let info = getCompletionInfo """
let myFancyFunc (x:string) = 
    x 
    |> _.ToL{caret}"""
    assertHasItemWithNames ["ToLower"] info

[<Fact>]
let ``Underscore dot lambda - No prefix`` () =
    let info = getCompletionInfo """
let s = ""
[s] |> List.map _.{caret} 
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Type decl - Record - Field type 01`` () =
    let info = getCompletionInfo """
type Record = { Field: {caret} }
"""
    assertHasItemWithNames ["string"] info


[<Fact>]
let ``Expr - Qualifier 01`` () =
    let info = getCompletionInfo """
let f (s: string) =
    s.Trim().{caret}
    s.Trim()
    s.Trim()
    ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 02`` () =
    let info =
        getCompletionInfo """
let f (s: string) =
    s.Trim()
    s.Trim().{caret}
    s.Trim()
    ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 03`` () =
    let info =
        getCompletionInfo """
let f (s: string) =
    s.Trim()
    s.Trim()
    s.Trim().{caret}
    ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 04`` () =
    let info =
        getCompletionInfo """
type T() =
    do
        System.String.Empty.ToString().L{caret}
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 05`` () =
    let info =
        getCompletionInfo """
System.String.Empty.ToString().{caret}
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 06`` () =
    let info =
        getCompletionInfo """
System.String.Empty.ToString().L{caret}
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 07`` () =
    let info =
        getCompletionInfo """
type T() =
    do
        System.String.Empty.ToString().L{caret}
        ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Import - Ns 01`` () =
    let info =
        getCompletionInfo """
namespace Ns

type Rec1 = { F: int }


namespace Ns

type Rec2 = { F: int }

module M =

    type Rec3 = { F: int }

    let _: R{caret} = ()
"""
    assertHasItemWithNames ["Rec1"; "Rec2"; "Rec3"] info

[<Fact>]
let ``Import - Ns 02 - Rec`` () =
    let info =
        getCompletionInfo """
namespace Ns

type Rec1 = { F: int }


namespace rec Ns

type Rec2 = { F: int }

module M =

    type Rec3 = { F: int }

    let _: R{caret} = ()
"""
    assertHasItemWithNames ["Rec1"; "Rec2"; "Rec3"] info

[<Fact>]
let ``Import - Ns 03 - Rec`` () =
    let info =
        getCompletionInfo """
namespace Ns

type Rec1 = { F: int }


namespace rec Ns

type Rec2 = { F: int }

module rec M =

    type Rec3 = { F: int }

    let _: R{caret} = ()
"""
    assertHasItemWithNames ["Rec1"; "Rec2"; "Rec3"] info