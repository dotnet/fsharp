// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.ExpressionLambdaConversionTests

open System
open System.Linq.Expressions
open Xunit
open Microsoft.FSharp.Linq.RuntimeHelpers

type LinqExpr =
    static member FromFunc<'T, 'U>(e: Expression<Func<'T, 'U>>) = e

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
let ``Lambda nested in an expression lambda is quoted`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, Expression<Func<int, int>>>(fun x -> LinqExpr.FromFunc(fun y -> y + x)) @>

    match bodyOf lambda with
    | :? MethodCallExpression as call ->
        Assert.Equal(nameof LinqExpr.FromFunc, call.Method.Name)
        Assert.Equal(ExpressionType.Quote, call.Arguments.[0].NodeType)
        Assert.Equal(ExpressionType.Lambda, (call.Arguments.[0] :?> UnaryExpression).Operand.NodeType)
    | body -> Assert.Fail $"Expected a call of FromFunc, got {body}"

    Assert.Equal(3, lambda.Compile().Invoke(1).Compile().Invoke 2)

[<Fact>]
let ``Nested lambda that reads no parameter of the tree stays a call`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, Expression<Func<int, int>>>(fun _ -> LinqExpr.FromFunc(fun y -> y + 1)) @>

    match bodyOf lambda with
    | :? MethodCallExpression as call -> Assert.Equal(ExpressionType.Call, call.Arguments.[0].NodeType)
    | body -> Assert.Fail $"Expected a call of FromFunc, got {body}"

    Assert.Equal(3, lambda.Compile().Invoke(0).Compile().Invoke 2)

[<Fact>]
let ``Let read only by a nested lambda is evaluated once for each outer element`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, Expression<Func<int, int>>>(fun _ -> let n = counter.Next() in LinqExpr.FromFunc(fun y -> y + n)) @>

    let add = lambda.Compile().Invoke(0).Compile()
    Assert.Equal(3, add.Invoke 2)
    Assert.Equal(4, add.Invoke 3)
    Assert.Equal(1, counter.Calls)

[<Fact>]
let ``Nested lambda in an evaluated quotation gets the values it captures`` () =
    let inner =
        LeafExpressionConverter.EvaluateQuotation <@ let x = 5 in LinqExpr.FromFunc(fun y -> y + x) @> :?> Expression<Func<int, int>>

    match inner.Body with
    | :? BinaryExpression as sum -> Assert.Equal(ExpressionType.Constant, sum.Right.NodeType)
    | body -> Assert.Fail $"Expected a sum, got {body}"

    Assert.Equal(7, inner.Compile().Invoke 2)

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

[<Fact>]
let ``Let of an arithmetic on the parameter is replaced by its value`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int>(fun a -> let n = a + 1 in n * n) @>

    Assert.False(hasBlock lambda)
    Assert.Equal(16, run lambda 3)

[<Fact>]
let ``Let of an arithmetic on a converted member is replaced by its value`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<Customer, float>(fun c -> let total = float c.Age * 2.5 in total + total) @>

    Assert.False(hasBlock lambda)
    Assert.Equal(15.0, lambda.Compile().Invoke(Customer(Age = 3)))

[<Fact>]
let ``Let of an arithmetic on a call keeps its block and runs the call once`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int>(fun a -> let n = counter.Next() + a in n + n) @>

    Assert.True(hasBlock lambda)
    Assert.Equal(8, run lambda 3)
    Assert.Equal(1, counter.Calls)

[<Fact>]
let ``Let read by a nested lambda that also reads the parameter is evaluated once for each outer element`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, Expression<Func<int, int>>>(fun x -> let n = counter.Next() in LinqExpr.FromFunc(fun y -> y + n + x)) @>

    let add = lambda.Compile().Invoke(0).Compile()
    Assert.Equal(3, add.Invoke 2)
    Assert.Equal(4, add.Invoke 3)
    Assert.Equal(1, counter.Calls)

[<Fact>]
let ``Lambda inside the value of a let does not make a nested lambda read the tree`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, Expression<Func<int, int>>>(fun _ -> let n = abs (counter.Next()) in LinqExpr.FromFunc(fun y -> y + n)) @>

    match bodyOf lambda with
    | :? MethodCallExpression as call -> Assert.Equal(ExpressionType.Call, call.Arguments.[0].NodeType)
    | body -> Assert.Fail $"Expected a call of FromFunc, got {body}"

    let add = lambda.Compile().Invoke(0).Compile()
    Assert.Equal(3, add.Invoke 2)
    Assert.Equal(4, add.Invoke 3)
    Assert.Equal(1, counter.Calls)

[<Fact>]
let ``Nested lambda that cannot be converted fails only where it is reached`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@
                Func<int, Expression<Func<int, bool>>>(fun x ->
                    if x < 0 then
                        LinqExpr.FromFunc(fun y -> (try y = x with _ -> false))
                    else
                        LinqExpr.FromFunc(fun y -> y = x))
            @>

    Assert.True(lambda.Compile().Invoke(1).Compile().Invoke 1)

[<Fact>]
let ``Record fields written out of order are evaluated as written in an expression lambda`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, Reading>(fun _ -> { Second = counter.Next(); First = counter.Next() }) @>

    Assert.Equal({ First = 2; Second = 1 }, run lambda 0)

[<Fact>]
let ``Record fields written out of order are evaluated as written in a query over a queryable`` () =
    let counter = Counter()
    let source = System.Linq.Queryable.AsQueryable [ 1 ]

    let rows =
        query {
            for _ in source do
                select { Second = counter.Next(); First = counter.Next() }
        }
        |> Seq.toList

    Assert.Equal<Reading list>([ { First = 2; Second = 1 } ], rows)

[<Fact>]
let ``Record fields written out of order are evaluated as written in a tuple of a query over a queryable`` () =
    let counter = Counter()
    let source = System.Linq.Queryable.AsQueryable [ 1; 2 ]

    let rows =
        query {
            for x in source do
                select (x, { Second = counter.Next(); First = counter.Next() })
        }
        |> Seq.toList

    Assert.Equal<(int * Reading) list>([ (1, { First = 2; Second = 1 }); (2, { First = 4; Second = 3 }) ], rows)
    Assert.Equal(4, counter.Calls)

[<Fact>]
let ``Record fields written out of order are evaluated as written in a tuple of a query over a sequence`` () =
    let counter = Counter()

    let rows =
        query {
            for x in [ 1; 2 ] do
                select (x, { Second = counter.Next(); First = counter.Next() })
        }
        |> Seq.toList

    Assert.Equal<(int * Reading) list>([ (1, { First = 2; Second = 1 }); (2, { First = 4; Second = 3 }) ], rows)
    Assert.Equal(4, counter.Calls)

[<Fact>]
let ``Record fields written out of order are evaluated after the items of the tuple that come before them`` () =
    let counter = Counter()
    let source = System.Linq.Queryable.AsQueryable [ 1; 2 ]

    let rows =
        query {
            for _ in source do
                select (counter.Next(), (0, { Second = counter.Next(); First = counter.Next() }))
        }
        |> Seq.toList

    Assert.Equal<(int * (int * Reading)) list>(
        [ (1, (0, { First = 3; Second = 2 })); (4, (0, { First = 6; Second = 5 })) ],
        rows
    )

    Assert.Equal(6, counter.Calls)

/// A record that is equal to another when its first field is
[<CustomEquality; NoComparison>]
type HeadOnly =
    { Head: int
      Tail: int }

    override x.Equals(other: obj) =
        match other with
        | :? HeadOnly as y -> x.Head = y.Head
        | _ -> false

    override x.GetHashCode() = x.Head.GetHashCode()

[<Fact>]
let ``Distinct tells apart the records of a query by their fields, written in order or not`` () =
    let source = System.Linq.Queryable.AsQueryable [ 1; 2 ]

    // The query makes anonymous objects of the records it selects, which are equal when their fields are. That holds
    // for the records with an equality of their own too, and fields written out of order change nothing about it.
    let inOrder = query { for x in source do select { Head = 0; Tail = x + 1 }; distinct } |> Seq.length
    let outOfOrder = query { for x in source do select { Tail = x + 1; Head = 0 }; distinct } |> Seq.length

    Assert.Equal(2, inOrder)
    Assert.Equal(2, outOfOrder)

[<Fact>]
let ``Let named like the record field it feeds is evaluated before the fields that follow it`` () =
    let counter = Counter()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<int, Reading>(fun _ -> let Second = counter.Next() in { First = counter.Next(); Second = Second }) @>

    Assert.Equal({ First = 2; Second = 1 }, run lambda 0)

[<Fact>]
let ``Let named like the record field it feeds is evaluated before the fields that follow it in a query over a queryable`` () =
    let counter = Counter()
    let source = System.Linq.Queryable.AsQueryable [ 1 ]

    let rows =
        query {
            for _ in source do
                select (
                    let Second = counter.Next()
                    { First = counter.Next(); Second = Second }
                )
        }
        |> Seq.toList

    Assert.Equal<Reading list>([ { First = 2; Second = 1 } ], rows)

[<Fact>]
let ``String made by a call is one string where a use can tell one from another`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, bool>(fun s -> let upper = s.ToUpperInvariant() in obj.ReferenceEquals(upper, upper)) @>

    Assert.True(hasBlock lambda)
    Assert.True(lambda.Compile().Invoke "ab")

[<Fact>]
let ``String made by a call is made at each use that only looks at it`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, bool>(fun s -> let upper = s.ToUpperInvariant() in upper.StartsWith "A" && upper.EndsWith "B") @>

    Assert.False(hasBlock lambda)
    Assert.True(lambda.Compile().Invoke "ab")

[<Fact>]
let ``String made by a call is one string where a method of the string hands it on`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, bool>(fun s ->
                let upper = s.ToUpperInvariant()
                obj.ReferenceEquals(upper.Substring(0), upper.Substring(0))) @>

    // Substring 0 gives the string it is called on
    Assert.True(hasBlock lambda)
    Assert.True(lambda.Compile().Invoke "ab")

[<Fact>]
let ``String made by a call is one string where Clone hands it on`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, bool>(fun s ->
                let upper = s.ToUpperInvariant()
                obj.ReferenceEquals(upper.Clone(), upper.Clone())) @>

    Assert.True(hasBlock lambda)
    Assert.True(lambda.Compile().Invoke "ab")

[<Fact>]
let ``String made by a call is one string for every element of a nested lambda that hands it on`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, Collections.Generic.IEnumerable<string>>(fun s ->
                let upper = s.ToUpperInvariant()
                System.Linq.Enumerable.Select([| 1; 2 |], Func<int, string>(fun _ -> upper))) @>

    let items = Seq.toArray (lambda.Compile().Invoke "ab")

    Assert.True(hasBlock lambda)
    Assert.True(obj.ReferenceEquals(items.[0], items.[1]))

[<Fact>]
let ``String made by a call is made for every element of a nested lambda that only looks at it`` () =
    let names = Collections.Generic.List<string>([ "a"; "AB"; "b"; "AB" ])

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, int>(fun s ->
                let upper = s.ToUpperInvariant()
                System.Linq.Enumerable.Count(names, Func<string, bool>(fun n -> n.StartsWith(upper)))) @>

    Assert.False(hasBlock lambda)
    Assert.Equal(2, lambda.Compile().Invoke "ab")

[<Fact>]
let ``String made by a call is one string where an operator hands it on`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, bool>(fun s ->
                let upper = s.ToUpperInvariant()
                obj.ReferenceEquals(upper + "", upper + "")) @>

    // Adding an empty string gives the string it is added to
    Assert.True(hasBlock lambda)
    Assert.True(lambda.Compile().Invoke "ab")

[<Fact>]
let ``String made by a call is made at each use where a method gives only a number or a boolean of a string made from it`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, bool>(fun s ->
                let upper = s.ToUpperInvariant()
                upper.Trim().Length > 1 && upper.Substring(1).Length < 9 && upper.StartsWith "A") @>

    Assert.False(hasBlock lambda)
    Assert.True(lambda.Compile().Invoke "ab")

[<Fact>]
let ``String made by a call is one string where one use hands it on and another only looks at it`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, string * bool>(fun s ->
                let upper = s.ToUpperInvariant()
                upper, upper.StartsWith "A") @>

    // Only one place can tell one object from another
    Assert.False(hasBlock lambda)
    Assert.Equal(("AB", true), lambda.Compile().Invoke "ab")

/// A parameter whose name counts how often it is read
type CountedPerson() =
    let mutable nameReads = 0
    member _.NameReads = nameReads

    member _.Name =
        nameReads <- nameReads + 1
        "ab"

[<Fact>]
let ``Member of the parameter that a let names is read at each use`` () =
    let person = CountedPerson()

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<CountedPerson, string>(fun p -> let n = p.Name in n + n) @>

    Assert.Equal("abab", lambda.Compile().Invoke person)
    Assert.Equal(2, person.NameReads)

[<Fact>]
let ``Member of the parameter that a nested lambda uses is read for each element of the nested lambda`` () =
    let person = CountedPerson()
    let names = Collections.Generic.List<string>([ "a"; "ab"; "b"; "ab" ])

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<CountedPerson, int>(fun p -> let n = p.Name in System.Linq.Enumerable.Count(names, Func<string, bool>(fun s -> s = n))) @>

    Assert.Equal(2, lambda.Compile().Invoke person)
    Assert.Equal(4, person.NameReads)

[<Fact>]
let ``Member of the parameter that a let names is read after a call that changes it`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<Collections.Generic.List<int>, int>(fun items -> let before = items.Count in addTo items 5 + before) @>

    // The read is made where the variable is used, after the call
    Assert.Equal(2, lambda.Compile().Invoke(Collections.Generic.List<int>()))

[<Fact>]
let ``Let whose value would throw is not evaluated on a path that does not use it`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<string, string>(fun name -> let s = name.Substring 5 in if name.Length > 5 then s else "") @>

    Assert.Equal("", lambda.Compile().Invoke "ab")

[<Fact>]
let ``EvaluateQuotation replaces a let inside a lambda that goes to a provider`` () =
    let person = CountedPerson()
    let people = System.Linq.Queryable.AsQueryable [ person ]

    let names =
        LeafExpressionConverter.EvaluateQuotation
            <@ System.Linq.Enumerable.ToArray(System.Linq.Queryable.Select(people, fun x -> let n = x.Name in n + n)) @>
        :?> string[]

    Assert.Equal<string[]>([| "abab" |], names)
    Assert.Equal(2, person.NameReads)

[<Fact>]
let ``Let of a division by a constant is replaced by its value`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int>(fun a -> let half = a / 2 in half + half) @>

    Assert.False(hasBlock lambda)
    Assert.Equal(6, run lambda 7)

[<Fact>]
let ``Let of a division by a parameter keeps its block so that its failure is not skipped`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<int, int>(fun a -> let quotient = 10 / a in if a > 100 then quotient else 0) @>

    Assert.True(hasBlock lambda)
    Assert.Throws<DivideByZeroException>(fun () -> run lambda 0 |> ignore) |> ignore

/// A record with a division by a character, a divisor that is no number to convert
type CharBox =
    { Code: int }

    static member (/)(b: CharBox, divisor: char) : char = char (b.Code / int divisor)

[<Fact>]
let ``Let of a division by a captured native integer is converted`` () =
    let divisor = 2n

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<nativeint, nativeint>(fun a -> let half = a / divisor in half + half) @>

    Assert.Equal(10n, lambda.Compile().Invoke 10n)

[<Fact>]
let ``Let of a remainder by a captured unsigned native integer is converted`` () =
    let divisor = 4un

    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression
            <@ Func<unativeint, unativeint>(fun a -> let rest = a % divisor in rest + rest) @>

    Assert.Equal(4un, lambda.Compile().Invoke 10un)

[<Fact>]
let ``Let of a division by a character with an operator of the user's is converted`` () =
    let lambda =
        LeafExpressionConverter.QuotationToLambdaExpression <@ Func<CharBox, char>(fun b -> let half = b / '\002' in half) @>

    Assert.Equal('\001', lambda.Compile().Invoke { Code = 2 })

[<Fact>]
let ``Query that selects a let of a division by a captured native integer is translated`` () =
    let divisor = 2n
    let numbers = System.Linq.Queryable.AsQueryable [ 4n; 10n ]

    let doubled = query { for n in numbers do select (let half = n / divisor in half + half) } |> Seq.toArray

    Assert.Equal<nativeint[]>([| 4n; 10n |], doubled)
