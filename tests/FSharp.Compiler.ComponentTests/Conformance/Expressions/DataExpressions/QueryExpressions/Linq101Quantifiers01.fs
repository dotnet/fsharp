// #Conformance #DataExpressions #Query
// Quantifier LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

// Any - Simple
let words = ["believe"; "relief"; "receipt"; "field"]

let iAfterE =
    query {
        for w in words do
        exists (w.Contains("ei"))
    }
if not iAfterE then printfn "iAfterE failed"; exit 1

// Any - Grouped
let products = getProductList()

let productGroups =
    query {
        for p in products do
        groupValBy p p.Category into g
        where (g.Any(fun x -> x.UnitsInStock = 0))
        select (g.Key, g)
    } |> Seq.toArray
let _,firstElement = productGroups.[0]
if productGroups.Length <> 3 || (firstElement.ToArray().[0].ProductName <> "Aniseed Syrup") then printfn "productGroups failed"; exit 1

// All - simple
let numbers = [1;11;3;19;41;65;19]

let onlyOdd =
    query {
        for n in numbers do
        all (n % 2 = 1)
    }
if not onlyOdd then printfn "onlyOdd failed"; exit 1

// All - Grouped
let productGroups2 =
    query {
        for p in products do
        groupValBy p p.Category into g
        where (g.All(fun x -> x.UnitsInStock > 0))
        select (g.Key, g)
    } |> Seq.toArray
let _,firstElement2 = productGroups2.[0]
if productGroups2.Length <> 5 || (firstElement2.ToArray().[0].ProductName <> "Chai") then printfn "productGroups2 failed"; exit 1
