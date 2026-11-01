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
