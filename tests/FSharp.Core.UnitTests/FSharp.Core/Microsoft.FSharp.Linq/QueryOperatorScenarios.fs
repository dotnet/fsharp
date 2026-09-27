// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.QueryOperatorScenarios

open System
open System.Linq
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Linq.NullableOperators

let private customers (data: Data) = Seq.ofList data.Customers

let private lines (data: Data) = Seq.ofList data.Lines

let private discountOrLowest (line: OrderLine) =
    if line.Discount.HasValue then line.Discount.Value else Int32.MinValue

let private discounts (data: Data) =
    data.Lines |> List.filter _.Discount.HasValue |> List.map _.Discount.Value

let private sequences () =
    [ Scenario.rows
          "where"
          (fun db -> query { for c in db.Customers do where (c.Age > 26); select c.Name })
          (customers >> Seq.filter (fun c -> c.Age > 26) >> Seq.map _.Name)

      Scenario.rows
          "where with string methods"
          (fun db -> query { for c in db.Customers do where (c.Name.StartsWith "A" || c.City = "Paris"); select c.Name })
          (customers >> Seq.filter (fun c -> c.Name.StartsWith "A" || c.City = "Paris") >> Seq.map _.Name)

      Scenario.rows
          "where on a nullable property"
          (fun db -> query { for c in db.Customers do where (c.Registered.HasValue && c.Registered.Value.Year > 2020); select c.Name })
          (customers >> Seq.filter (fun c -> c.Registered.HasValue && c.Registered.Value.Year > 2020) >> Seq.map _.Name)

      Scenario.rows
          "where with a nullable operator"
          (fun db -> query { for l in db.Lines do where (l.Discount ?> 1); select l.Id })
          (lines >> Seq.filter (fun l -> l.Discount ?> 1) >> Seq.map _.Id)

      Scenario.rows
          "if then"
          (fun db -> query { for c in db.Customers do if c.Age > 26 then yield c.Name })
          (customers >> Seq.filter (fun c -> c.Age > 26) >> Seq.map _.Name)

      Scenario.rows
          "sortBy, thenByDescending"
          (fun db -> query { for c in db.Customers do sortBy c.City; thenByDescending c.Age; select c.Name })
          (customers >> Seq.sortBy (fun c -> c.City, -c.Age) >> Seq.map _.Name)
      |> Scenario.ordered
      |> Scenario.flat [ "OrderBy"; "ThenByDescending"; "Select" ]

      Scenario.rows
          "sortByDescending, thenBy"
          (fun db -> query { for c in db.Customers do sortByDescending c.City; thenBy c.Age; select c.Name })
          (customers >> Seq.sortBy _.Age >> Seq.sortByDescending _.City >> Seq.map _.Name)
      |> Scenario.ordered
      |> Scenario.flat [ "OrderByDescending"; "ThenBy"; "Select" ]

      Scenario.rows
          "sortByNullable, thenByNullableDescending"
          (fun db -> query { for l in db.Lines do sortByNullable l.Discount; thenByNullableDescending (Nullable l.Id); select l.Id })
          (lines >> Seq.sortBy (fun l -> discountOrLowest l, -l.Id) >> Seq.map _.Id)
      |> Scenario.ordered
      |> Scenario.flat [ "OrderBy"; "ThenByDescending"; "Select" ]

      Scenario.rows
          "sortByNullableDescending, thenByNullable"
          (fun db -> query { for l in db.Lines do sortByNullableDescending l.Discount; thenByNullable (Nullable l.Id); select l.Id })
          (lines >> Seq.sortBy (fun l -> -(int64 (discountOrLowest l)), l.Id) >> Seq.map _.Id)
      |> Scenario.ordered
      |> Scenario.flat [ "OrderByDescending"; "ThenBy"; "Select" ]

      Scenario.rows "distinct" (fun db -> query { for c in db.Customers do select c.City; distinct }) (customers >> Seq.map _.City >> Seq.distinct)

      Scenario.rows
          "skip, take"
          (fun db -> query { for c in db.Customers do sortBy c.Id; skip 1; take 2; select c.Name })
          (customers >> Seq.skip 1 >> Seq.take 2 >> Seq.map _.Name)
      |> Scenario.ordered

      Scenario.rows
          "skipWhile, takeWhile"
          (fun db -> query { for c in db.Customers do sortBy c.Id; skipWhile (c.Id < 2); takeWhile (c.Id < 4); select c.Name })
          (customers >> Seq.skipWhile (fun c -> c.Id < 2) >> Seq.takeWhile (fun c -> c.Id < 4) >> Seq.map _.Name)
      |> Scenario.ordered

      Scenario.rows
          "join"
          (fun db -> query { for o in db.Orders do join l in db.Lines on (o.Id = l.OrderId); select (o.Id, l.Id) })
          (fun data -> seq { for o in data.Orders do for l in o.Lines -> o.Id, l.Id })

      Scenario.rows
          "groupJoin"
          (fun db -> query { for o in db.Orders do groupJoin l in db.Lines on (o.Id = l.OrderId) into lines; select (o.Id, lines.Count()) })
          (fun data -> seq { for o in data.Orders -> o.Id, o.Lines.Count })

      Scenario.rows
          "leftOuterJoin"
          (fun db ->
              query {
                  for o in db.Orders do
                  leftOuterJoin l in db.Lines on (o.Id = l.OrderId) into lines
                  for l in lines do
                  select (o.Id, (if obj.ReferenceEquals(l, null) then -1 else l.Id))
              })
          (fun data ->
              seq {
                  for o in data.Orders do
                      if o.Lines.Count = 0 then yield o.Id, -1 else for l in o.Lines -> o.Id, l.Id
              })

      Scenario.rows
          "leftOuterJoin tested with isNull"
          (fun db ->
              query {
                  for o in db.Orders do
                  leftOuterJoin l in db.Lines on (o.Id = l.OrderId) into lines
                  for l in lines do
                  select (o.Id, isNull l)
              })
          (fun data ->
              seq {
                  for o in data.Orders do
                      if o.Lines.Count = 0 then yield o.Id, true else for _ in o.Lines -> o.Id, false
              }) ]

let private values () =
    [ Scenario.value "count" (fun db -> query { for c in db.Customers do where (c.Age > 26); count }) (fun _ -> 3)

      Scenario.value "exists" (fun db -> query { for c in db.Customers do exists (c.City = "Paris") }) (fun _ -> true)

      Scenario.value "all" (fun db -> query { for c in db.Customers do all (c.Age > 26) }) (fun _ -> false)

      Scenario.value "contains" (fun db -> query { for c in db.Customers do select c.Id; contains 2 }) (fun _ -> true)

      Scenario.value "find" (fun db -> query { for c in db.Customers do find (c.Age < 30) }) (fun data -> data.Customers.[1])

      Scenario.value "head" (fun db -> query { for c in db.Customers do sortBy c.Id; head }) (fun data -> data.Customers.[0])

      Scenario.value
          "headOrDefault"
          (fun db -> query { for c in db.Customers do where (c.Age > 100); headOrDefault })
          (fun _ -> null)

      Scenario.value "last" (fun db -> query { for c in db.Customers do sortBy c.Id; last }) (fun data -> data.Customers.[3])

      Scenario.value
          "lastOrDefault"
          (fun db -> query { for c in db.Customers do where (c.Age > 100); lastOrDefault })
          (fun _ -> null)

      Scenario.value
          "exactlyOne"
          (fun db -> query { for c in db.Customers do where (c.City = "Paris"); exactlyOne })
          (fun data -> data.Customers.[1])

      Scenario.value
          "exactlyOneOrDefault"
          (fun db -> query { for c in db.Customers do where (c.City = "Rome"); exactlyOneOrDefault })
          (fun _ -> null)

      Scenario.value "nth" (fun db -> query { for c in db.Customers do sortBy c.Id; nth 2 }) (fun data -> data.Customers.[2])

      Scenario.value "minBy" (fun db -> query { for c in db.Customers do minBy c.Age }) (customers >> Seq.map _.Age >> Seq.min)

      Scenario.value "maxBy" (fun db -> query { for c in db.Customers do maxBy c.Age }) (customers >> Seq.map _.Age >> Seq.max)

      Scenario.value "sumBy" (fun db -> query { for c in db.Customers do sumBy c.Age }) (customers >> Seq.sumBy _.Age)

      Scenario.value
          "averageBy"
          (fun db -> query { for c in db.Customers do averageBy (float c.Age) })
          (customers >> Seq.averageBy (fun c -> float c.Age))

      Scenario.value
          "minByNullable"
          (fun db -> query { for l in db.Lines do minByNullable l.Discount })
          (discounts >> List.min >> Nullable)

      Scenario.value
          "maxByNullable"
          (fun db -> query { for l in db.Lines do maxByNullable l.Discount })
          (discounts >> List.max >> Nullable)

      Scenario.value
          "sumByNullable"
          (fun db -> query { for l in db.Lines do sumByNullable l.Discount })
          (discounts >> List.sum >> Nullable)

      Scenario.value
          "averageByNullable"
          (fun db -> query { for l in db.Lines do averageByNullable (Nullable.float l.Discount) })
          (discounts >> List.averageBy float >> Nullable) ]

let cases () = sequences () @ values ()
