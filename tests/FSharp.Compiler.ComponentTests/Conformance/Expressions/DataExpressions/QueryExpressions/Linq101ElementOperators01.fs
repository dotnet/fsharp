// #Conformance #DataExpressions #Query
// GroupBy LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

// First - simple
let products = getProductList()

let products12 =
    query {
        for p in products do
        where (p.ProductID = 12)
        head
    }
if products12.ProductName <> "Queso Manchego La Pastora" then printfn "products12 failed"; exit 1

// First - Condition
let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]

let startsWithO =  
    query {
        for s in strings do
        where (s.[0] = 'o')
        head
    }
if startsWithO <> "one" then printfn "startsWithO failed"; exit 1

// FirstOrDefault - Simple
let numbers : int list = []
let firstNumOrDefault =
    query {
        for n in numbers do
        headOrDefault
    }
if firstNumOrDefault <> 0 then printfn "firstNumOrDefault failed"; exit 1

// ElementAt
let numbers2 = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

let fourthLowNum = 
    query {
        for n in numbers2 do
        where (n > 5)
        nth 1
    }
if fourthLowNum <> 8 then printfn "fourthLowNum failed"; exit 1