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

// Select - Simple 2
let products = getProductList()

let productNames =
    query {
        for p in products do
        select (p.ProductName)
    }

// Select - Transformation
let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
let textNums =
    query {
        for n in numbers do
        select (strings.[n])
    } |> Seq.toList

// Select - Anonymous Types 1
let words = ["aPPLE"; "BlUeBeRrY"; "cHeRry" ]

let upperLowerWords =
   query {
       for w in words do
       select (w.ToUpper(), w.ToLower())
   } |> Seq.toArray

// Select - Anonymous Types 2
let digitOddEvens =
    query {
        for n in numbers do
        select (strings.[n], (n % 2) = 0)
    } |> Seq.toList

// Select - Anonymous Types 3
let productInfos =
    query {
        for p in products do
        select (p.ProductName, p.Category, p.UnitPrice)
    } |> Seq.toArray

// Select - Filtered
let digits = strings
let lowNums = 
    query {
        for n in numbers do
        where (n < 5)
        select digits.[n]
    } |> Seq.toList
if lowNums <> ["four"; "one"; "three"; "two"; "zero"] then printfn "lowNums failed"; raise (new Exception("exit 1"))

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

// SelectMany - Compound from 2
let customers = getCustomerList()
let orders =
    query {
        for c in customers do
        for o in c.Orders do
        where (o.Total < 500.00M)
        select (c.CustomerID, o.OrderID, o.Total)
    } |> Seq.toArray

// SelectMany - compound from 3
let orders2 =
    query {
        for c in customers do
        for o in c.Orders do
        where (o.OrderDate >= DateTime(1998, 1,1))
        select (c.CustomerID, o.OrderID, o.OrderDate)
    } |> Seq.toArray

// SelectMany - from Assignment
let orders3 =
    query {
        for c in customers do
        for o in c.Orders do
        where (o.Total >= 2000.0M)
        select (c.CustomerID, o.OrderID, o.Total)
    }

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