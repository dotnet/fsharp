// #Conformance #DataExpressions #Query
// Ordering LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

// OrderBy - Simple 1
let words = ["cherry"; "apple"; "blueberry"]
let sortedWords =
    query {
        for w in words do
        sortBy w
    } |> Seq.toList
if sortedWords <> ["apple"; "blueberry"; "cherry"] then printfn "sortedWords failed"; exit 1

// OrderBy - Simple 2
let sortedWords2 =
    query {
        for w in words do 
        sortBy (w.Length)
    } |> Seq.toList
if sortedWords2 <> ["apple"; "cherry"; "blueberry"] then printfn "sortedWords2 failed"; exit 1

// OrderBy - Simple 3
let products = getProductList()
let sortedProducts =
    query {
        for p in products do
        sortBy p.ProductName
        select p
    } |> Seq.toArray
if sortedProducts.Count() <> 77 || sortedProducts.[0].ProductName <> "Alice Mutton" then printfn "sortedProducts failed"; exit 1

// OrderByDescending - Simple 1
let doubles = [1.7M, 2.3M, 1.9M, 4.1M, 2.9M]
let sortedDoubles =
    query {
        for d in doubles do
        sortByDescending d
    } |> Seq.toArray
if sortedDoubles <> [|(1.7M, 2.3M, 1.9M, 4.1M, 2.9M)|] then printfn "sortedDoubles failed"; exit 1

// OrderByDescending - Simple 2
let sortedProducts2 =
    query {
        for p in products do
        sortByDescending p.UnitsInStock
    } |> Seq.toArray
if sortedProducts2.[0].ProductName <> "Rhönbräu Klosterbier" then printfn "sortedProducts2 failed"; exit 1

// ThenBy - Simple
let digits = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
let sortedDigits =
    query {
        for d in digits do
        sortBy d.Length
        thenBy d
    } |> Seq.toList
if sortedDigits <> ["one"; "six"; "two"; "five"; "four"; "nine"; "zero"; "eight"; "seven"; "three"] then printfn "sortedDigits failed"; exit 1

// ThenByDescending - Simple
let sortedProducts3 =
    query {
        for p in products do
        sortBy p.Category
        thenByDescending p.UnitPrice
        select p
    } |> Seq.toArray
if sortedProducts3.[0].ProductName <> "Côte de Blaye" then printfn "sortedProducts3 failed"; exit 1