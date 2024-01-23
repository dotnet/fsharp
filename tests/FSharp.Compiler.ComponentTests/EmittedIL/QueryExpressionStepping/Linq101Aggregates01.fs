// #Conformance #DataExpressions #Query
// GroupBy LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

// Count - Simple
let factorsOf300 = [2;2;3;5;5]

let uniqueFactors = 
    query {
        for n in factorsOf300 do
        distinct
    } |> Seq.length

// Sum - Simple
let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

let numSum = 
    query {
        for n in numbers do
        sumBy n
    }

// Sum - Projection
let words = ["cherry"; "apple"; "blueberry"]

let totalChars = 
    query {
        for w in words do
        sumBy (w.Length)
    }

// Sum - Grouped
let products = getProductList()

let categories =
    query {
        for p in products do
        groupValBy p p.Category into g
        let sum = query {
            for x in g do
            sumBy x.UnitsInStock
        }
        select (g.Key, sum)
    } |> Seq.toArray

// Min - simple
let minNum = query { for n in numbers do minBy n }

// Min - Projection
let shortestWord = query { for w in words do minBy w.Length }

// Min - Grouped
let categories2 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let min = query { for x in g do minBy x.UnitPrice }
        select (g.Key, min)
    } |> Seq.toArray

// Min - Elements
let categories3 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let min = g.Min(fun (p : Product) -> p.UnitPrice)
        let cheapestProducts = query { for x in g do where (x.UnitPrice = min) }
        select (g.Key, cheapestProducts)
    } |> Seq.toArray

// Max - Simple
let maxNum = query { for n in numbers do maxBy n }

// Max - Projection
let longestLength = query { for w in words do maxBy w.Length }

// Max - Grouped
let categories4 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let mostExpensivePrice = query { for x in g do maxBy x.UnitPrice }
        select (g.Key, mostExpensivePrice)
    } |> Seq.toArray

// Max - Elements
let categories5 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let maxPrice = query { for x in g do maxBy x.UnitPrice }
        let mostExpensiveProducts = query { for x in g do where (x.UnitPrice = maxPrice) }
        select (g.Key, mostExpensiveProducts)
    } |> Seq.toArray

// Average - Simple
let numbers2 = [5.1; 4.1; 1.1; 3.1; 9.1; 8.1; 6.1; 7.1; 2.1; 0.1]
let averageNum = query { for n in numbers2 do averageBy n }

// Average - Projection
let averageLength = 
    query { 
        for w in words do 
        let wl = w.Length |> float
        averageBy wl
    }

// Average - Grouped
let categories6 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let averagePrice = query { for x in g do averageBy x.UnitPrice }
        select (g.Key, averagePrice)
    } |> Seq.toArray