// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.ExpressionNewMembersTests

open System
open System.Linq.Expressions
open Xunit
open Microsoft.FSharp.Linq.RuntimeHelpers

type PrivateRepresentation = private { Hidden: int }

type OnlyId(id: int) =
    member _.Id = id

let private construction (lambda: Expression<'T>) =
    match lambda.Body with
    | :? NewExpression as construction -> construction
    | body -> failwith $"Expected a New, got {body}"

let private memberNames (construction: NewExpression) =
    [ for m in construction.Members -> m.Name ]

[<Fact>]
let ``Tuple construction names the property each argument becomes`` () =
    let tuple = construction (LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int * string>(fun x -> (x, "a")) @>)
    Assert.Equal<string list>([ "Item1"; "Item2" ], memberNames tuple)
    Assert.All(tuple.Members, fun m -> Assert.Equal(Reflection.MemberTypes.Property, m.MemberType))

[<Fact>]
let ``Struct tuple construction names the field each argument becomes`` () =
    let tuple =
        construction (LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, struct (int * string)>(fun x -> struct (x, "a")) @>)

    Assert.Equal<string list>([ "Item1"; "Item2" ], memberNames tuple)
    Assert.All(tuple.Members, fun m -> Assert.Equal(Reflection.MemberTypes.Field, m.MemberType))

[<Fact>]
let ``Tuple of nine names Rest and the members of the nested tuple`` () =
    let tuple =
        construction (
            LeafExpressionConverter.QuotationToLambdaExpression
                <@ Func<int, _>(fun x -> (x, x, x, x, x, x, x, x, x)) @>
        )

    Assert.Equal<string list>([ "Item1"; "Item2"; "Item3"; "Item4"; "Item5"; "Item6"; "Item7"; "Rest" ], memberNames tuple)

    match tuple.Arguments.[7] with
    | :? NewExpression as rest -> Assert.Equal<string list>([ "Item1"; "Item2" ], memberNames rest)
    | rest -> Assert.Fail $"Expected Rest to be a New, got {rest}"

[<Fact>]
let ``Record construction names the field each argument becomes`` () =
    let record =
        construction (LeafExpressionConverter.QuotationToLambdaExpression <@ Func<Customer, _>(fun c -> { PersonName = c.Name; PersonAge = c.Age }) @>)

    Assert.Equal<string list>([ "PersonName"; "PersonAge" ], memberNames record)

[<Fact>]
let ``Anonymous record construction names the field each argument becomes`` () =
    let record =
        construction (LeafExpressionConverter.QuotationToLambdaExpression <@ Func<Customer, _>(fun c -> {| Age = c.Age; Name = c.Name |}) @>)

    Assert.Equal<string list>([ "Age"; "Name" ], memberNames record)

[<Fact>]
let ``Record whose fields cannot be read publicly is built without members`` () =
    let record = construction (LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, _>(fun x -> { Hidden = x }) @>)
    Assert.Null record.Members

[<Fact>]
let ``Constructor marked as an anonymous object keeps the properties of its type`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, OnlyId>(fun x -> LeafExpressionConverter.NewAnonymousObjectHelper(OnlyId x)) @>

    Assert.Equal<string list>([ "Id" ], memberNames (construction lambda))
    Assert.Equal(3, lambda.Compile().Invoke(3).Id)

[<Fact>]
let ``Anonymous object of the query translation names its items`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, _>(fun x -> LeafExpressionConverter.NewAnonymousObjectHelper(AnonymousObject<int, string>(x, "a"))) @>

    Assert.Equal<string list>([ "Item1"; "Item2" ], memberNames (construction lambda))
