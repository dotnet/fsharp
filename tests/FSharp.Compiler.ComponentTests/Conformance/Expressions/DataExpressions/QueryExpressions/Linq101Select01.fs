// #Conformance #DataExpressions #Query
// Select LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

// Select - Simple 1 
let numsPlusOne =
    query {
        for n in numbers do
        select (n + 1)
    } |> Seq.toList
if numsPlusOne <> [ 6; 5; 2; 4; 10; 9; 7; 8; 3; 1 ] then printfn "numsPlusOne failed"; exit 1

// Select - Simple 2
let products = getProductList()

let productNames =
    query {
        for p in products do
        select (p.ProductName)
    }
if productNames.Count() <> 77 then printfn "productNames failed"; exit 1

// Select - Transformation
let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
let textNums =
    query {
        for n in numbers do
        select (strings.[n])
    } |> Seq.toList
if textNums <> [ "five"; "four"; "one"; "three"; "nine"; "eight"; "six"; "seven"; "two"; "zero" ] then printfn "textNums failed"; exit 1


// Select - Anonymous Types 1
let words = ["aPPLE"; "BlUeBeRrY"; "cHeRry" ]

let upperLowerWords =
   query {
       for w in words do
       select (w.ToUpper(), w.ToLower())
   } |> Seq.toArray
if upperLowerWords.Count() <> 3 || upperLowerWords.[0] <> ("APPLE", "apple") then printfn "upperLowerWords failed"; exit 1

// Select - Anonymous Types 2
let digitOddEvens =
    query {
        for n in numbers do
        select (strings.[n], (n % 2) = 0)
    } |> Seq.toList
if digitOddEvens |> List.filter (fun (_,even) -> even) |> Seq.length <> 5 then printfn "digitOddEvens failed"; exit 1

// Select - Anonymous Types 3
let productInfos =
    query {
        for p in products do
        select (p.ProductName, p.Category, p.UnitPrice)
    } |> Seq.toArray
if productInfos.[0] <> ("Chai", "Beverages", 18.0000M) then printfn "productInfos failed"; exit 1

// Select - Filtered
let digits = strings
let lowNums = 
    query {
        for n in numbers do
        where (n < 5)
        select digits.[n]
    } |> Seq.toList
if lowNums <> ["four"; "one"; "three"; "two"; "zero"] then printfn "lowNums failed"; exit 1

// SelectMany - Compound from 1
let numbersA = [0; 2; 4; 5; 6; 8; 9]
let numbersB = [1; 3; 5; 7; 8]

let pairs =
    query {
        for a in numbersA do
        for b in numbersB do
        where (a < b)
        select (a,b)
    } |> Seq.toArray
if pairs.Count() <> 16 || pairs.[0] <> (0,1) then printfn "pairs failed"; exit 1

// SelectMany - Compound from 2
let customers = getCustomerList()
let orders =
    query {
        for c in customers do
        for o in c.Orders do
        where (o.Total < 500.00M)
        select (c.CustomerID, o.OrderID, o.Total)
    } |> Seq.toArray
if orders.Count() <> 234 || orders.[0] <> ("ALFKI", 10702, 330.00M) then printfn "orders failed"; exit 1

// SelectMany - compound from 3
let orders2 =
    query {
        for c in customers do
        for o in c.Orders do
        where (o.OrderDate >= DateTime(1998, 1,1))
        select (c.CustomerID, o.OrderID, o.OrderDate)
    } |> Seq.toArray
if orders2.Count() <> 270 || orders2.[0] <> ("ALFKI", 10835, DateTime(1998,1,15)) then printfn "orders2 failed"; exit 1

// SelectMany - from Assignment
let orders3 =
    query {
        for c in customers do
        for o in c.Orders do
        where (o.Total >= 2000.0M)
        select (c.CustomerID, o.OrderID, o.Total)
    }
if orders3.Count() <> 185 then printfn "orders3 failed"; exit 1

// SelectMany - Multiple from
let cutOffDate = DateTime(1997, 1, 1)

let orders4 =
    query {
        for c in customers do
        where (c.Region = "WA")
        for o in c.Orders do
        where (o.OrderDate >= cutOffDate)
        select (c.CustomerID, o.OrderID)
    }
if orders4.Count() <> 17 then printfn "orders4 failed"; exit 1