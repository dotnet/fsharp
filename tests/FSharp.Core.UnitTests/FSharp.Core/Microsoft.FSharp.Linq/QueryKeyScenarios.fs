// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.QueryKeyScenarios

open System.Linq
open System.Linq.Expressions
open Microsoft.FSharp.Linq.RuntimeHelpers

let private block = [ Violation.Statement ExpressionType.Block ]

/// A tree is pinned with every block it has: one that goes from three to two is a change as well
let private blocks count =
    List.replicate count (Violation.Statement ExpressionType.Block)

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

/// A record that is equal to itself only: two that are built alike are different.
[<ReferenceEquality>]
type PersonOnce = { NameOnce: string; AgeOnce: int }
let private rebuiltGroup =
    [ Violation.RuntimeHelperCall "RebuiltGroupings`2.Of" ]

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
      |> Scenario.violating (blocks 2)

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
      |> Scenario.violating (blocks 2)

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

      Scenario.rows
          "groupValBy a tuple of values, sum of one of them"
          (fun db -> query { for c in db.Customers do groupValBy (c.Name, c.Age) c.City into g; select (g.Key, g.Sum(fun (_, age) -> age)) })
          (customers >> Seq.groupBy _.City >> Seq.map (fun (city, group) -> city, group |> Seq.sumBy _.Age))

      Scenario.rows
          "groupValBy a tuple of values, average of one of them"
          (fun db -> query { for c in db.Customers do groupValBy (c.Name, c.Age) c.City into g; select (g.Key, g.Average(fun (_, age) -> float age)) })
          (customers >> Seq.groupBy _.City >> Seq.map (fun (city, group) -> city, group |> Seq.averageBy (fun c -> float c.Age)))

      Scenario.rows
          "groupValBy a tuple of values, max of one of them"
          (fun db -> query { for c in db.Customers do groupValBy (c.Name, c.Age) c.City into g; select (g.Key, g.Max(fun (_, age) -> age)) })
          (customers >> Seq.groupBy _.City >> Seq.map (fun (city, group) -> city, group |> Seq.map _.Age |> Seq.max))

      Scenario.rows
          "groupValBy a tuple of values, one of them selected then summed"
          (fun db -> query { for c in db.Customers do groupValBy (c.Name, c.Age) c.City into g; select (g.Key, g.Select(fun (_, age) -> age).Sum()) })
          (customers >> Seq.groupBy _.City >> Seq.map (fun (city, group) -> city, group |> Seq.sumBy _.Age))

      Scenario.rows
          "groupValBy a tuple of values, counted with a predicate"
          (fun db -> query { for c in db.Customers do groupValBy (c.Name, c.Age) c.City into g; select (g.Key, g.Count(fun (_, age) -> age > 26)) })
          (customers >> Seq.groupBy _.City >> Seq.map (fun (city, group) -> city, group |> Seq.filter (fun c -> c.Age > 26) |> Seq.length))

      Scenario.rows
          "groupBy after a join, key and count"
          (fun db -> query { for c in db.Customers do join o in db.Orders on (c.Id = o.CustomerId); groupBy c.City into g; select (g.Key, g.Count()) })
          (fun data -> seq { for c in data.Customers do for _ in c.Orders -> c.City } |> Seq.countBy id)

      Scenario.rows
          "groupBy after a join, sum over the joined side"
          (fun db ->
              query { for c in db.Customers do join o in db.Orders on (c.Id = o.CustomerId); groupBy c.City into g; select (g.Key, g.Sum(fun (_, o) -> o.Total)) })
          (fun data -> seq { for c in data.Customers do for o in c.Orders -> c.City, o.Total } |> Seq.groupBy fst |> Seq.map (fun (city, totals) -> city, Seq.sumBy snd totals))

      Scenario.rows
          "Select written in the query keeps its projection"
          (fun db ->
              let numbers = [| 10; 11; 12 |]

              query {
                  for c in db.Customers do
                  let age = c.Age
                  select (c.Name, age, numbers.Select(fun n -> n / 2).Distinct().Count())
              })
          (fun data -> seq { for c in data.Customers -> c.Name, c.Age, 2 })

      Scenario.rows
          "groups of groups counted against each other"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupValBy { PersonName = c.Name; PersonAge = c.Age } c.City into g
                  groupBy g.Key.Length into byLength

                  select
                      (byLength.Key,
                       byLength.Sum(fun older ->
                           byLength.Sum(fun younger -> younger.Sum(fun y -> older.Count(fun o -> o.PersonAge > y.PersonAge)))))
              })
          (fun data ->
              data.Customers
              |> Seq.groupBy _.City
              |> Seq.groupBy (fun (city, _) -> city.Length)
              |> Seq.map (fun (length, cities) ->
                  let ages = [ for _, people in cities -> [ for p in people -> p.Age ] ]

                  length,
                  List.sum
                      [ for older in ages do
                            for younger in ages do
                                for y in younger -> older |> List.filter (fun o -> o > y) |> List.length ]))

      Scenario.rows
          "groups after a join as the result"
          (fun db -> query { for c in db.Customers do join o in db.Orders on (c.Id = o.CustomerId); groupBy c.City })
          (fun data ->
              seq { for c in data.Customers do for o in c.Orders -> c, o }
              |> Seq.groupBy (fun (c, _) -> c.City)
              |> Seq.map (fun (city, group) -> Grouping(city, group) :> IGrouping<_, _>))
      |> Scenario.violating [ Violation.RuntimeHelperCall "new Grouping`2" ]

      // A group the query uses as a value is one object, however often it is mentioned or gone through
      Scenario.rows
          "groupValBy a tuple of values, the group compared with itself"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupValBy (c.Name, c.Age) c.City into g
                  where (g.Equals g)
                  select (g.Key, obj.ReferenceEquals(g, g))
              })
          (customers >> Seq.map _.City >> Seq.distinct >> Seq.map (fun city -> city, true))
      |> Scenario.violating rebuiltGroup

      Scenario.rows
          "groups of groups, a group found among the groups it is in"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupValBy (c.Name, c.Age) c.City into g
                  groupBy g.Key.Length into byLength
                  select (byLength.Key, byLength.Contains(byLength.First()), obj.ReferenceEquals(byLength.First(), byLength.First()))
              })
          (customers >> Seq.map _.City.Length >> Seq.distinct >> Seq.map (fun length -> length, true, true))
      |> Scenario.violating rebuiltGroup

      Scenario.rows
          "groupValBy a tuple of values, the group as a sequence is still the group"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupValBy (c.Name, c.Age) c.City into g
                  select (g.Key, ((g.AsEnumerable() :> obj) :? IGrouping<string, string * int>))
              })
          (customers >> Seq.map _.City >> Seq.distinct >> Seq.map (fun city -> city, true))
      |> Scenario.violating rebuiltGroup

      Scenario.rows
          "groupBy a group, the group found among the elements it is the key of"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupValBy (c.Name, c.Age) c.City into g
                  groupBy g into byGroup
                  select (byGroup.Key.Key, byGroup.Contains(byGroup.Key))
              })
          (customers >> Seq.map _.City >> Seq.distinct >> Seq.map (fun city -> city, true))
      |> Scenario.violating rebuiltGroup

      // An element that a lambda over a group uses as a value of its own is one object in that lambda
      Scenario.rows
          "groupValBy records compared by reference, counted where an element is itself"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupValBy { NameOnce = c.Name; AgeOnce = c.Age } c.City into g
                  select (g.Key, g.Count(fun p -> p = p))
              })
          (customers >> Seq.countBy _.City)

      Scenario.rows
          "find among the groups of a nested query"
          (fun db ->
              query {
                  for c in db.Customers do
                  where (c.Orders.Count > 0)
                  select (c.Name, (query { for o in c.Orders do groupValBy (o.Id, o.Total) (o.Id % 2) into g; find (g.Count() > 0) }).Key)
              })
          (customers >> Seq.filter (fun c -> c.Orders.Count > 0) >> Seq.map (fun c -> c.Name, (Seq.head c.Orders).Id % 2))
      |> Scenario.violating [ Violation.RuntimeHelperCall "new Grouping`2" ]
      // A member that a Select of the query's own reads can fail, so that Select runs where it is written
      Scenario.rows
          "Select written in the query over a member that cannot always be read"
          (fun db ->
              query {
                  for o in db.Orders do
                  let next = o.Id + 1
                  select (o.Id, next, o.Lines.Select(fun l -> l.Discount.Value).Count())
              })
          (fun _ -> Seq.empty<int * int * int>)
      |> Scenario.violating [ Violation.Throws "InvalidOperationException" ] ]

let cases () = joins () @ groupings ()
