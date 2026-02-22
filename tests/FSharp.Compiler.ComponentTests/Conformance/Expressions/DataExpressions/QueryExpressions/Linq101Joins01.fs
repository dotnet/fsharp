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
if q.Count() <> 46 || (q.[0] <> ("Beverages", "Chai")) then printfn "q failed"; exit 1

// Group Join
let q2 =
    query {
        for c in categories do
        groupJoin p in products on (c = p.Category) into ps
        select (c, ps)
    } |> Seq.toArray
let _,p = q2.[0]
if q2.Count() <> 5 || p.Count() <> 12 then printfn "q2 failed"; exit 1

// Cross Join with Group Join
let q3 =
    query {
        for c in categories do
        groupJoin p in products on (c = p.Category) into ps
        for p in ps do
        select (c, p.ProductName)
    } |> Seq.toArray
if q3.Count() <> 46 || q3.[0] <> ("Beverages", "Chai") then printfn "q3 failed"; exit 1

// Left Outer Join
let q4 =
    query {
        for c in categories do
        groupJoin p in products on (c = p.Category) into ps
        for p in ps.DefaultIfEmpty() do
        let t = if (box p = null) then "(No products)" else p.ProductName
        select (c, t)
    } |> Seq.toArray
if q4.Count() <> 47 || q4.[46] <> ("Seafood", "Röd Kaviar") then printfn "q4 failed"; exit 1