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

// Take - nested
let customers = getCustomerList()
let WAOrders =
    query {
        for c in customers do
        for o in c.Orders do
        where (c.Region = "WA")
        select (c.CustomerID, o.OrderID, o.OrderDate)
    } |> Seq.toArray

// Skip - simple
let allButFirst4Numbers = 
    query {
        for n in numbers do
        skip 4
    } |> Seq.toList

// Skip - Nested
let WAOrders2 =
    query {
        for c in customers do
        for o in c.Orders do
        where (c.Region = "WA")
        select (c.CustomerID, o.OrderID, o.OrderDate)
    } |> Seq.skip 2 |> Seq.toList

// TakeWhile - simple
let firstNumbersLessThan6 =
    query {
        for n in numbers do
        takeWhile (n < 6)
    } |> Seq.toList

// SkipWhile - simple
let allButFirst3Numbers = 
    query {
        for n in numbers do
        skipWhile (n % 3 <> 0)
    } |> Seq.toList