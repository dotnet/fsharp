// #Conformance #DataExpressions #Query
// Set Operators LINQ101 samples converted to query

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
if uniqueFactors <> [2;3;5] then printfn "uniqueFactors failed"; exit 1

// Distinct - 2
let products = getProductList()

let categoryNames =
    query {
        for p in products do
        select p.Category
        distinct
    } |> Seq.toList
if categoryNames <> ["Beverages"; "Condiments"; "Produce"; "Meat/Poultry"; "Seafood"; "Dairy Products"; "Confections"; "Grains/Cereals"] then printfn "categoryNames failed"; exit 1

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

let uniqueFirstChars = productFirstChars.Union(customerFirstChars) |> Seq.toArray
if uniqueFirstChars <> [|'C'; 'A'; 'G'; 'U'; 'N'; 'M'; 'I'; 'Q'; 'K'; 'T'; 'P'; 'S'; 'R'; 'B'; 'J';'Z'; 'V'; 'F'; 'E'; 'W'; 'L'; 'O'; 'D'; 'H'|] then printfn "uniqueFirstChars failed"; exit 1

// Intersect - 2
let commonFirstChars = productFirstChars.Intersect(customerFirstChars) |> Seq.toArray
if commonFirstChars <> [|'C'; 'A'; 'G'; 'N'; 'M'; 'I'; 'Q'; 'K'; 'T'; 'P'; 'S'; 'R'; 'B'; 'V'; 'F';'E'; 'W'; 'L'; 'O'|] then printfn "commonFirstChars failed"; exit 1

// Except - 2
let productOnlyFirstChars = productFirstChars.Except(customerFirstChars) |> Seq.toList
if productOnlyFirstChars <> ['U'; 'J'; 'Z'] then printfn "productOnlyFirstChars failed"; exit 1