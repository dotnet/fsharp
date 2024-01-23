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

// OrderBy - Simple 2
let sortedWords2 =
    query {
        for w in words do 
        sortBy (w.Length)
    } |> Seq.toList

// OrderBy - Simple 3
let products = getProductList()
let sortedProducts =
    query {
        for p in products do
        sortBy p.ProductName
        select p
    } |> Seq.toArray

// OrderByDescending - Simple 1
// Dev11:179653
//let doubles = [1.7M, 2.3M, 1.9M, 4.1M, 2.9M]
//let sortedDoubles =
//    query {
//        for d in doubles do
//        sortByDescending d
//    } |> Seq.toArray
//if sortedDoubles <>

// OrderByDescending - Simple 2
let sortedProducts2 =
    query {
        for p in products do
        sortByDescending p.UnitsInStock
    } |> Seq.toArray

// ThenBy - Simple
let digits = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
let sortedDigits =
    query {
        for d in digits do
        sortBy d.Length
        thenBy d
    } |> Seq.toList

// ThenByDescending - Simple
let sortedProducts3 =
    query {
        for p in products do
        sortBy p.Category
        thenByDescending p.UnitPrice
        select p
    } |> Seq.toArray