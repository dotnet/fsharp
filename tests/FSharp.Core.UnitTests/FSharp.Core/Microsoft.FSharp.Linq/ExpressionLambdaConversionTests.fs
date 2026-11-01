// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.ExpressionLambdaConversionTests

open System
open System.Linq.Expressions
open Xunit
open Microsoft.FSharp.Linq.RuntimeHelpers

type private Counter() =
    let mutable calls = 0
    member _.Calls = calls

    member _.Next() =
        calls <- calls + 1
        calls

let private bodyOf (lambda: Expression<'T>) = lambda.Body

[<Fact>]
let ``Anonymous record with fields out of alphabetical order becomes a New in an expression lambda`` () =
    let lambda = LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, _>(fun x -> {| B = x; A = x + 1 |}) @>
    Assert.Equal(ExpressionType.New, (bodyOf lambda).NodeType)

[<Fact>]
let ``Record with fields out of declaration order becomes a New in an expression lambda`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<Customer, _>(fun c -> { PersonAge = c.Age; PersonName = c.Name }) @>

    Assert.Equal(ExpressionType.New, (bodyOf lambda).NodeType)

[<Fact>]
let ``Tupled lambda parameters read the tuple directly in an expression lambda`` () =
    let lambda = LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int * string, bool>(fun (id, _) -> id > 1) @>

    match bodyOf lambda with
    | :? BinaryExpression as comparison ->
        match comparison.Left with
        | :? MemberExpression as item -> Assert.Equal("Item1", item.Member.Name)
        | left -> Assert.Fail $"Expected a read of Item1, got {left}"
    | body -> Assert.Fail $"Expected a comparison, got {body}"

[<Fact>]
let ``Assigned mutable variable stays a block in an expression lambda`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun x ->
                    let mutable y = x
                    y <- y + 1
                    y)
            @>

    Assert.Equal(ExpressionType.Block, (bodyOf lambda).NodeType)
    Assert.Equal(6, lambda.Compile().Invoke 5)

[<Fact>]
let ``EvaluateQuotation still evaluates a let once`` () =
    let counter = Counter()
    let result = LeafExpressionConverter.EvaluateQuotation <@ let v = counter.Next() in v + v @>
    Assert.Equal(2, result :?> int)
    Assert.Equal(1, counter.Calls)

[<Struct>]
type Tally =
    val mutable Count: int

    member this.Add() =
        this.Count <- this.Count + 1

    member this.AddAndCount() =
        this.Count <- this.Count + 1
        this.Count

type TallyHolder() =
    [<DefaultValue>]
    val mutable Tally: Tally

type Reading = { First: int; Second: int }

let mutable private hashCalls = 0

/// A struct whose hash is the number of times a hash was asked for
[<Struct; CustomEquality; NoComparison>]
type CountedHash(id: int) =
    member _.Id = id

    override _.GetHashCode() =
        hashCalls <- hashCalls + 1
        hashCalls

    override _.Equals(_: obj) = false

/// A formatting provider that counts how often it is asked for a format
type CountingProvider() =
    let mutable calls = 0
    member _.Calls = calls

    interface IFormatProvider with
        member _.GetFormat(_: Type) =
            calls <- calls + 1
            null

let private run (lambda: Expression<Func<int, 'T>>) (argument: int) = lambda.Compile().Invoke argument

[<Fact>]
let ``Let read after a later assignment keeps the value it was bound to`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let mutable y = a
                    let x = y + 1
                    y <- 100
                    x + y)
            @>

    Assert.Equal(131, run lambda 30)

[<Fact>]
let ``Let read after a property is set keeps the value it was bound to`` () =
    let view = CustomerView(ViewAge = 1)

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let before = view.ViewAge
                    view.ViewAge <- before + a
                    before)
            @>

    Assert.Equal(1, run lambda 5)
    Assert.Equal(6, view.ViewAge)

[<Fact>]
let ``Mutable struct changed in place is one value`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let mutable tally = Tally()
                    tally.Add()
                    tally.Add()
                    tally.Count + a)
            @>

    Assert.Equal(5, run lambda 3)

[<Fact>]
let ``Object bound by a let and changed is one object`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let items = Collections.Generic.List<int>()
                    items.Add a
                    items.Count)
            @>

    Assert.Equal(1, run lambda 7)

[<Fact>]
let ``Object bound by a let and read twice is one object`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, bool>(fun _ -> let o = obj () in obj.ReferenceEquals(o, o)) @>

    Assert.True(run lambda 0)

[<Fact>]
let ``Let that is never read is still evaluated`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int>(fun a -> let unused = counter.Next() in a) @>

    Assert.Equal(4, run lambda 4)
    Assert.Equal(1, counter.Calls)

[<Fact>]
let ``Record fields written out of order are evaluated as written in a query over a sequence`` () =
    let counter = Counter()

    let first =
        LeafExpressionConverter.EvaluateQuotation
            <@ query { for _ in [ 1; 2 ] do select { Second = counter.Next(); First = counter.Next() }; head } @>
        :?> Reading

    Assert.Equal({ First = 2; Second = 1 }, first)

type private BlockFinder() =
    inherit ExpressionVisitor()

    member val Found = false with get, set

    override this.VisitBlock node =
        this.Found <- true
        base.VisitBlock node

let private hasBlock (expression: Expression) =
    let finder = BlockFinder()
    finder.Visit expression |> ignore
    finder.Found

let addTo (items: Collections.Generic.List<int>) (item: int) =
    items.Add item
    1

[<Fact>]
let ``Value of a let that a lambda reads is computed once however often the lambda runs`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let seen = Collections.Generic.HashSet<int>()
                    Seq.length (Seq.filter (fun x -> seen.Add x) [| a; a; a + 1 |]))
            @>

    Assert.Equal(2, run lambda 5)

[<Fact>]
let ``Call bound by a let and read twice runs once`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int>(fun a -> let v = counter.Next() in v + v + a) @>

    Assert.Equal(2, run lambda 0)
    Assert.Equal(1, counter.Calls)

[<Fact>]
let ``Object behind a cast is one object when read twice`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, bool>(fun _ ->
                    let items = Collections.Generic.List<int>() :> Collections.Generic.IList<int>
                    obj.ReferenceEquals(items, items))
            @>

    Assert.True(run lambda 0)

[<Fact>]
let ``Object reached through a second let is one object`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let made = Collections.Generic.List<int>()
                    let items = made
                    addTo items a + items.Count)
            @>

    Assert.Equal(2, run lambda 7)

[<Fact>]
let ``Member of an object read before a call keeps the value it had`` () =
    let items = Collections.Generic.List<int>()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int>(fun a -> let before = items.Count in addTo items a + before) @>

    Assert.Equal(1, run lambda 7)

[<Fact>]
let ``Statement anywhere in the lambda keeps every let where it is written`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<CustomerView, int>(fun view ->
                    let before = view.ViewAge
                    view.ViewAge <- before + 10
                    before)
            @>

    let view = CustomerView(ViewAge = 1)
    Assert.Equal(1, lambda.Compile().Invoke view)
    Assert.Equal(11, view.ViewAge)

[<Fact>]
let ``Enumerator bound by a let is one enumerator`` () =
    let items = Collections.Generic.List<int>([ 10; 20 ])

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let position = (items :> seq<int>).GetEnumerator()
                    if position.MoveNext() then position.Current + a else -1)
            @>

    Assert.Equal(10, run lambda 0)

[<Fact>]
let ``Value built on a value that is read twice does not multiply`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, int>(fun a ->
                    let b = a + a
                    let c = b + b
                    let d = c + c
                    let e = d + d
                    let f = e + e
                    let g = f + f
                    let h = g + g
                    let i = h + h
                    i + i)
            @>

    Assert.Equal(512, run lambda 1)
    Assert.True((bodyOf lambda).ToString().Length < 400)

[<Fact>]
let ``Method of a string bound by a let is called at each read`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<string, string>(fun s -> let upper = s.ToUpper() in upper + upper) @>

    Assert.False(hasBlock lambda)
    Assert.Equal("ABAB", lambda.Compile().Invoke "ab")

[<Fact>]
let ``Call bound by a let and read once is made at that read`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<Customer, bool>(fun c -> let orders = Seq.length c.Orders in orders > c.Age + 1) @>

    Assert.False(hasBlock lambda)

[<Fact>]
let ``Record fields are not reordered around a statement in a query over a queryable`` () =
    let source = System.Linq.Queryable.AsQueryable [ 1; 2 ]

    let rows =
        query {
            for x in source do
                select (
                    let mutable y = x
                    let second = (y <- y * 10; y)
                    { First = y; Second = second }
                )
        }
        |> Seq.toList

    Assert.Equal<Reading list>([ { First = 10; Second = 10 }; { First = 20; Second = 20 } ], rows)
[<Fact>]
let ``Call bound by a let runs before a member that is read ahead of the variable`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<Collections.Generic.List<int>, int>(fun items -> let added = addTo items 5 in items.Count + added) @>

    Assert.Equal(2, lambda.Compile().Invoke(Collections.Generic.List<int>()))

[<Fact>]
let ``Struct bound by a let is a copy`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<TallyHolder, int>(fun holder -> let tally = holder.Tally in tally.AddAndCount()) @>

    let holder = TallyHolder()
    Assert.Equal(1, lambda.Compile().Invoke holder)
    Assert.Equal(0, holder.Tally.Count)

[<Fact>]
let ``Boxed value bound by a let is one object`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, bool>(fun a -> let boxed = (a :> obj) in obj.ReferenceEquals(boxed, boxed)) @>

    Assert.True(run lambda 7)

[<Fact>]
let ``Record fields are not reordered next to a statement in a query over a queryable`` () =
    let counter = Counter()
    let view = CustomerView(ViewAge = 1)
    let source = System.Linq.Queryable.AsQueryable [ 1 ]

    let rows =
        query {
            for x in source do
                select (
                    view.ViewAge <- x
                    { Second = counter.Next(); First = counter.Next() }
                )
        }
        |> Seq.toList

    Assert.Equal<Reading list>([ { First = 2; Second = 1 } ], rows)

[<Fact>]
let ``Let written by hand is evaluated before the fields that follow it`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, Reading>(fun _ -> let second = counter.Next() in { First = counter.Next(); Second = second }) @>

    Assert.Equal({ First = 2; Second = 1 }, run lambda 0)

[<Fact>]
let ``Let written by hand is evaluated before the fields that follow it in a query over a queryable`` () =
    let counter = Counter()
    let source = System.Linq.Queryable.AsQueryable [ 1 ]

    let rows =
        query {
            for _ in source do
                select (
                    let second = counter.Next()
                    { First = counter.Next(); Second = second }
                )
        }
        |> Seq.toList

    Assert.Equal<Reading list>([ { First = 2; Second = 1 } ], rows)

[<Fact>]
let ``Hash of a nullable struct of the user's is computed once however often it is read`` () =
    hashCalls <- 0

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<Nullable<CountedHash>, int>(fun n -> let hash = n.GetHashCode() in hash + hash) @>

    Assert.Equal(2, lambda.Compile().Invoke(Nullable(CountedHash 1)))
    Assert.Equal(1, hashCalls)

[<Fact>]
let ``Text made with a formatting provider is made once however often it is read`` () =
    let counting = CountingProvider()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<IFormatProvider, string>(fun provider -> let text = (-1).ToString provider in text + text) @>

    // A negative number asks the provider for its sign
    let once = (-1).ToString()
    Assert.Equal(once + once, lambda.Compile().Invoke counting)
    Assert.Equal(1, counting.Calls)

[<Fact>]
let ``Struct bound by a mutable let named like the copy the compiler makes is a copy`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<TallyHolder, int>(fun holder ->
                    let mutable copyOfStruct = holder.Tally
                    copyOfStruct.AddAndCount())
            @>

    let holder = TallyHolder()
    Assert.Equal(1, lambda.Compile().Invoke holder)
    Assert.Equal(0, holder.Tally.Count)
