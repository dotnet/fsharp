// #Conformance #DataExpressions #Query
// Join LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

// Cross Join
let categories = ["Beverages"; "Condiments"; "Vegetables"; "Dairy Products"; "Seafood"]
let products = getProductList()

let q =
    query {
        for c in categories do
        join p in products on (c = p.Category)
        select (c, p.ProductName)
    } |> Seq.toArray

// Group Join
let q2 =
    query {
        for c in categories do
        groupJoin p in products on (c = p.Category) into ps
        select (c, ps)
    } |> Seq.toArray

// Cross Join with Group Join
let q3 =
    query {
        for c in categories do
        groupJoin p in products on (c = p.Category) into ps
        for p in ps do
        select (c, p.ProductName)
    } |> Seq.toArray

// Left Outer Join
let q4 =
    query {
        for c in categories do
        groupJoin p in products on (c = p.Category) into ps
        for p in ps.DefaultIfEmpty() do
        let t = if (box p = null) then "(No products)" else p.ProductName
        select (c, t)
    } |> Seq.toArray