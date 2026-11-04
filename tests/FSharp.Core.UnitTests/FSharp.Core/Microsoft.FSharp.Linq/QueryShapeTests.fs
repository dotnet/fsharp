// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Linq

open System
open System.Collections.Generic
open System.Linq
open System.Linq.Expressions
open System.Reflection
open Xunit

type QueryShapeTests() =

    static let rows (cases: Scenario list) =
        cases |> Seq.map (fun scenario -> [| box scenario |])

    static member Projections = rows (QueryProjectionScenarios.cases ())
    static member Operators = rows (QueryOperatorScenarios.cases ())
    static member Keys = rows (QueryKeyScenarios.cases ())
    static member Structures = rows (QueryStructureScenarios.cases ())
    static member MethodSyntax = rows (QueryMethodSyntaxScenarios.cases ())

    [<Theory; MemberData(nameof QueryShapeTests.Projections)>]
    member _.Projection(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.Operators)>]
    member _.Operator(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.Keys)>]
    member _.Key(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.Structures)>]
    member _.Structure(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.MethodSyntax)>]
    member _.``Method syntax``(scenario: Scenario) = Scenario.check scenario

/// Trees built by hand, the way a C# compiler builds them, to check the validator itself. They do not depend on how
/// FSharp.Core translates a query, so they mean the same in every branch.
module private ValidatorTrees =

    let customers = Queryable.AsQueryable<Customer> Seq.empty
    let orders = Queryable.AsQueryable<Order> Seq.empty

    let customerPair = typeof<Tuple<int, string>>
    let orderPair = typeof<Tuple<int, float>>

    let call (name: string) (typeArguments: Type list) (arguments: Expression list) =
        Expression.Call(typeof<Queryable>, name, Array.ofList typeArguments, Array.ofList arguments) :> Expression

    let quote (lambda: LambdaExpression) = Expression.Quote lambda :> Expression

    let pair (tuple: Type) (arguments: Expression list) (named: bool) =
        let constructor = tuple.GetConstructors().[0]

        if named then
            Expression.New(constructor, arguments, [| tuple.GetProperty "Item1" :> MemberInfo; tuple.GetProperty "Item2" :> MemberInfo |])
        else
            Expression.New(constructor, arguments)

    /// customers.Select(c => new Tuple<int, string>(c.Id, c.Name)), with or without naming the members
    let customerPairs (named: bool) =
        let c = Expression.Parameter(typeof<Customer>, "c")
        let body = pair customerPair [ Expression.Property(c, "Id"); Expression.Property(c, "Name") ] named

        call "Select" [ typeof<Customer>; customerPair ] [ customers.Expression; quote (Expression.Lambda(body, c)) ]

    /// orders.Select(o => new Tuple<int, double>(o.Id, o.Total)), with or without naming the members
    let orderPairs (named: bool) =
        let o = Expression.Parameter(typeof<Order>, "o")
        let body = pair orderPair [ Expression.Property(o, "Id"); Expression.Property(o, "Total") ] named

        call "Select" [ typeof<Order>; orderPair ] [ orders.Expression; quote (Expression.Lambda(body, o)) ]

    /// source.Where(t => t.Item1 > 0)
    let whereItem1Positive (source: Expression) (tuple: Type) =
        let t = Expression.Parameter(tuple, "t")
        let predicate = Expression.Lambda(Expression.GreaterThan(Expression.Property(t, "Item1"), Expression.Constant(0)), t)

        call "Where" [ tuple ] [ source; quote predicate ]

    let item1 (p: ParameterExpression) = Expression.Property(p, "Item1") :> Expression
    let item2 (p: ParameterExpression) = Expression.Property(p, "Item2") :> Expression
    let zero (_: ParameterExpression) = Expression.Constant(0) :> Expression

    /// outer.Join(inner, o => outerKey(o), i => innerKey(i), (o, i) => result(o, i)), with int keys
    let join
        (outer: Expression)
        (outerElement: Type)
        (inner: Expression)
        (innerElement: Type)
        (outerKey: ParameterExpression -> Expression)
        (innerKey: ParameterExpression -> Expression)
        (result: ParameterExpression -> ParameterExpression -> Expression)
        =
        let parameter (ty: Type) name = Expression.Parameter(ty, name)
        let o1 = parameter outerElement "o"
        let i1 = parameter innerElement "i"
        let o2 = parameter outerElement "o"
        let i2 = parameter innerElement "i"
        let resultBody = result o2 i2

        call
            "Join"
            [ outerElement; innerElement; typeof<int>; resultBody.Type ]
            [ outer
              inner
              quote (Expression.Lambda(outerKey o1, o1))
              quote (Expression.Lambda(innerKey i1, i1))
              quote (Expression.Lambda(resultBody, o2, i2)) ]

type ProviderShapeValidatorTests() =

    static let unnamedRead = [ Violation.ReadsUnnamedMember "Tuple`2" ]

    static let reported tree = ProviderShape.violations tree

    [<Fact>]
    member _.``A member read from an unnamed projection is reported``() =
        let unnamed = ValidatorTrees.whereItem1Positive (ValidatorTrees.customerPairs false) ValidatorTrees.customerPair
        let named = ValidatorTrees.whereItem1Positive (ValidatorTrees.customerPairs true) ValidatorTrees.customerPair

        Assert.Equal<Violation list>(unnamedRead, reported unnamed)
        Assert.Empty(reported named)

    [<Fact>]
    member _.``A chain inside the lambda of another operator is checked``() =
        let nested named =
            let inner = ValidatorTrees.whereItem1Positive (ValidatorTrees.orderPairs named) ValidatorTrees.orderPair
            let any = ValidatorTrees.call "Any" [ ValidatorTrees.orderPair ] [ inner ]
            let c = Expression.Parameter(typeof<Customer>, "c")

            ValidatorTrees.call "Where" [ typeof<Customer> ] [ ValidatorTrees.customers.Expression; ValidatorTrees.quote (Expression.Lambda(any, c)) ]

        Assert.Equal<Violation list>(unnamedRead, reported (nested false))
        Assert.Empty(reported (nested true))

    [<Fact>]
    member _.``A query captured from earlier is checked where it is used``() =
        let capturing named =
            let captured = ValidatorTrees.customers.Provider.CreateQuery<Tuple<int, string>>(ValidatorTrees.customerPairs named)
            let o = Expression.Parameter(typeof<Order>, "o")
            let t = Expression.Parameter(ValidatorTrees.customerPair, "t")
            let predicate = Expression.Lambda(Expression.GreaterThan(Expression.Property(t, "Item1"), Expression.Property(o, "Id")), t)
            let any = ValidatorTrees.call "Any" [ ValidatorTrees.customerPair ] [ Expression.Constant(captured) :> Expression; ValidatorTrees.quote predicate ]

            ValidatorTrees.call "Where" [ typeof<Order> ] [ ValidatorTrees.orders.Expression; ValidatorTrees.quote (Expression.Lambda(any, o)) ]

        Assert.Equal<Violation list>(unnamedRead, reported (capturing false))
        Assert.Empty(reported (capturing true))

    [<Fact>]
    member _.``An identity Select keeps the elements unnamed``() =
        let t = Expression.Parameter(ValidatorTrees.customerPair, "t")

        let identity =
            ValidatorTrees.call
                "Select"
                [ ValidatorTrees.customerPair; ValidatorTrees.customerPair ]
                [ ValidatorTrees.customerPairs false; ValidatorTrees.quote (Expression.Lambda(t, t)) ]

        Assert.Equal<Violation list>(unnamedRead, reported (ValidatorTrees.whereItem1Positive identity ValidatorTrees.customerPair))

    [<Fact>]
    member _.``The elements of a collection selector stay unnamed after SelectMany``() =
        let flattened named =
            let c = Expression.Parameter(typeof<Customer>, "c")
            let collectionType = typedefof<Func<_, _>>.MakeGenericType(typeof<Customer>, typeof<IEnumerable<Tuple<int, float>>>)
            let collection = Expression.Lambda(collectionType, ValidatorTrees.orderPairs named, c)

            ValidatorTrees.call
                "SelectMany"
                [ typeof<Customer>; ValidatorTrees.orderPair ]
                [ ValidatorTrees.customers.Expression; ValidatorTrees.quote collection ]

        Assert.Equal<Violation list>(unnamedRead, reported (ValidatorTrees.whereItem1Positive (flattened false) ValidatorTrees.orderPair))
        Assert.Empty(reported (ValidatorTrees.whereItem1Positive (flattened true) ValidatorTrees.orderPair))

    [<Fact>]
    member _.``A named input of a join is not blamed for the other one being unnamed``() =
        // The two inputs have the same tuple type, and only the outer one is unnamed. Neither key reads a member of it.
        let tree =
            ValidatorTrees.join
                (ValidatorTrees.customerPairs false)
                ValidatorTrees.customerPair
                (ValidatorTrees.customerPairs true)
                ValidatorTrees.customerPair
                ValidatorTrees.zero
                ValidatorTrees.item1
                (fun _ _ -> ValidatorTrees.zero null)

        Assert.Empty(reported tree)

    [<Fact>]
    member _.``An unnamed inner input of a join is checked against the inner key``() =
        let tree =
            ValidatorTrees.join
                (ValidatorTrees.customerPairs true)
                ValidatorTrees.customerPair
                (ValidatorTrees.customerPairs false)
                ValidatorTrees.customerPair
                ValidatorTrees.zero
                ValidatorTrees.item1
                (fun _ _ -> ValidatorTrees.zero null)

        Assert.Equal<Violation list>(unnamedRead, reported tree)

    [<Fact>]
    member _.``An unnamed inner input of a join is checked against the second parameter of the result``() =
        let tree =
            ValidatorTrees.join
                (ValidatorTrees.customerPairs true)
                ValidatorTrees.customerPair
                (ValidatorTrees.customerPairs false)
                ValidatorTrees.customerPair
                ValidatorTrees.zero
                ValidatorTrees.zero
                (fun _ i -> ValidatorTrees.item2 i)

        Assert.Equal<Violation list>(unnamedRead, reported tree)

    [<Fact>]
    member _.``An unnamed inner input of a join is checked when the outer input is a plain source``() =
        let tree =
            ValidatorTrees.join
                ValidatorTrees.customers.Expression
                typeof<Customer>
                (ValidatorTrees.orderPairs false)
                ValidatorTrees.orderPair
                (fun c -> Expression.Property(c, "Id") :> Expression)
                ValidatorTrees.item1
                (fun _ _ -> ValidatorTrees.zero null)

        Assert.Equal<Violation list>(unnamedRead, reported tree)

    [<Fact>]
    member _.``A selector that is not a lambda is passed over``() =
        let selector =
            Expression.Constant(
                Expression.Lambda<Func<Customer, int>>(Expression.Constant(0), Expression.Parameter(typeof<Customer>, "c")),
                typeof<Expression<Func<Customer, int>>>
            )

        let tree =
            ValidatorTrees.call "Select" [ typeof<Customer>; typeof<int> ] [ ValidatorTrees.customers.Expression; selector :> Expression ]

        Assert.Empty(reported tree)

    [<Fact>]
    member _.``A block is reported once for each block``() =
        let c = Expression.Parameter(typeof<Customer>, "c")

        let block (name: string) =
            let x = Expression.Variable(typeof<int>, name)
            Expression.Block([| x |], Expression.Assign(x, Expression.Property(c, "Id")), Expression.GreaterThan(x, Expression.Constant 0)) :> Expression

        let where (body: Expression) =
            ValidatorTrees.call "Where" [ typeof<Customer> ] [ ValidatorTrees.customers.Expression; ValidatorTrees.quote (Expression.Lambda(body, c)) ]

        let blockViolation = Violation.Statement ExpressionType.Block

        Assert.Equal<Violation list>([ blockViolation ], reported (where (block "x")))
        Assert.Equal<Violation list>([ blockViolation; blockViolation ], reported (where (Expression.AndAlso(block "x", block "y"))))

    [<Fact>]
    member _.``A lambda that is invoked is reported``() =
        let c = Expression.Parameter(typeof<Customer>, "c")
        let n = Expression.Parameter(typeof<int>, "n")
        let positive = Expression.Lambda<Func<int, bool>>(Expression.GreaterThan(n, Expression.Constant 0), n)
        let tree = Expression.Invoke(positive, Expression.Property(c, "Id"))

        let where =
            ValidatorTrees.call "Where" [ typeof<Customer> ] [ ValidatorTrees.customers.Expression; ValidatorTrees.quote (Expression.Lambda(tree, c)) ]

        Assert.Equal<Violation list>([ Violation.LambdaInvocation ], reported where)

    [<Fact>]
    member _.``A parameter that no lambda binds is reported``() =
        let c = Expression.Parameter(typeof<Customer>, "c")
        let n = Expression.Parameter(typeof<int>, "n")

        let where =
            ValidatorTrees.call
                "Where"
                [ typeof<Customer> ]
                [ ValidatorTrees.customers.Expression
                  ValidatorTrees.quote (Expression.Lambda(Expression.GreaterThan(Expression.Property(c, "Id"), n), c)) ]

        Assert.Equal<Violation list>([ Violation.UnboundParameter "n" ], reported where)

    [<Fact>]
    member _.``A count made by the provider passes``() =
        Scenario.value "count by the provider" (fun db -> db.Customers.Where(fun c -> c.Age > 26).Count()) (fun _ -> 3)
        |> Scenario.check

    [<Fact>]
    member _.``A count made over the rows of the provider leaves it``() =
        Scenario.value "count on the client" (fun db -> db.Customers.Where(fun c -> c.Age > 26).AsEnumerable().Count()) (fun _ -> 3)
        |> Scenario.violating [ Violation.LeftProvider ]
        |> Scenario.check

        Scenario.value "count by Enumerable" (fun db -> Enumerable.Count(db.Customers.Where(fun c -> c.Age > 26))) (fun _ -> 3)
        |> Scenario.violating [ Violation.LeftProvider ]
        |> Scenario.check

    [<Fact>]
    member _.``A count made by the client over counts made by the provider leaves it``() =
        // The provider counts the orders once for each customer, and the client counts the customers
        Scenario.value
            "client count with a provider count in its projection"
            (fun db -> db.Customers.Where(fun c -> c.Age > 26).AsEnumerable().Select(fun _ -> db.Orders.Count()).Count())
            (fun _ -> 3)
        |> Scenario.violating [ Violation.LeftProvider ]
        |> Scenario.check

        Scenario.value
            "client count with an inner provider count"
            (fun db -> Enumerable.Count(db.Customers.Select(fun _ -> db.Orders.Count())))
            (fun data -> data.Customers.Length)
        |> Scenario.violating [ Violation.LeftProvider ]
        |> Scenario.check

    [<Fact>]
    member _.``A result selector that gives back an unnamed element keeps the elements unnamed``() =
        let item = Expression.Parameter(typeof<int>, "item")

        let flattened named =
            let pair = Expression.Parameter(ValidatorTrees.customerPair, "pair")
            let collectionType = typedefof<Func<_, _>>.MakeGenericType(ValidatorTrees.customerPair, typeof<IEnumerable<int>>)
            let collection = Expression.Lambda(collectionType, Expression.NewArrayInit(typeof<int>, Expression.Constant 0), pair)
            let first = Expression.Parameter(ValidatorTrees.customerPair, "pair")

            ValidatorTrees.call
                "SelectMany"
                [ ValidatorTrees.customerPair; typeof<int>; ValidatorTrees.customerPair ]
                [ ValidatorTrees.customerPairs named
                  ValidatorTrees.quote collection
                  ValidatorTrees.quote (Expression.Lambda(first, first, item)) ]

        Assert.Equal<Violation list>(unnamedRead, reported (ValidatorTrees.whereItem1Positive (flattened false) ValidatorTrees.customerPair))
        Assert.Empty(reported (ValidatorTrees.whereItem1Positive (flattened true) ValidatorTrees.customerPair))

    [<Fact>]
    member _.``The second parameter of a SelectMany result selector is an element of the collection``() =
        let collected named =
            let customer = Expression.Parameter(typeof<Customer>, "c")
            let collectionType = typedefof<Func<_, _>>.MakeGenericType(typeof<Customer>, typeof<IEnumerable<Tuple<int, float>>>)
            let collection = Expression.Lambda(collectionType, ValidatorTrees.orderPairs named, customer)
            let owner = Expression.Parameter(typeof<Customer>, "c")
            let order = Expression.Parameter(ValidatorTrees.orderPair, "o")

            ValidatorTrees.call
                "SelectMany"
                [ typeof<Customer>; ValidatorTrees.orderPair; ValidatorTrees.orderPair ]
                [ ValidatorTrees.customers.Expression
                  ValidatorTrees.quote collection
                  ValidatorTrees.quote (Expression.Lambda(order, owner, order)) ]

        Assert.Equal<Violation list>(unnamedRead, reported (ValidatorTrees.whereItem1Positive (collected false) ValidatorTrees.orderPair))
        Assert.Empty(reported (ValidatorTrees.whereItem1Positive (collected true) ValidatorTrees.orderPair))

    [<Fact>]
    member _.``A join or a zip that gives back an unnamed element keeps the elements unnamed``() =
        let joined outerNamed innerNamed (result: ParameterExpression -> ParameterExpression -> Expression) =
            ValidatorTrees.join
                (ValidatorTrees.customerPairs outerNamed)
                ValidatorTrees.customerPair
                (ValidatorTrees.customerPairs innerNamed)
                ValidatorTrees.customerPair
                ValidatorTrees.zero
                ValidatorTrees.zero
                result

        let check tree = reported (ValidatorTrees.whereItem1Positive tree ValidatorTrees.customerPair)

        Assert.Equal<Violation list>(unnamedRead, check (joined false true (fun o _ -> o)))
        Assert.Empty(check (joined true false (fun o _ -> o)))
        Assert.Equal<Violation list>(unnamedRead, check (joined true false (fun _ i -> i)))
        Assert.Empty(check (joined false true (fun _ i -> i)))

    [<Fact>]
    member _.``A zip that gives back an unnamed element keeps the elements unnamed``() =
        let zipped firstNamed secondNamed =
            let a = Expression.Parameter(ValidatorTrees.customerPair, "a")
            let b = Expression.Parameter(ValidatorTrees.customerPair, "b")

            ValidatorTrees.whereItem1Positive
                (ValidatorTrees.call
                    "Zip"
                    [ ValidatorTrees.customerPair; ValidatorTrees.customerPair; ValidatorTrees.customerPair ]
                    [ ValidatorTrees.customerPairs firstNamed
                      ValidatorTrees.customerPairs secondNamed
                      ValidatorTrees.quote (Expression.Lambda(a, a, b)) ])
                ValidatorTrees.customerPair

        Assert.Equal<Violation list>(unnamedRead, reported (zipped false true))
        Assert.Empty(reported (zipped true false))

    [<Fact>]
    member _.``The element an Aggregate function takes is checked, and a seed that is accumulated is not``() =
        // customers.Select(c => new Tuple<int, string>(c.Id, c.Name)).Aggregate(0, (acc, pair) => acc + pair.Item1)
        let summed named =
            let acc = Expression.Parameter(typeof<int>, "acc")
            let pair = Expression.Parameter(ValidatorTrees.customerPair, "pair")
            let func = Expression.Lambda(Expression.Add(acc, ValidatorTrees.item1 pair), acc, pair)

            ValidatorTrees.call
                "Aggregate"
                [ ValidatorTrees.customerPair; typeof<int> ]
                [ ValidatorTrees.customerPairs named; Expression.Constant 0; ValidatorTrees.quote func ]

        Assert.Equal<Violation list>(unnamedRead, reported (summed false))
        Assert.Empty(reported (summed true))

        // The accumulated value has the type of the elements here, and a member read from it is no read from an element
        let counted =
            let acc = Expression.Parameter(ValidatorTrees.customerPair, "acc")
            let pair = Expression.Parameter(ValidatorTrees.customerPair, "pair")
            let seed = ValidatorTrees.pair ValidatorTrees.customerPair [ Expression.Constant 0; Expression.Constant "" ] true
            let count = Expression.Add(ValidatorTrees.item1 acc, Expression.Constant 1)
            let next = ValidatorTrees.pair ValidatorTrees.customerPair [ count; Expression.Constant "" ] true

            ValidatorTrees.call
                "Aggregate"
                [ ValidatorTrees.customerPair; ValidatorTrees.customerPair ]
                [ ValidatorTrees.customerPairs false; seed :> Expression; ValidatorTrees.quote (Expression.Lambda(next, acc, pair)) ]

        Assert.Empty(reported counted)

    [<Fact>]
    member _.``Both parameters of an Aggregate function with no seed are elements``() =
        // customers.Select(c => new Tuple<int, string>(c.Id, c.Name)).Aggregate((a, b) => new Tuple<int, string>(a.Item1 + b.Item1, a.Item2))
        let combined named =
            let a = Expression.Parameter(ValidatorTrees.customerPair, "a")
            let b = Expression.Parameter(ValidatorTrees.customerPair, "b")
            let sum = Expression.Add(ValidatorTrees.item1 a, ValidatorTrees.item1 b)
            let body = ValidatorTrees.pair ValidatorTrees.customerPair [ sum; ValidatorTrees.item2 a ] true

            ValidatorTrees.call
                "Aggregate"
                [ ValidatorTrees.customerPair ]
                [ ValidatorTrees.customerPairs named; ValidatorTrees.quote (Expression.Lambda(body, a, b)) ]

        Assert.Equal<Violation list>(unnamedRead @ unnamedRead, reported (combined false))
        Assert.Empty(reported (combined true))

    [<Fact>]
    member _.``An entity that is wrong in a property other than its key does not pass``() =
        let fails (build: Customer -> Customer) =
            let scenario =
                Scenario.value
                    "payload"
                    (fun db ->
                        query {
                            for c in db.Customers do
                            select (build c) into p
                            head
                        })
                    (fun data -> data.Customers.[0])

            Scenario.failures scenario |> List.exists (fun failure -> failure.IndexOf("rows: expected", StringComparison.Ordinal) >= 0)

        let copy (c: Customer) = Customer(Id = c.Id, Name = c.Name, City = c.City, Age = c.Age, Registered = c.Registered)

        Assert.True(fails (fun c -> Customer(Id = c.Id, Name = "CORRUPTED", City = c.City, Age = -1, Registered = c.Registered)))
        Assert.True(fails (fun c -> Customer(Id = c.Id, Name = c.Name, City = c.City, Age = c.Age)))
        Assert.False(fails copy)
