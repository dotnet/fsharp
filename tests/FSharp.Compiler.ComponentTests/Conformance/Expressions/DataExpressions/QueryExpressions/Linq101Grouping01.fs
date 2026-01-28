// #Conformance #DataExpressions #Query
// GroupBy LINQ101 samples converted to query
open System
open System.Collections.Generic
open System.Linq

let digits = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]

// GroupBy - Simple 1
let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

let numberGroups =
    query {       
        for n in numbers do
        groupValBy n (n % 5) into g
        select (g.Key, g.ToArray())
    } |> Seq.toArray
if numberGroups.[0] <> (0,[|5;0|]) then printfn "numberGroups failed"; exit 1

// GroupBy - Simple 2
let words = ["blueberry"; "chimpanzee"; "abacus"; "banana"; "apple"; "cheese" ]

let wordGroups =
    query {
        for w in words do
        groupValBy w (w.[0]) into g
        select (g.Key, g.ToArray())
    } |> Seq.toArray
if wordGroups.Count() <> 3 || wordGroups.[0] <> ('b', [|"blueberry"; "banana"|]) then printfn "wordGroups failed"; exit 1

// GroupBy - Simple 3
let products = getProductList()

let orderGroups =
    query {
        for p in products do
        groupValBy p p.Category into g
        select (g.Key, g.ToArray())
    } |> Seq.toArray
let _,firstElement = orderGroups.[0]
if orderGroups.Length <> 8 || firstElement.Length <> 12 then printfn "orderGroups failed"; exit 1

// GroupBy - Nested
let customers = getCustomerList()

let customerOrderGroups =
    query {
        for c in customers do
        let yearGroups =
            query {
                for o in c.Orders do
                groupValBy o (o.OrderDate.Year) into yg
                let monthGroups =
                    query {
                        for o in yg do
                        groupValBy o (o.OrderDate.Month) into mg
                        select (mg.Key, mg.ToArray())
                    }
                select (yg.Key, monthGroups.ToArray())
            }
        select (c.CompanyName, yearGroups.ToArray())
    } |> Seq.toArray
let _,firstGroup = customerOrderGroups.[0]
let _, monthGroup = firstGroup.[0]
let month,order = monthGroup.[0]
if customerOrderGroups.Length <> 91 || firstGroup.Length <> 2 || (order.[0].OrderID <> 10643) then printfn "customerOrderGroups failed"; exit 1