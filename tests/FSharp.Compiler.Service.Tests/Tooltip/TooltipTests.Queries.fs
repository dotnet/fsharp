module FSharp.Compiler.Service.Tests.TooltipQueriesTests

open System
open System.IO
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.IO
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization
open TestFramework

let private dataSourceCode =
    """namespace DataSource
open System
open System.Xml.Linq

type Product() =
    let mutable id = 0
    let mutable name = ""
    let mutable category = ""
    let mutable price = 0M
    let mutable unitsInStock = 0
    member x.ProductID with get() = id and set(v) = id <- v
    member x.ProductName with get() = name and set(v) = name <- v
    member x.Category with get() = category and set(v) = category <- v
    member x.UnitPrice with get() = price and set(v) = price <- v
    member x.UnitsInStock with get() = unitsInStock and set(v) = unitsInStock <- v

module Products =
    let getProductList() =
        [
        Product(ProductID = 1, ProductName = "Chai", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 39 );
        Product(ProductID = 2, ProductName = "Chang", Category = "Beverages", UnitPrice = 19.0000M, UnitsInStock = 17 );
        Product(ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments", UnitPrice = 10.0000M, UnitsInStock = 13 );
        ]
"""

let private assertQuickInfoInQuery (expected: string) (markedFile2: string) =
    foldedProjectTooltip [ dataSourceCode ] [ sysLib "System.Xml.Linq" ] markedFile2
    |> assertFoldedTooltipContains true "query tooltip" expected

[<Fact>]
let ``Regression.ComputationExpressionMemberAppearingInQuickInfo`` () =
    let source =
        """module Test
let q2 =
    query {
        for p in [1;2] do
            join cccccc in [3;4] on (p = cccccc)
            yield ccc{caret}ccc
    }"""

    assertTooltipDoesNotContain "Yield" source
    assertTooltipContains "val cccccc: int" source

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.QuickInfoSmokeTest1`` () =
    let source = """let q = query { for x in ["1"] do selec{caret}t x }"""
    assertTooltipContains "custom operation: select" source
    assertTooltipContains "custom operation: select ('Result)" source
    assertTooltipContains "Calls" source
    assertTooltipContains "Linq.QueryBuilder.Select" source

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.QuickInfoSmokeTest2`` () =
    let source = """let q = query { for x in ["1"] do joi{caret}n y in ["2"] on (x = y); select (x,y) }"""
    assertTooltipContains "custom operation: join" source
    assertTooltipContains "join var in collection on (outerKey = innerKey)" source
    assertTooltipContains "Calls" source
    assertTooltipContains "Linq.QueryBuilder.Join" source

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.QuickInfoSmokeTest3`` () =
    let source = """let q = query { for x in ["1"] do groupJoin{caret} y in ["2"] on (x = y) into g; select (x,g) }"""
    assertTooltipContains "custom operation: groupJoin" source
    assertTooltipContains "groupJoin var in collection on (outerKey = innerKey)" source
    assertTooltipContains "Calls" source
    assertTooltipContains "Linq.QueryBuilder.GroupJoin" source

[<Fact(Skip = "bug196137:Wrong type quickinfo in the query with errors elsewhere")>]
let ``Query.WithError1.Bug196137`` () =
    assertQuickInfoInQuery
        "Product.ProductName: string"
        """open DataSource
let products = Products.getProductList()
let sortedProducts =
    query {
        for p in products do
        let x = p.ProductID + "a"
        sortBy p.ProductName{caret}
        select p
    }"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.WithError2`` () =
    assertQuickInfoInQuery
        "custom operation: minBy ('Value)"
        """open DataSource
let products = Products.getProductList()
let test =
    query {
        for p in products do
        let x = p.ProductID + "1"
        minBy{caret} p.UnitPrice
    }"""

[<Fact(Skip = "Multiple failures due to CancellationTokenSource being disposed. Bad test")>]
let ``Query.WithinLargeQuery`` () =
    let source =
        """open DataSource
let products = Products.getProductList()
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
let largequery =
    query {
        for p in products do
        sortBy p.ProductName
        thenBy p.UnitPrice
        thenByDescending p.Category
        where (p.UnitsInStock < 100)
        where (p.Category = "Condiments")
        groupValBy(*Mark1*) p p.Category into g
        let maxPrice = query { for x in g do maxBy(*Mark2*) x.UnitPrice }
        let mostExpensiveProducts = query { for x in g do where (x.UnitPrice = maxPrice) }
        select (g.Key, mostExpensiveProducts, query {
                                                    for n in numbers do
                                                    where (n%2 = 0)
                                                    where(*Mark3*) (n > 2)
                                                    where (n < 40)
                                                    select n})
        distinct(*Mark4*)
    }"""

    let at (mark: string) = source.Replace(mark, "{caret}")
    assertQuickInfoInQuery "custom operation: groupValBy ('Value) ('Key)" (at "(*Mark1*)")
    assertQuickInfoInQuery "custom operation: maxBy ('Value)" (at "(*Mark2*)")
    assertQuickInfoInQuery "custom operation: where (bool)" (at "(*Mark3*)")
    assertQuickInfoInQuery "custom operation: distinct" (at "(*Mark4*)")

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.ArgumentToQuery.OperatorError`` () =
    let source =
        """let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
let foo =
    query {
        for n in numbers do
        orderBy (n.GetType())
        select n }"""

    assertTooltipContains "val n: int" (markAtStartOfMarker source "n.GetType()")
    assertTooltipContains "System.Object.GetType() : System.Type" (markAtStartOfMarker source "Type()")

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.ArgumentToQuery.InNestedQuery`` () =
    let source =
        """open DataSource
let products = Products.getProductList()
let test1 =
    query {
        for p in products do
        sortBy p.ProductName
        select (p.ProductName, query { for f in products do
                                       groupValBy(*Mark3*) f f.Category into g
                                       let maxPrice = query { for x in g do maxBy x.UnitPrice }
                                       let mostExpensiveProducts = query { for x in g do where(*Mark1*) (x.UnitPrice = maxPrice(*Mark2*)) }
                                       select(*Mark4*) (g.Key, g)}) }"""

    let at (mark: string) = source.Replace(mark, "{caret}")
    assertQuickInfoInQuery "custom operation: where (bool)" (at "(*Mark1*)")
    assertQuickInfoInQuery "val maxPrice: decimal" (at "(*Mark2*)")
    assertQuickInfoInQuery "custom operation: groupValBy ('Value) ('Key)" (at "(*Mark3*)")
    assertQuickInfoInQuery "custom operation: select ('Result)" (at "(*Mark4*)")

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.ComputationExpression.Method`` () =
    let source =
        """open System.Collections.Generic
let chars = ["A";"B";"C"]
type WorkflowBuilder() =
    let yieldedItems = new List<string>()
    member this.Items = yieldedItems |> Array.ofSeq
    member this.Yield(item) = yieldedItems.Add(item)
    member this.YieldFrom(items : seq<string>) =
        items |> Seq.iter (fun item -> yieldedItems.Add(item.ToUpper()))
        ()
    member this.Combine(f, g) = g
    member this.Delay (f : unit -> 'a) =
        f()
    member this.Zero() = ()
    member this.Return _ = this.Items
let computationExpreQuery =
    query {
            for char in chars do
            let workflow = new WorkflowBuilder()
            let result =
                workflow {
                    yield "foo"
                    yield "bar"
                    yield! [| "a"; "b"; "c" |]
                    return ()
                }
            let t = workflow.Combine(*Mark1*)("a","b")
            let d = workflow.Zero(*Mark2*)()
            where (result |> Array.exists(fun i -> i = char))
            yield char
    }"""

    let at (mark: string) = source.Replace(mark, "{caret}")
    assertTooltipContains "member WorkflowBuilder.Combine: f: 'b0 * g: 'c1 -> 'c1" (at "(*Mark1*)")
    assertTooltipContains "member WorkflowBuilder.Zero: unit -> unit" (at "(*Mark2*)")

[<Fact>]
let ``Query.ComputationExpression.CustomOp`` () =
    let source =
        """open System
open Microsoft.FSharp.Quotations

type EventBuilder() =
    member _.For(ev:IObservable<'T>, loop:('T -> #IObservable<'U>)) : IObservable<'U> = failwith ""
    member _.Yield(v:'T) : IObservable<'T> = failwith ""
    member _.Quote(v:Quotations.Expr<'T>) : Expr<'T> = v
    member _.Run(x:Expr<'T>) = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation x :?> 'T

    [<CustomOperation("myWhere",MaintainsVariableSpace=true)>]
    member _.Where (x, [<ProjectionParameter>] f) = Observable.filter f x

    [<CustomOperation("mySelect")>]
    member _.Select (x, [<ProjectionParameter>] f) = Observable.map f x

    [<CustomOperation("scanSumBy")>]
    member inline _.ScanSumBy (source, [<ProjectionParameter>] f : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f b) LanguagePrimitives.GenericZero<'U> source

let myquery = EventBuilder()
let f = new Event<int * int >()
let e1 =
    myquery { for x in f.Publish do
                myWhere(*Mark1*) (fst x < 100)
                scanSumBy(*Mark2*) (snd x)
                }"""

    let at (mark: string) = source.Replace(mark, "{caret}")
    assertTooltipContains "custom operation: myWhere (bool)" (at "(*Mark1*)")
    assertTooltipContains "Calls EventBuilder.Where" (at "(*Mark1*)")
    assertTooltipContains "custom operation: scanSumBy ('U)" (at "(*Mark2*)")
    assertTooltipContains "Calls EventBuilder.ScanSumBy" (at "(*Mark2*)")
