// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.QueryFSharpOperatorScenarios

open System.Linq

let private customers (data: Data) = Seq.ofList data.Customers

let private lines (data: Data) = Seq.ofList data.Lines

let private withoutLines (data: Data) =
    data.Orders |> Seq.filter (fun o -> o.Lines.Count = 0) |> Seq.map _.Id

let private withLines (data: Data) =
    seq { for o in data.Orders do for l in o.Lines -> o.Id, l.Id }

let private math () =
    [ Scenario.rows
          "abs and sign in where"
          (fun db -> query { for c in db.Customers do where (abs (c.Age - 30) < 5 && sign (c.Age - 30) < 0); select c.Name })
          (customers >> Seq.filter (fun c -> abs (c.Age - 30) < 5 && sign (c.Age - 30) < 0) >> Seq.map _.Name)

      Scenario.rows
          "min and max in where"
          (fun db -> query { for c in db.Customers do where (max c.Age 30 = 30 && min c.Age 26 = 26); select c.Name })
          (customers >> Seq.filter (fun c -> max c.Age 30 = 30 && min c.Age 26 = 26) >> Seq.map _.Name)

      Scenario.rows
          "abs, sign, min and max in select"
          (fun db -> query { for c in db.Customers do select (abs (c.Age - 30), sign (c.Age - 30), min c.Age 30, max c.Age 30) })
          (customers >> Seq.map (fun c -> abs (c.Age - 30), sign (c.Age - 30), min c.Age 30, max c.Age 30))

      Scenario.rows
          "floor and ceil in where"
          (fun db -> query { for l in db.Lines do where (floor l.Price = 2.0 || ceil l.Price = 3.0); select l.Id })
          (lines >> Seq.filter (fun l -> floor l.Price = 2.0 || ceil l.Price = 3.0) >> Seq.map _.Id)

      Scenario.rows
          "round and truncate in where"
          (fun db -> query { for l in db.Lines do where (round (l.Price * 1.1) = 3.0 && truncate l.Price < 3.0); select l.Id })
          (lines >> Seq.filter (fun l -> round (l.Price * 1.1) = 3.0 && truncate l.Price < 3.0) >> Seq.map _.Id)

      Scenario.rows
          "sqrt and ** in where"
          (fun db -> query { for l in db.Lines do where (sqrt l.Price > 1.6 && l.Price ** 2.0 < 10.0); select l.Id })
          (lines >> Seq.filter (fun l -> sqrt l.Price > 1.6 && l.Price ** 2.0 < 10.0) >> Seq.map _.Id)

      Scenario.rows
          "exp, log and log10 in where"
          (fun db ->
              query {
                  for c in db.Customers do
                  where (exp (float c.Age / 10.0) > 15.0 && Operators.log (float c.Age) < 3.5 && log10 (float c.Age) > 1.4)
                  select c.Name
              })
          (customers
           >> Seq.filter (fun c -> exp (float c.Age / 10.0) > 15.0 && Operators.log (float c.Age) < 3.5 && log10 (float c.Age) > 1.4)
           >> Seq.map _.Name)

      Scenario.rows
          "sin, cos and tan in where"
          (fun db -> query { for c in db.Customers do where (sin (float c.Age) < 0.0 && cos (float c.Age) > 0.0 && tan (float c.Age) < 0.0); select c.Name })
          (customers >> Seq.filter (fun c -> sin (float c.Age) < 0.0 && cos (float c.Age) > 0.0 && tan (float c.Age) < 0.0) >> Seq.map _.Name)

      Scenario.rows
          "asin, acos, atan and atan2 in where"
          (fun db ->
              query {
                  for c in db.Customers do
                  let ratio = float c.Age / 100.0
                  where (asin ratio > 0.26 && acos ratio > 1.2 && atan ratio < 0.33 && atan2 (float c.Age) 100.0 > 0.25)
                  select c.Name
              })
          (customers
           >> Seq.filter (fun c ->
               let ratio = float c.Age / 100.0
               asin ratio > 0.26 && acos ratio > 1.2 && atan ratio < 0.33 && atan2 (float c.Age) 100.0 > 0.25)
           >> Seq.map _.Name)

      Scenario.rows
          "sinh, cosh and tanh in where"
          (fun db ->
              query {
                  for c in db.Customers do
                  where (sinh (float c.Age / 10.0) > 8.0 && cosh (float c.Age / 10.0) < 15.0 && tanh (float c.Age / 10.0) > 0.99)
                  select c.Name
              })
          (customers
           >> Seq.filter (fun c -> sinh (float c.Age / 10.0) > 8.0 && cosh (float c.Age / 10.0) < 15.0 && tanh (float c.Age / 10.0) > 0.99)
           >> Seq.map _.Name)

      Scenario.rows
          "sqrt, floor, ceil and truncate in select"
          (fun db -> query { for l in db.Lines do select (sqrt l.Price, floor l.Price, ceil l.Price, truncate l.Price) })
          (lines >> Seq.map (fun l -> sqrt l.Price, floor l.Price, ceil l.Price, truncate l.Price))

      Scenario.rows
          "sqrt of a float32 in where"
          (fun db -> query { for l in db.Lines do where (sqrt (float32 l.Price) > 1.6f); select l.Id })
          (lines >> Seq.filter (fun l -> sqrt (float32 l.Price) > 1.6f) >> Seq.map _.Id) ]

let private tuples () =
    [ Scenario.rows
          "fst and snd after select into"
          (fun db -> query { for c in db.Customers do select (c.Id, c.Name) into t; where (fst t > 1); select (snd t) })
          (customers >> Seq.filter (fun c -> c.Id > 1) >> Seq.map _.Name)

      Scenario.rows
          "fst and snd of a let-bound tuple"
          (fun db -> query { for c in db.Customers do let t = (c.Id, c.Name) in where (fst t > 1); select (snd t, fst t) })
          (customers >> Seq.filter (fun c -> c.Id > 1) >> Seq.map (fun c -> c.Name, c.Id)) ]

let private nulls () =
    [ Scenario.rows
          "isNull in where"
          (fun db ->
              query {
                  for o in db.Orders do
                  leftOuterJoin l in db.Lines on (o.Id = l.OrderId) into lines
                  for l in lines do
                  where (isNull l)
                  select o.Id
              })
          withoutLines

      Scenario.rows
          "isNull of a boxed nullable in select"
          (fun db -> query { for c in db.Customers do select (c.Name, isNull (box c.Registered)) })
          (customers >> Seq.map (fun c -> c.Name, not c.Registered.HasValue))

      Scenario.rows
          "box compared with null in where"
          (fun db ->
              query {
                  for o in db.Orders do
                  leftOuterJoin l in db.Lines on (o.Id = l.OrderId) into lines
                  for l in lines do
                  where (box l <> null)
                  select (o.Id, l.Id)
              })
          withLines

      Scenario.rows
          "boxed nullable compared with null in where"
          (fun db -> query { for c in db.Customers do where (box c.Registered = null); select c.Name })
          (customers >> Seq.filter (fun c -> not c.Registered.HasValue) >> Seq.map _.Name)

      Scenario.rows
          "not in where"
          (fun db -> query { for c in db.Customers do where (not (c.Age > 28)); select c.Name })
          (customers >> Seq.filter (fun c -> not (c.Age > 28)) >> Seq.map _.Name) ]

let private methodSyntax () =
    [ Scenario.rows
          "Where with abs and max"
          (fun db -> db.Customers.Where(fun c -> abs (c.Age - 30) <= 2 && max c.Age 29 = c.Age).Select(fun c -> c.Name))
          (customers >> Seq.filter (fun c -> abs (c.Age - 30) <= 2 && max c.Age 29 = c.Age) >> Seq.map _.Name)

      Scenario.rows
          "Where with sqrt and sin"
          (fun db -> db.Lines.Where(fun l -> sqrt l.Price > 1.6 && sin l.Price > 0.0).Select(fun l -> l.Id))
          (lines >> Seq.filter (fun l -> sqrt l.Price > 1.6 && sin l.Price > 0.0) >> Seq.map _.Id)

      Scenario.rows
          "Select with floor and sign"
          (fun db -> db.Lines.Select(fun l -> (l.Id, floor l.Price, sign (l.Quantity - 2))))
          (lines >> Seq.map (fun l -> l.Id, floor l.Price, sign (l.Quantity - 2)))

      Scenario.rows
          "GroupBy a tuple, fst of the key"
          (fun db -> db.Customers.GroupBy(fun c -> (c.City, c.Age > 28)).Select(fun g -> (fst g.Key, g.Count())))
          (customers >> Seq.countBy (fun c -> c.City, c.Age > 28) >> Seq.map (fun ((city, _), count) -> city, count))

      Scenario.rows
          "Where with isNull"
          (fun db -> db.Customers.Where(fun c -> not (isNull c.Name) && isNull (box c.Registered)).Select(fun c -> c.Name))
          (customers >> Seq.filter (fun c -> not c.Registered.HasValue) >> Seq.map _.Name)

      Scenario.rows
          "Where with box compared with null"
          (fun db -> db.Customers.Where(fun c -> box c.Registered <> null).Select(fun c -> c.Name))
          (customers >> Seq.filter (fun c -> c.Registered.HasValue) >> Seq.map _.Name) ]

let cases () = math () @ tuples () @ nulls () @ methodSyntax ()
