// #Conformance #DataExpressions #Query
// Where LINQ101 samples converted to query

open System
open System.Collections.Generic
open System.Linq


let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]
 
 // Where - Simple 1
let lowNums =
    query {
        for n in numbers do
        where (n < 5)
        select n;
    } |> List.ofSeq

if lowNums <> [4;1;3;2;0] then printfn "lowNums failed"; exit 1

// Where - Simple 2
let products = getProductList()

let soldOutProducts =
    query {
        for p in products do
        where (p.UnitsInStock = 0)
        select p
    }
if soldOutProducts.Count() <> 5 then printfn "soldOutProducts failed"; exit 1

// Where - Simple 3
let expensiveInStockProducts =
    query {
        for p in products do
        where (p.UnitsInStock > 0 && p.UnitPrice > 3.00M)
        select p
    }
if expensiveInStockProducts.Count() <> 71 then printfn "expensiveInStockProducts failed"; exit 1

// Where - Drilldown
let customers = getCustomerList()

let waCustomers =
    query {
        for c in customers do
        where (c.Region = "WA")
        select c
    } |> Seq.toArray
if waCustomers.Count() <> 3 then printfn "waCustomers.Count failed"; exit 1
if waCustomers.[0].Orders.Count() <> 2 then printfn "waCustomers.[0] failed"; exit 1
if waCustomers.[1].Orders.Count() <> 3 then printfn "waCustomers.[1] failed"; exit 1
if waCustomers.[2].Orders.Count() <> 14 then printfn "waCustomers.[2] failed"; exit 1

// Where - Indexed
let digits = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
let shortDigits =
    query {
        for d in digits do
        select d
    } 
    |> Seq.mapi (fun i d -> if d.Length < i then Some(d) else None) 
    |> Seq.choose id
if shortDigits.Count() <> 5 then printfn "shortDigits failed"; exit 1
