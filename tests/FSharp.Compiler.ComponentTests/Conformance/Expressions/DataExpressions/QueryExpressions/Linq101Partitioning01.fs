// #Conformance #DataExpressions #Query
// Partitioning LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

// Take - simple
let first3Numbers =
    query {
        for n in numbers do
        take 3
    } |> Seq.toList
if first3Numbers <> [5;4;1] then printfn "first3Numbers failed"; exit 1 

// Take - nested
let customers = getCustomerList()
let WAOrders =
    query {
        for c in customers do
        for o in c.Orders do
        where (c.Region = "WA")
        select (c.CustomerID, o.OrderID, o.OrderDate)
    } |> Seq.toArray
if WAOrders |> Seq.take 3 |> Seq.toArray <> WAOrders.[0..2]  then printfn "first3WAOrders failed"; exit 1

// Skip - simple
let allButFirst4Numbers = 
    query {
        for n in numbers do
        skip 4
    } |> Seq.toList
if allButFirst4Numbers <> [9; 8; 6; 7; 2; 0] then printfn "allbutFirst4Numbers failed"; exit 1

// Skip - Nested
let WAOrders2 =
    query {
        for c in customers do
        for o in c.Orders do
        where (c.Region = "WA")
        select (c.CustomerID, o.OrderID, o.OrderDate)
    } |> Seq.skip 2 |> Seq.toList
if WAOrders2.Count() <> 17 then printfn "WAOrders2 failed"; exit 1

// TakeWhile - simple
let firstNumbersLessThan6 =
    query {
        for n in numbers do
        takeWhile (n < 6)
    } |> Seq.toList
if firstNumbersLessThan6 <> [5;4;1;3] then printfn "firstNumbersLessThan6 failed"; exit 1

// SkipWhile - simple
let allButFirst3Numbers = 
    query {
        for n in numbers do
        skipWhile (n % 3 <> 0)
    } |> Seq.toList
if allButFirst3Numbers <> [3; 9; 8; 6; 7; 2; 0] then printfn "allButFirst3Numbers"; exit 1