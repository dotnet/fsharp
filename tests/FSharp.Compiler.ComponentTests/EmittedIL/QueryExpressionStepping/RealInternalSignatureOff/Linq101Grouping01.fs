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

// GroupBy - Simple 2
let words = ["blueberry"; "chimpanzee"; "abacus"; "banana"; "apple"; "cheese" ]

let wordGroups =
    query {
        for w in words do
        groupValBy w (w.[0]) into g
        select (g.Key, g.ToArray())
    } |> Seq.toArray

// GroupBy - Simple 3
let products = getProductList()

let orderGroups =
    query {
        for p in products do
        groupValBy p p.Category into g
        select (g.Key, g.ToArray())
    } |> Seq.toArray

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