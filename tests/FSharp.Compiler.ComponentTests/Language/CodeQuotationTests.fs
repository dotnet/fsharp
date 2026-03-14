// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler
open FSharp.Quotations.Patterns

module CodeQuotationsTests =

    [<Fact>]
    let ``Quotation on op_UnaryPlus(~+) compiles and runs`` () =
        Fsx """
open FSharp.Linq.RuntimeHelpers
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns

let eval q = LeafExpressionConverter.EvaluateQuotation q

let inline f x = <@ (~+) x @>
let x = <@ f 1 @>
let y : unit =
    match f 1 with
    | Call(_, methInfo, _) when methInfo.Name = "op_UnaryPlus" ->
        ()
    | e ->
        failwithf "did not expect expression for 'y': %A" e
let z : unit =
    match f 5 with
    | (CallWithWitnesses(_, methInfo, methInfoW, _, _) as e) when methInfo.Name = "op_UnaryPlus" && methInfoW.Name = "op_UnaryPlus$W" ->
        if ((eval e) :?> int) = 5 then
            ()
        else
            failwith "did not expect evaluation false"
    | e ->
        failwithf "did not expect expression for 'z': %A" e
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Quotation on decimal literal compiles and runs`` () =
        FSharp """
open Microsoft.FSharp.Quotations.DerivedPatterns

[<Literal>]
let x = 7m

let expr = <@ x @>

match expr with
| Decimal n -> printfn "%M" n
| _ -> failwith (string expr)
        """
        |> asExe
        |> withLangVersion80
        |> compileAndRun
        |> shouldSucceed

    // Tests for issues #11131 and #15648 - anonymous record field ordering
    // Note: The fix is in FSharp.Core/Linq.fs - these tests verify that queries 
    // with anonymous records work correctly regardless of field order.
    // The expression tree structure tests are in FSharp.Core.UnitTests which directly
    // references the modified FSharp.Core.
    
    [<Fact>]
    let ``Anonymous records with both field orders produce equivalent results - issue 11131 and 15648`` () =
        Fsx """
open System.Linq

type Person = { Name: string; Id : int }
type Wrapper = { Person: Person }

let data = [
    { Person = { Name = "One"; Id = 1 } }
    { Person = { Name = "Two"; Id = 2 } }
  ]

// Both orders should produce same results when executed
let resultsAlpha = 
    data.AsQueryable().Select(fun x -> {| A = x.Person.Name; B = x.Person.Id |}).ToList()
      
let resultsNonAlpha = 
    data.AsQueryable().Select(fun x -> {| B = x.Person.Id; A = x.Person.Name |}).ToList()

// Verify results are equivalent
if resultsAlpha.Count <> resultsNonAlpha.Count then
    failwith "Result counts don't match"

for i in 0 .. resultsAlpha.Count - 1 do
    if resultsAlpha.[i].A <> resultsNonAlpha.[i].A then
        failwithf "A values don't match at index %d" i
    if resultsAlpha.[i].B <> resultsNonAlpha.[i].B then
        failwithf "B values don't match at index %d" i
    
printfn "Both field orders produce equivalent results: %d items" resultsAlpha.Count
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Nested anonymous records work correctly`` () =
        Fsx """
open System.Linq

type Person = { Name: string; Id : int }
type Wrapper = { Person: Person }

let data = [
    { Person = { Name = "One"; Id = 1 } }
  ]

// Nested anonymous records should work
let queryNested = 
    data.AsQueryable().Select(fun x -> {| Other = {| Name = x.Person.Name; Id = x.Person.Id |} |}).ToList()

if queryNested.Count <> 1 then
    failwith "Expected 1 result"
    
if queryNested.[0].Other.Name <> "One" then
    failwithf "Expected Name='One', got '%s'" queryNested.[0].Other.Name
    
if queryNested.[0].Other.Id <> 1 then
    failwithf "Expected Id=1, got %d" queryNested.[0].Other.Id
    
printfn "Nested anonymous record works correctly"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``F# record with non-declaration field order works correctly`` () =
        Fsx """
open System.Linq

type Person = { Name: string; Id : int }
type PartialPerson = { LastName: string; ID : int }

let data = [ { Name = "One"; Id = 1 }; { Name = "Two"; Id = 2 } ]

// Declaration order
let query1 = data.AsQueryable().Select(fun p -> { LastName = p.Name; ID = p.Id }).ToList()
      
// Non-declaration order (swapped)
let query2 = data.AsQueryable().Select(fun p -> { ID = p.Id; LastName = p.Name }).ToList()

if query1.Count <> query2.Count then
    failwith "Result counts don't match"

for i in 0 .. query1.Count - 1 do
    if query1.[i].LastName <> query2.[i].LastName then
        failwithf "LastName values don't match at index %d" i
    if query1.[i].ID <> query2.[i].ID then
        failwithf "ID values don't match at index %d" i
    
printfn "Both F# record field orderings produce equivalent results"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed