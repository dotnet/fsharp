// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.QueryStructureScenarios

open System.Linq

let private quotedLambda =
    [ Violation.RuntimeHelperCall "LeafExpressionConverter.QuotationToLambdaExpression"
      Violation.RuntimeHelperCall "LeafExpressionConverter.SubstHelper" ]

let private nested operators =
    operators |> List.map Violation.NestedInCollectionSelector

let private orDefault (items: seq<'T>) =
    if Seq.isEmpty items then Seq.singleton Unchecked.defaultof<'T> else items

let private idOrNone (order: Order) =
    match order with
    | null -> -1
    | order -> order.Id

let private nameOrNone (product: Product) =
    match product with
    | null -> ""
    | product -> product.Name

let private ordersWithLines (data: Data) =
    seq { for c in data.Customers do for o in c.Orders do for l in o.Lines -> c.Name, o.Id, l.Id }

let private customersWithOrders (data: Data) =
    seq { for c in data.Customers do for o in c.Orders -> c.Name, o.Id }

let private leftJoinedOrders (data: Data) =
    seq { for c in data.Customers do for o in orDefault c.Orders -> c.Name, idOrNone o }

let private leftJoinedOrdersAndProducts (data: Data) =
    seq {
        for c in data.Customers do
            for o in orDefault c.Orders do
                for p in orDefault (data.Products |> List.filter (fun p -> p.Id = c.Id)) -> c.Name, idOrNone o, nameOrNone p
    }

let private loops () =
    [ Scenario.rows
          "two for"
          (fun db -> query { for c in db.Customers do for p in db.Products do select (c.Name, p.Name) })
          (fun data -> seq { for c in data.Customers do for p in data.Products -> c.Name, p.Name })
      |> Scenario.flat [ "SelectMany"; "Select"; "Select" ]

      Scenario.rows
          "two for, where over both"
          (fun db -> query { for c in db.Customers do for o in db.Orders do where (o.CustomerId = c.Id); select (c.Name, o.Id) })
          customersWithOrders
      |> Scenario.flat [ "SelectMany"; "Where"; "Select"; "Select" ]

      Scenario.rows
          "for, where, for, where"
          (fun db ->
              query {
                  for c in db.Customers do
                  where (c.Age > 26)
                  for o in db.Orders do
                  where (o.CustomerId = c.Id)
                  select (c.Name, o.Id)
              })
          (fun data -> seq { for c in data.Customers do if c.Age > 26 then for o in c.Orders -> c.Name, o.Id })
      |> Scenario.flat [ "Where"; "SelectMany"; "Where"; "Select"; "Select" ]

      Scenario.rows
          "for over a navigation collection"
          (fun db -> query { for c in db.Customers do for o in c.Orders do select (c.Name, o.Id) })
          customersWithOrders
      |> Scenario.flat [ "SelectMany"; "Select"; "Select" ]

      Scenario.rows
          "for over two levels of navigation collections"
          (fun db -> query { for c in db.Customers do for o in c.Orders do for l in o.Lines do select (c.Name, o.Id, l.Id) })
          ordersWithLines
      |> Scenario.violating (nested [ "SelectMany" ])

      Scenario.rows
          "three for"
          (fun db ->
              query {
                  for c in db.Customers do
                  for o in db.Orders do
                  for l in db.Lines do
                  where (o.CustomerId = c.Id && l.OrderId = o.Id)
                  select (c.Name, o.Id, l.Id)
              })
          ordersWithLines
      |> Scenario.violating (nested [ "SelectMany" ])

      Scenario.rows
          "for after sortBy"
          (fun db -> query { for c in db.Customers do sortBy c.Age; for p in db.Products do select (c.Name, p.Name) })
          (fun data -> seq { for c in data.Customers |> List.sortBy _.Age do for p in data.Products -> c.Name, p.Name })
      |> Scenario.ordered
      |> Scenario.flat [ "OrderBy"; "SelectMany"; "Select"; "Select" ]

      Scenario.rows
          "for after join"
          (fun db ->
              query {
                  for c in db.Customers do
                  join o in db.Orders on (c.Id = o.CustomerId)
                  for l in db.Lines do
                  where (l.OrderId = o.Id)
                  select (c.Name, o.Id, l.Id)
              })
          ordersWithLines
      |> Scenario.flat [ "Join"; "SelectMany"; "Where"; "Select"; "Select" ]

      Scenario.rows
          "let between for"
          (fun db ->
              query { for c in db.Customers do let next = c.Id + 9 in for o in db.Orders do where (o.Id = next); select (c.Name, o.Id) })
          (fun data -> seq { for c in data.Customers do for o in data.Orders do if o.Id = c.Id + 9 then yield c.Name, o.Id })
      |> Scenario.flat [ "SelectMany"; "Where"; "Select"; "Select" ]

      Scenario.rows
          "if then around for"
          (fun db -> query { for c in db.Customers do if c.Age > 26 then for o in c.Orders do yield (c.Name, o.Id) })
          (fun data -> seq { for c in data.Customers do if c.Age > 26 then for o in c.Orders -> c.Name, o.Id })
      |> Scenario.flat [ "Where"; "SelectMany"; "Select" ]

      Scenario.rows
          "for around if then"
          (fun db -> query { for c in db.Customers do for o in db.Orders do if o.CustomerId = c.Id then yield (c.Name, o.Id) })
          customersWithOrders
      |> Scenario.flat [ "SelectMany"; "Select" ]

      Scenario.rows
          "yield! of a navigation collection"
          (fun db -> query { for c in db.Customers do yield! c.Orders })
          (fun data -> Seq.ofList data.Orders)
      |> Scenario.flat [ "SelectMany" ]

      Scenario.rows
          "distinct and take after two for"
          (fun db -> query { for c in db.Customers do for o in c.Orders do select c.City; distinct; take 1 })
          (fun _ -> seq { "London" })
      |> Scenario.ordered
      |> Scenario.flat [ "SelectMany"; "Select"; "Distinct"; "Take" ]

      Scenario.rows
          "groupBy after two for"
          (fun db -> query { for c in db.Customers do for o in c.Orders do groupBy c.City into g; select (g.Key, g.Count()) })
          (fun data -> seq { for c in data.Customers do for _ in c.Orders -> c.City } |> Seq.countBy id)
      |> Scenario.violating [ Violation.RuntimeHelperCall "new Grouping`2" ]

      Scenario.value "count over two for" (fun db -> query { for c in db.Customers do for o in c.Orders do count }) (fun _ -> 4)

      Scenario.value
          "exists over two for"
          (fun db -> query { for c in db.Customers do for o in c.Orders do exists (o.Total > 10.0 && c.City = "London") })
          (fun _ -> true)
      |> Scenario.violating [ Violation.ReadsUnnamedMember "Tuple`2" ] ]

let private subqueries () =
    [ Scenario.rows
          "nested query as the source"
          (fun db -> query { for o in (query { for o in db.Orders do where (o.Total > 4.0); select o }) do select o.Id })
          (fun data -> seq { for o in data.Orders do if o.Total > 4.0 then yield o.Id })
      |> Scenario.flat [ "Where"; "Select" ]

      Scenario.rows
          "nested query as the source of the second for"
          (fun db ->
              query {
                  for c in db.Customers do
                  for o in (query { for o in db.Orders do where (o.CustomerId = c.Id); select o }) do
                  select (c.Name, o.Id)
              })
          customersWithOrders
      |> Scenario.flat [ "SelectMany"; "Select"; "Select" ]

      Scenario.rows
          "nested query in select"
          (fun db ->
              query { for c in db.Customers do select (c.Name, query { for o in db.Orders do where (o.CustomerId = c.Id); select o.Id }) })
          (fun data -> seq { for c in data.Customers -> c.Name, (seq { for o in c.Orders -> o.Id }).AsQueryable() })

      Scenario.rows
          "second for over a source filtered by a lambda"
          (fun db -> query { for c in db.Customers do for o in db.Orders.Where(fun o -> o.CustomerId = c.Id) do select (c.Name, o.Id) })
          customersWithOrders
      |> Scenario.violating quotedLambda

      Scenario.rows
          "where with a lambda over another source"
          (fun db -> query { for c in db.Customers do where (db.Orders.Any(fun o -> o.CustomerId = c.Id)); select c.Name })
          (fun data -> seq { for c in data.Customers do if c.Orders.Count > 0 then yield c.Name })
      |> Scenario.violating quotedLambda ]

let private leftJoins () =
    [ Scenario.rows
          "left join spelled leftOuterJoin"
          (fun db ->
              query {
                  for c in db.Customers do
                  leftOuterJoin o in db.Orders on (c.Id = o.CustomerId) into orders
                  for o in orders do
                  select (c.Name, (if obj.ReferenceEquals(o, null) then -1 else o.Id))
              })
          leftJoinedOrders
      |> Scenario.flat [ "GroupJoin"; "SelectMany"; "Select"; "Select" ]

      Scenario.rows
          "left join spelled groupJoin and DefaultIfEmpty"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupJoin o in db.Orders on (c.Id = o.CustomerId) into orders
                  for o in orders.DefaultIfEmpty() do
                  select (c.Name, (if obj.ReferenceEquals(o, null) then -1 else o.Id))
              })
          leftJoinedOrders
      |> Scenario.flat [ "GroupJoin"; "SelectMany"; "Select"; "Select" ]

      Scenario.rows
          "two left joins spelled leftOuterJoin"
          (fun db ->
              query {
                  for c in db.Customers do
                  leftOuterJoin o in db.Orders on (c.Id = o.CustomerId) into orders
                  for o in orders do
                  leftOuterJoin p in db.Products on (c.Id = p.Id) into products
                  for p in products do
                  select
                      (c.Name,
                       (if obj.ReferenceEquals(o, null) then -1 else o.Id),
                       (if obj.ReferenceEquals(p, null) then "" else p.Name))
              })
          leftJoinedOrdersAndProducts
      |> Scenario.violating (nested [ "GroupJoin"; "SelectMany" ])

      Scenario.rows
          "two left joins spelled groupJoin and DefaultIfEmpty"
          (fun db ->
              query {
                  for c in db.Customers do
                  groupJoin o in db.Orders on (c.Id = o.CustomerId) into orders
                  for o in orders.DefaultIfEmpty() do
                  groupJoin p in db.Products on (c.Id = p.Id) into products
                  for p in products.DefaultIfEmpty() do
                  select
                      (c.Name,
                       (if obj.ReferenceEquals(o, null) then -1 else o.Id),
                       (if obj.ReferenceEquals(p, null) then "" else p.Name))
              })
          leftJoinedOrdersAndProducts
      |> Scenario.violating (nested [ "GroupJoin"; "SelectMany" ])

      Scenario.rows
          "join after a left join"
          (fun db ->
              query {
                  for c in db.Customers do
                  leftOuterJoin o in db.Orders on (c.Id = o.CustomerId) into orders
                  for o in orders do
                  join p in db.Products on (c.Id = p.Id)
                  select (c.Name, (if obj.ReferenceEquals(o, null) then -1 else o.Id), p.Name)
              })
          (leftJoinedOrdersAndProducts >> Seq.filter (fun (_, _, product) -> product <> ""))
      |> Scenario.violating (nested [ "Join" ]) ]

let cases () = loops () @ subqueries () @ leftJoins ()
