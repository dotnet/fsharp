// #Conformance #DataExpressions #Query
// Set Opeartors LINQ101 samples converted to query

open System
open System.Collections.Generic
open System.Linq

// Distinct - 1
let factorsOf300 = [2;2;3;5;5]

let uniqueFactors = 
    query {
        for n in factorsOf300 do
        distinct
    } |> Seq.toList

// Distinct - 2
let products = getProductList()

let categoryNames =
    query {
        for p in products do
        select p.Category
        distinct
    } |> Seq.toList

// Union - 2
let customers = getCustomerList()

let productFirstChars = 
    query {
        for p in products do
        select p.ProductName.[0]
    }

let customerFirstChars =
    query {
        for c in customers do
        select c.CompanyName.[0]
    }