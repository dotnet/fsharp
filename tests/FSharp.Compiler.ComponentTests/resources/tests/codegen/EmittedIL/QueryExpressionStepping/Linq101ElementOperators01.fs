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

// First - Condition
let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]

let startsWithO =  
    query {
        for s in strings do
        where (s.[0] = 'o')
        head
    }

// FirstOrDefault - Simple
let numbers : int list = []
let firstNumOrDefault =
    query {
        for n in numbers do
        headOrDefault
    }

// FirstOrDefault - Condition
// TODO: DevDiv: 184318
//let products = getProductList()
//
//let product789 =
//    query{
//        for p in products do
//        where (p.ProductID = 789)
//        headOrDefault
//    }
//if product789 <> null then printfn "product789 failed"; raise (new Exception("exit 1"))

// ElementAt
let numbers2 = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

let fourthLowNum = 
    query {
        for n in numbers2 do
        where (n > 5)
        nth 1
    }