// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.QueryMethodSyntaxScenarios

open System.Linq
open System.Linq.Expressions

let private block = [ Violation.Statement ExpressionType.Block ]

let private reading typeName =
    [ Violation.ReadsUnnamedMember typeName ]

let private customers (data: Data) = Seq.ofList data.Customers

let cases () =
    [ Scenario.rows "Select a tuple" (fun db -> db.Customers.Select(fun c -> (c.Id, c.Name))) (customers >> Seq.map (fun c -> c.Id, c.Name))

      Scenario.rows
          "Select a struct tuple"
          (fun db -> db.Customers.Select(fun c -> struct (c.Id, c.Name)))
          (customers >> Seq.map (fun c -> struct (c.Id, c.Name)))

      Scenario.rows
          "Where over a query built earlier"
          (fun db ->
              let withLargeOrders = db.Orders.Where(fun o -> o.Total > 4.0).Select(fun o -> o.CustomerId)
              db.Customers.Where(fun c -> withLargeOrders.Contains c.Id).Select(fun c -> c.Name))
          (customers >> Seq.filter (fun c -> c.Orders |> Seq.exists (fun o -> o.Total > 4.0)) >> Seq.map _.Name)

      Scenario.rows
          "Where over a query built earlier with a let"
          (fun db ->
              let withLargeOrders = db.Orders.Where(fun o -> let total = o.Total in total > 4.0).Select(fun o -> o.CustomerId)
              db.Customers.Where(fun c -> withLargeOrders.Contains c.Id).Select(fun c -> c.Name))
          (customers >> Seq.filter (fun c -> c.Orders |> Seq.exists (fun o -> o.Total > 4.0)) >> Seq.map _.Name)
      |> Scenario.violating block

      Scenario.rows
          "Select a record, fields in declaration order"
          (fun db -> db.Customers.Select(fun c -> { PersonName = c.Name; PersonAge = c.Age }))
          (customers >> Seq.map (fun c -> { PersonName = c.Name; PersonAge = c.Age }))

      Scenario.rows
          "Select a record, fields out of declaration order"
          (fun db -> db.Customers.Select(fun c -> { PersonAge = c.Age; PersonName = c.Name }))
          (customers >> Seq.map (fun c -> { PersonName = c.Name; PersonAge = c.Age }))
      |> Scenario.violating block

      Scenario.rows
          "Select an anonymous record, fields in alphabetical order"
          (fun db -> db.Customers.Select(fun c -> {| Age = c.Age; Name = c.Name |}))
          (customers >> Seq.map (fun c -> {| Age = c.Age; Name = c.Name |}))

      Scenario.rows
          "Select an anonymous record, fields out of alphabetical order"
          (fun db -> db.Customers.Select(fun c -> {| Name = c.Name; Age = c.Age |}))
          (customers >> Seq.map (fun c -> {| Age = c.Age; Name = c.Name |}))
      |> Scenario.violating block

      Scenario.rows
          "Select with a let used twice"
          (fun db -> db.Customers.Select(fun c -> let upper = c.Name.ToUpper() in upper + upper))
          (customers >> Seq.map (fun c -> c.Name.ToUpper() + c.Name.ToUpper()))
      |> Scenario.violating block

      Scenario.rows
          "Where taking a tuple apart"
          (fun db -> db.Customers.Select(fun c -> (c.Id, c.Name)).Where(fun (id, _) -> id > 1))
          (customers >> Seq.map (fun c -> c.Id, c.Name) >> Seq.filter (fun (id, _) -> id > 1))
      |> Scenario.violating (reading "Tuple`2" @ block)

      Scenario.rows
          "Where taking the last of nine apart"
          (fun db ->
              db.Customers
                  .Select(fun c -> (c.Id + 100, c.Name, c.City, c.Age, c.Id * 7, c.Name.Length, c.City.Length, c.Age * 2, c.Id))
                  .Where(fun (_, _, _, _, _, _, _, _, last) -> last > 1)
                  .Select(fun (_, name, _, _, _, _, _, _, _) -> name))
          (customers >> Seq.filter (fun c -> c.Id > 1) >> Seq.map _.Name)
      |> Scenario.violating (reading "Tuple`8" @ block)

      Scenario.rows
          "Where over a query that selects a tuple"
          (fun db -> (query { for c in db.Customers do select (c.Id, c.Name) }).Where(fun (id, _) -> id > 1))
          (customers >> Seq.map (fun c -> c.Id, c.Name) >> Seq.filter (fun (id, _) -> id > 1))
      |> Scenario.violating (reading "Tuple`2" @ block)

      Scenario.rows
          "Where on a nullable property"
          (fun db -> db.Customers.Where(fun c -> c.Registered.HasValue && c.Registered.Value.Year > 2020))
          (customers >> Seq.filter (fun c -> c.Registered.HasValue && c.Registered.Value.Year > 2020))
      |> Scenario.violating block

      Scenario.rows
          "Where with a lambda over another source"
          (fun db -> db.Customers.Where(fun c -> db.Orders.Any(fun o -> o.CustomerId = c.Id)))
          (customers >> Seq.filter (fun c -> c.Orders.Count > 0))
      |> Scenario.violating
          [ Violation.RuntimeHelperCall "LeafExpressionConverter.QuotationToLambdaExpression"
            Violation.RuntimeHelperCall "LeafExpressionConverter.SubstHelper" ]

      Scenario.rows
          "OrderBy, ThenByDescending"
          (fun db -> db.Customers.OrderBy(fun c -> c.City).ThenByDescending(fun c -> c.Age).Select(fun c -> c.Name))
          (customers >> Seq.sortBy (fun c -> c.City, -c.Age) >> Seq.map _.Name)
      |> Scenario.ordered

      Scenario.rows
          "GroupBy a tuple"
          (fun db -> db.Customers.GroupBy(fun c -> (c.City, c.Age > 28)).Select(fun g -> (g.Key, g.Count())))
          (customers >> Seq.countBy (fun c -> c.City, c.Age > 28))

      Scenario.rows
          "SelectMany over a navigation collection"
          (fun db -> db.Customers.SelectMany(fun c -> c.Orders :> seq<Order>).Select(fun o -> o.Id))
          (fun data -> data.Orders |> Seq.map _.Id) ]
