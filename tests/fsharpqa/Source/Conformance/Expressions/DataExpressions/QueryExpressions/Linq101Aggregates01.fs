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
if uniqueFactors <> 3 then printfn "uniqueFactors failed"; exit 1

// Sum - Simple
let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

let numSum = 
    query {
        for n in numbers do
        sumBy n
    }
if numSum <> 45 then printfn "numSum failed"; exit 1

// Sum - Projection
let words = ["cherry"; "apple"; "blueberry"]

let totalChars = 
    query {
        for w in words do
        sumBy (w.Length)
    }
if totalChars <> 20 then printfn "totalChars failed"; exit 1

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
if categories.Length <> 8 || (categories.[0] <> ("Beverages", 559)) then printfn "categories failed"; exit 1

// Min - simple
let minNum = query { for n in numbers do minBy n }
if minNum <> 0 then printfn "minNum failed"; exit 1

// Min - Projection
let shortestWord = query { for w in words do minBy w.Length }
if shortestWord <> 5 then printfn "shortestWord failed"; exit 1

// Min - Grouped
let categories2 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let min = query { for x in g do minBy x.UnitPrice }
        select (g.Key, min)
    } |> Seq.toArray
if categories2.Length <> 8 || (categories2.[0] <> ("Beverages", 4.5M)) then printfn "categories2 failed"; exit 1

// Min - Elements
let categories3 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let min = g.Min(fun (p : Product) -> p.UnitPrice)
        let cheapestProducts = query { for x in g do where (x.UnitPrice = min) }
        select (g.Key, cheapestProducts)
    } |> Seq.toArray
let _,firstElement = categories3.[0]
if categories3.Length <> 8 || (firstElement.ToArray().[0].UnitsInStock <> 20) then printfn "categories3 failed"; exit 1

// Max - Simple
let maxNum = query { for n in numbers do maxBy n }
if maxNum <> 9 then printfn "maxNum failed"; exit 1

// Max - Projection
let longestLength = query { for w in words do maxBy w.Length }
if longestLength <> 9 then printfn "longestLength failed"; exit 1

// Max - Grouped
let categories4 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let mostExpensivePrice = query { for x in g do maxBy x.UnitPrice }
        select (g.Key, mostExpensivePrice)
    } |> Seq.toArray
if categories4.Length <> 8 || (categories4.[7] <> ("Grains/Cereals", 38.0M)) then printfn "categories4 failed"; exit 1

// Max - Elements
let categories5 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let maxPrice = query { for x in g do maxBy x.UnitPrice }
        let mostExpensiveProducts = query { for x in g do where (x.UnitPrice = maxPrice) }
        select (g.Key, mostExpensiveProducts)
    } |> Seq.toArray
let _,firstElement2 = categories5.[0]
if categories5.Length <> 8 || (firstElement2.ToArray().[0].UnitPrice <> 263.50M) then printfn "categories5 failed"; exit 1

// Average - Simple
let numbers2 = [5.0; 4.0; 1.0; 3.0; 9.0; 8.0; 6.0; 7.0; 2.0; 0.0]
let averageNum = query { for n in numbers2 do averageBy n }
if averageNum <> 4.5 then printfn "averageNum failed"; exit 1

// Average - Projection
let averageLength = 
    query { 
        for w in words do 
        let wl = w.Length |> float
        averageBy wl
    }
if (averageLength - 6.66666666) > 0.1 then printfn "averageLength failed"; exit 1

// Average - Grouped
let categories6 =
    query {
        for p in products do
        groupValBy p p.Category into g
        let averagePrice = query { for x in g do averageBy x.UnitPrice }
        select (g.Key, averagePrice)
    } |> Seq.toArray
if categories6.Length <> 8 || (categories6.[1] <> ("Condiments", 23.0625M)) then printfn "categories6 failed"; exit 1
