// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.ExpressionFSharpOperatorTests

open System
open System.Linq.Expressions
open Xunit
open Microsoft.FSharp.Linq.RuntimeHelpers

type Distance =
    { Metres: float }

    static member Abs(d: Distance) = { Metres = abs d.Metres }

let private bodyOf (quotation: Quotations.Expr<Func<'T, 'U>>) =
    (LeafExpressionConverter.QuotationToLambdaExpression quotation).Body

let private calledMethod (body: Expression) =
    match body with
    | :? MethodCallExpression as call -> call.Method
    | body -> failwith $"Expected a call, got {body}"

[<Fact>]
let ``fst and snd read the tuple's items`` () =
    for body, item in [ bodyOf <@ Func<int * string, int>(fun t -> fst t) @>, "Item1"; bodyOf <@ Func<int * string, string>(fun t -> snd t) @>, "Item2" ] do
        match body with
        | :? MemberExpression as read -> Assert.Equal(item, read.Member.Name)
        | body -> Assert.Fail $"Expected a read of {item}, got {body}"

[<Fact>]
let ``isNull compares with a null constant`` () =
    match bodyOf <@ Func<string, bool>(fun s -> isNull s) @> with
    | :? BinaryExpression as comparison ->
        Assert.Equal(ExpressionType.Equal, comparison.NodeType)
        Assert.Null comparison.Method
        Assert.Equal(ExpressionType.Constant, comparison.Right.NodeType)
    | body -> Assert.Fail $"Expected a comparison, got {body}"

[<Fact>]
let ``box converts to obj`` () =
    let body = bodyOf <@ Func<int, obj>(fun x -> box x) @>
    Assert.Equal(ExpressionType.Convert, body.NodeType)
    Assert.Equal(typeof<obj>, body.Type)

[<Fact>]
let ``Math functions call System.Math`` () =
    Assert.Equal(typeof<Math>.GetMethod("Sqrt", [| typeof<float> |]), calledMethod (bodyOf <@ Func<float, float>(fun x -> sqrt x) @>))
    Assert.Equal(typeof<Math>.GetMethod("Abs", [| typeof<int> |]), calledMethod (bodyOf <@ Func<int, int>(fun x -> abs x) @>))
    Assert.Equal(typeof<Math>.GetMethod("Max", [| typeof<int>; typeof<int> |]), calledMethod (bodyOf <@ Func<int, int>(fun x -> max x 3) @>))
    Assert.Equal(typeof<Math>.GetMethod("Pow", [| typeof<float>; typeof<float> |]), calledMethod (bodyOf <@ Func<float, float>(fun x -> x ** 2.0) @>))

[<Fact>]
let ``Math functions on float32 go through float as the F# functions do`` () =
    let lambda = LeafExpressionConverter.QuotationToLambdaExpression <@ Func<float32, float32>(fun x -> sqrt x) @>

    match lambda.Body with
    | :? UnaryExpression as conversion ->
        Assert.Equal(typeof<float32>, conversion.Type)
        Assert.Equal(typeof<Math>.GetMethod("Sqrt", [| typeof<float> |]), calledMethod conversion.Operand)
    | body -> Assert.Fail $"Expected a conversion to float32, got {body}"

    Assert.Equal(sqrt 2.0f, lambda.Compile().Invoke 2.0f)

[<Fact>]
let ``Math functions on other types keep their F# implementation`` () =
    let absolute = LeafExpressionConverter.QuotationToLambdaExpression <@ Func<Distance, Distance>(fun d -> abs d) @>
    Assert.Equal({ Metres = 2.0 }, absolute.Compile().Invoke { Metres = -2.0 })

    let smaller = LeafExpressionConverter.QuotationToLambdaExpression <@ Func<string, string>(fun s -> min s "b") @>
    Assert.Equal("a", smaller.Compile().Invoke "a")

let private compiled (quotation: Quotations.Expr<Func<'T, 'U>>) =
    (LeafExpressionConverter.QuotationToLambdaExpression quotation).Compile()

[<Fact>]
let ``Compiled math functions give what the F# functions give`` () =
    let inputs = [ -7.5; -2.5; -0.5; 0.0; 0.5; 1.5; 2.5; 3.5; 9.25 ]

    let unary: (string * (float -> float) * Func<float, float>) list =
        [ "abs", abs, compiled <@ Func<float, float>(fun x -> abs x) @>
          "floor", floor, compiled <@ Func<float, float>(fun x -> floor x) @>
          "ceil", ceil, compiled <@ Func<float, float>(fun x -> ceil x) @>
          "round", round, compiled <@ Func<float, float>(fun x -> round x) @>
          "truncate", truncate, compiled <@ Func<float, float>(fun x -> truncate x) @>
          "exp", exp, compiled <@ Func<float, float>(fun x -> exp x) @>
          "sin", sin, compiled <@ Func<float, float>(fun x -> sin x) @>
          "cos", cos, compiled <@ Func<float, float>(fun x -> cos x) @>
          "tan", tan, compiled <@ Func<float, float>(fun x -> tan x) @>
          "atan", atan, compiled <@ Func<float, float>(fun x -> atan x) @>
          "sinh", sinh, compiled <@ Func<float, float>(fun x -> sinh x) @>
          "cosh", cosh, compiled <@ Func<float, float>(fun x -> cosh x) @>
          "tanh", tanh, compiled <@ Func<float, float>(fun x -> tanh x) @> ]

    for name, expected, actual in unary do
        for x in inputs do
            Assert.True(expected x = actual.Invoke x, $"{name} {x}: expected {expected x}, got {actual.Invoke x}")

    Assert.Equal(sign -7.5, (compiled <@ Func<float, int>(fun x -> sign x) @>).Invoke -7.5)

[<Fact>]
let ``Compiled functions of two arguments keep the order of the arguments`` () =
    Assert.Equal(2.0 ** 3.0, (compiled <@ Func<float, float>(fun x -> x ** 3.0) @>).Invoke 2.0)
    Assert.Equal(3.0 ** 2.0, (compiled <@ Func<float, float>(fun x -> 3.0 ** x) @>).Invoke 2.0)
    Assert.Equal(atan2 1.0 2.0, (compiled <@ Func<float, float>(fun x -> atan2 x 2.0) @>).Invoke 1.0)
    Assert.Equal(atan2 2.0 1.0, (compiled <@ Func<float, float>(fun x -> atan2 2.0 x) @>).Invoke 1.0)
    Assert.Equal(Operators.log 8.0, (compiled <@ Func<float, float>(fun x -> Operators.log x) @>).Invoke 8.0)
    Assert.Equal(log10 8.0, (compiled <@ Func<float, float>(fun x -> log10 x) @>).Invoke 8.0)

[<Fact>]
let ``Compiled min and max treat NaN and signed zero as the F# functions do`` () =
    let minWith = compiled <@ Func<float, float>(fun x -> min x 0.0) @>
    let maxWith = compiled <@ Func<float, float>(fun x -> max x 0.0) @>
    Assert.True(Double.IsNaN(min nan 0.0) = Double.IsNaN(minWith.Invoke nan))
    Assert.True(Double.IsNaN(max nan 0.0) = Double.IsNaN(maxWith.Invoke nan))
    Assert.Equal(BitConverter.DoubleToInt64Bits(min -0.0 0.0), BitConverter.DoubleToInt64Bits(minWith.Invoke -0.0))
    Assert.Equal(BitConverter.DoubleToInt64Bits(max -0.0 0.0), BitConverter.DoubleToInt64Bits(maxWith.Invoke -0.0))

[<Fact>]
let ``fst of a tuple written in a query evaluates the other item`` () =
    let source = System.Linq.Queryable.AsQueryable [ 1; 2 ]

    Assert.Throws<DivideByZeroException>(fun () ->
        query {
            for x in source do
                select (x, "a") into t
                select (fst (fst t, 1 / (fst t - fst t)))
        }
        |> Seq.toList
        |> ignore)
    |> ignore

[<Fact>]
let ``snd of a tuple written in a query evaluates the other item`` () =
    let source = System.Linq.Queryable.AsQueryable [ 1; 2 ]

    Assert.Throws<DivideByZeroException>(fun () ->
        query {
            for x in source do
                select (x, "a") into t
                select (snd (1 / (fst t - fst t), snd t))
        }
        |> Seq.toList
        |> ignore)
    |> ignore
