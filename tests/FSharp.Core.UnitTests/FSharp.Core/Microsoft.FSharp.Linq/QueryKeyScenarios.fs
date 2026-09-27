// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.QueryKeyScenarios

open System.Linq
open System.Linq.Expressions
open Microsoft.FSharp.Linq.RuntimeHelpers

let private block = [ Violation.Statement ExpressionType.Block ]

let private customers (data: Data) = Seq.ofList data.Customers

/// Orders paired with the lines that share both the order number and, as quantity, the customer number.
let private matchingLines (data: Data) =
    seq {
        for o in data.Orders do
            for l in data.Lines do
                if l.OrderId = o.Id && l.Quantity = o.CustomerId then
                    yield o.Id, l.Id
    }

let private matchingLinesOrNone (data: Data) =
    seq {
        for o in data.Orders do
            match matchingLines data |> Seq.filter (fun (order, _) -> order = o.Id) |> List.ofSeq with
            | [] -> yield o.Id, -1
            | matches -> yield! matches
    }

let private joins () =
    [ Scenario.rows
          "join on a scalar"
          (fun db -> query { for c in db.Customers do join o in db.Orders on (c.Id = o.CustomerId); select (c.Name, o.Id) })
          (fun data -> seq { for c in data.Customers do for o in c.Orders -> c.Name, o.Id })

      Scenario.rows
          "join on a tuple"
          (fun db ->
              query { for o in db.Orders do join l in db.Lines on ((o.Id, o.CustomerId) = (l.OrderId, l.Quantity)); select (o.Id, l.Id) })
          matchingLines

      Scenario.rows
          "join on a struct tuple"
          (fun db ->
              query {
                  for o in db.Orders do
                  join l in db.Lines on (struct (o.Id, o.CustomerId) = struct (l.OrderId, l.Quantity))
                  select (o.Id, l.Id)
              })
          matchingLines

      Scenario.rows
          "join on a record, fields in declaration order"
          (fun db ->
              query {
                  for o in db.Orders do
                  join l in db.Lines on ({ OrderNumber = o.Id; Units = o.CustomerId } = { OrderNumber = l.OrderId; Units = l.Quantity })
                  select (o.Id, l.Id)
              })
          matchingLines

      Scenario.rows
          "join on a record, fields out of declaration order"
          (fun db ->
              query {
                  for o in db.Orders do
                  join l in db.Lines on ({ Units = o.CustomerId; OrderNumber = o.Id } = { Units = l.Quantity; OrderNumber = l.OrderId })
                  select (o.Id, l.Id)
              })
          matchingLines
      |> Scenario.violating block

      Scenario.rows
          "join on an anonymous record, fields in alphabetical order"
          (fun db ->
              query {
                  for o in db.Orders do
                  join l in db.Lines on ({| Order = o.Id; Units = o.CustomerId |} = {| Order = l.OrderId; Units = l.Quantity |})
                  select (o.Id, l.Id)
              })
          matchingLines

      Scenario.rows
          "join on an anonymous record, fields out of alphabetical order"
          (fun db ->
              query {
                  for o in db.Orders do
                  join l in db.Lines on ({| Units = o.CustomerId; Order = o.Id |} = {| Units = l.Quantity; Order = l.OrderId |})
                  select (o.Id, l.Id)
              })
          matchingLines
      |> Scenario.violating block

      Scenario.rows
          "groupJoin on a tuple"
          (fun db ->
              query {
                  for o in db.Orders do
                  groupJoin l in db.Lines on ((o.Id, o.CustomerId) = (l.OrderId, l.Quantity)) into lines
                  select (o.Id, lines.Count())
              })
          (fun data -> seq { for o in data.Orders -> o.Id, matchingLines data |> Seq.filter (fun (order, _) -> order = o.Id) |> Seq.length })

      Scenario.rows
          "leftOuterJoin on a tuple"
          (fun db ->
              query {
                  for o in db.Orders do
                  leftOuterJoin l in db.Lines on ((o.Id, o.CustomerId) = (l.OrderId, l.Quantity)) into lines
                  for l in lines do
                  select (o.Id, (if obj.ReferenceEquals(l, null) then -1 else l.Id))
              })
          matchingLinesOrNone ]

let private groupings () =
    [ Scenario.rows
          "groupBy a scalar"
          (fun db -> query { for c in db.Customers do groupBy c.City into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy _.City)

      Scenario.rows
          "groupBy a scalar, groups themselves"
          (fun db -> query { for c in db.Customers do groupBy c.City })
          (customers >> Seq.groupBy _.City >> Seq.map (fun (city, group) -> Grouping(city, group) :> IGrouping<_, _>))

      Scenario.rows
          "groupBy a tuple"
          (fun db -> query { for c in db.Customers do groupBy (c.City, c.Age > 28) into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy (fun c -> c.City, c.Age > 28))

      Scenario.rows
          "groupBy a tuple, element of the key"
          (fun db -> query { for c in db.Customers do groupBy (c.City, c.Age > 28) into g; select (fst g.Key, g.Count()) })
          (customers >> Seq.countBy (fun c -> c.City, c.Age > 28) >> Seq.map (fun ((city, _), count) -> city, count))
      |> Scenario.violating [ Violation.RuntimeHelperCall "Operators.Fst" ]

      Scenario.rows
          "groupBy a struct tuple"
          (fun db -> query { for c in db.Customers do groupBy (struct (c.City, c.Age > 28)) into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy (fun c -> struct (c.City, c.Age > 28)))

      Scenario.rows
          "groupBy a record, fields in declaration order"
          (fun db -> query { for c in db.Customers do groupBy { KeyCity = c.City; KeyAge = c.Age / 10 } into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy (fun c -> { KeyCity = c.City; KeyAge = c.Age / 10 }))

      Scenario.rows
          "groupBy a record, fields out of declaration order"
          (fun db -> query { for c in db.Customers do groupBy { KeyAge = c.Age / 10; KeyCity = c.City } into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy (fun c -> { KeyCity = c.City; KeyAge = c.Age / 10 }))
      |> Scenario.violating block

      Scenario.rows
          "groupBy an anonymous record, fields in alphabetical order"
          (fun db -> query { for c in db.Customers do groupBy {| City = c.City; Decade = c.Age / 10 |} into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy (fun c -> {| City = c.City; Decade = c.Age / 10 |}))

      Scenario.rows
          "groupBy an anonymous record, fields out of alphabetical order"
          (fun db -> query { for c in db.Customers do groupBy {| Decade = c.Age / 10; City = c.City |} into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy (fun c -> {| City = c.City; Decade = c.Age / 10 |}))
      |> Scenario.violating block

      Scenario.rows
          "groupValBy a tuple of values"
          (fun db -> query { for c in db.Customers do groupValBy (c.Name, c.Age) c.City into g; select (g.Key, g.Count()) })
          (customers >> Seq.countBy _.City)
      |> Scenario.violating [ Violation.RuntimeHelperCall "new Grouping`2" ] ]

let cases () = joins () @ groupings ()
