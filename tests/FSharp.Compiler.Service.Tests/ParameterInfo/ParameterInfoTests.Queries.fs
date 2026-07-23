module FSharp.Compiler.Service.Tests.ParameterInfoQueriesTests

open Xunit

[<Fact>]
let ``LocationOfParams.UnmatchedParensBeforeModuleKeyword.Bug245850.Case2a`` () =
    assertHasParameterInfo """
module Repro =
    query { for a in System.Int16.TryParse({caret}
module AA =
    let x = 10"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.InNestedQuery`` () =
    assertParameterInfoContains ["obj"] """
let tuples = [ (1, 8, 9); (56, 45, 3)]
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
let tp = (2,3,6)
let foo =
    query {
        for n in numbers do
        yield (n, query {for x in tuples do
                         let r = x.Equals({caret}tp)
                         select r })
        }"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.WithErrors`` () =
    assertParameterInfoContains ["obj"] """
let tuples = [ (1, 8, 9); (56, 45, 3)]
let tp = (2,3,6)
let foo =
    query {
        for t in tuples do
        orderBy (t.Equals({caret}tp))
        }"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.OperatorWithParentheses`` () =
    assertParameterInfoContains [] """
let categories = ["Beverages"; "Condiments"; "Vegetables";]
let products = [1;2;3]
let q2 =
    query {
        for c in categories do
        groupJoin({caret}for p in products -> c = p) into ps
        select (c, ps)
    } |> Seq.toArray"""

[<Fact>]
let ``Query.OptionalArgumentsInQuery`` () =
    assertParameterInfoContains ["x: int"; "?y int"] """
type TT(x : int, ?y : int) =
    let z = y
    do printfn "%A" z
    member this.Foo(?z : int) = z

type TT2(x : int, y : int option) =
    let z  = y
    do printfn "%A" z
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]

let test3 =
    query {
        for n in numbers do
        let tt = TT({caret}
        minBy n
    }"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.OverloadMethod.InQuery`` () =
    assertParameterInfoContains ["int"; "int"; "string"; "bool"] """
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]

type Foo() =
    member this.A1(x1 : int, x2 : int, ?y : string, ?Z: bool) = ()
    member this.A1(x1 : int, X2 : string, ?y : int, ?Z: bool) = ()

let test3 =
    query {
        for n in numbers do
        let foo = new Foo()
        foo.A1(1,1,{caret}
        minBy n
    }"""
