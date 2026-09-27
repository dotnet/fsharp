// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Linq.QueryProjectionScenarios

open System.Linq.Expressions

let private block = [ Violation.Statement ExpressionType.Block ]

let private customers (data: Data) = Seq.ofList data.Customers

let cases () =
    [ Scenario.rows "scalar" (fun db -> query { for c in db.Customers do select c.Name }) (customers >> Seq.map _.Name)

      Scenario.rows "entity" (fun db -> query { for c in db.Customers do select c }) customers

      Scenario.rows
          "computed scalars"
          (fun db -> query { for c in db.Customers do select (c.Name + "!", c.Age + 1, (if c.Age > 28 then "senior" else "junior")) })
          (customers >> Seq.map (fun c -> c.Name + "!", c.Age + 1, (if c.Age > 28 then "senior" else "junior")))

      Scenario.rows
          "tuple of two"
          (fun db -> query { for c in db.Customers do select (c.Id, c.Name) })
          (customers >> Seq.map (fun c -> c.Id, c.Name))

      Scenario.rows
          "tuple of seven"
          (fun db -> query { for c in db.Customers do select (c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2) })
          (customers >> Seq.map (fun c -> c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2))

      Scenario.rows
          "tuple of eight"
          (fun db -> query { for c in db.Customers do select (c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2, c.Id * 7) })
          (customers >> Seq.map (fun c -> c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2, c.Id * 7))

      Scenario.rows
          "tuple of nine"
          (fun db -> query { for c in db.Customers do select (c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2, c.Id * 7, c.Age - c.Id) })
          (customers >> Seq.map (fun c -> c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2, c.Id * 7, c.Age - c.Id))

      Scenario.rows
          "nested tuple"
          (fun db -> query { for c in db.Customers do select ((c.Id, c.Name), c.Age) })
          (customers >> Seq.map (fun c -> (c.Id, c.Name), c.Age))

      Scenario.rows
          "struct tuple of two"
          (fun db -> query { for c in db.Customers do select (struct (c.Id, c.Name)) })
          (customers >> Seq.map (fun c -> struct (c.Id, c.Name)))

      Scenario.rows
          "struct tuple of nine"
          (fun db ->
              query { for c in db.Customers do select (struct (c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2, c.Id * 7, c.Age - c.Id)) })
          (customers >> Seq.map (fun c -> struct (c.Id, c.Name, c.City, c.Age, c.Id + 100, c.Name.Length, c.Age * 2, c.Id * 7, c.Age - c.Id)))

      Scenario.rows
          "struct tuple inside a tuple"
          (fun db -> query { for c in db.Customers do select (struct (c.Id, c.Name), c.Age) })
          (customers >> Seq.map (fun c -> struct (c.Id, c.Name), c.Age))

      Scenario.rows
          "tuple inside a struct tuple"
          (fun db -> query { for c in db.Customers do select (struct ((c.Id, c.Name), c.Age)) })
          (customers >> Seq.map (fun c -> struct ((c.Id, c.Name), c.Age)))

      Scenario.rows
          "tuple taken apart by the next for"
          (fun db ->
              query {
                  for (id, name) in (query { for c in db.Customers do select (c.Id, c.Name) }) do
                  where (id > 1)
                  select name
              })
          (customers >> Seq.filter (fun c -> c.Id > 1) >> Seq.map _.Name)
      |> Scenario.violating [ Violation.ReadsUnnamedMember "Tuple`2" ]

      Scenario.rows
          "struct tuple taken apart by the next for"
          (fun db ->
              query {
                  for struct (id, name) in (query { for c in db.Customers do select (struct (c.Id, c.Name)) }) do
                  where (id > 1)
                  select name
              })
          (customers >> Seq.filter (fun c -> c.Id > 1) >> Seq.map _.Name)
      |> Scenario.violating [ Violation.ReadsUnnamedMember "ValueTuple`2" ]

      Scenario.rows
          "record, fields in declaration order"
          (fun db -> query { for c in db.Customers do select { PersonName = c.Name; PersonAge = c.Age } })
          (customers >> Seq.map (fun c -> { PersonName = c.Name; PersonAge = c.Age }))

      Scenario.rows
          "record, fields out of declaration order"
          (fun db -> query { for c in db.Customers do select { PersonAge = c.Age; PersonName = c.Name } })
          (customers >> Seq.map (fun c -> { PersonName = c.Name; PersonAge = c.Age }))
      |> Scenario.violating block

      Scenario.rows
          "record of three, fields rotated"
          (fun db -> query { for c in db.Customers do select { Town = c.City; FullName = c.Name; Years = c.Age } })
          (customers >> Seq.map (fun c -> { FullName = c.Name; Years = c.Age; Town = c.City }))
      |> Scenario.violating block

      Scenario.rows
          "record holding a tuple"
          (fun db -> query { for c in db.Customers do select {| Age = c.Age; Pair = (c.Id, c.Name) |} })
          (customers >> Seq.map (fun c -> {| Age = c.Age; Pair = (c.Id, c.Name) |}))

      Scenario.rows
          "anonymous record, fields in alphabetical order"
          (fun db -> query { for c in db.Customers do select {| Age = c.Age; Name = c.Name |} })
          (customers >> Seq.map (fun c -> {| Age = c.Age; Name = c.Name |}))

      Scenario.rows
          "anonymous record, fields out of alphabetical order"
          (fun db -> query { for c in db.Customers do select {| Name = c.Name; Age = c.Age |} })
          (customers >> Seq.map (fun c -> {| Age = c.Age; Name = c.Name |}))
      |> Scenario.violating block

      Scenario.rows
          "anonymous record, method calls in fields out of order"
          (fun db -> query { for c in db.Customers do select {| Upper = c.Name.ToUpper(); City = c.City.Trim() |} })
          (customers >> Seq.map (fun c -> {| City = c.City.Trim(); Upper = c.Name.ToUpper() |}))
      |> Scenario.violating block

      Scenario.rows
          "anonymous record inside an anonymous record"
          (fun db -> query { for c in db.Customers do select {| Person = {| Name = c.Name; Age = c.Age |} |} })
          (customers >> Seq.map (fun c -> {| Person = {| Age = c.Age; Name = c.Name |} |}))
      |> Scenario.violating block

      Scenario.rows
          "struct anonymous record, fields in alphabetical order"
          (fun db -> query { for c in db.Customers do select (struct {| Age = c.Age; Name = c.Name |}) })
          (customers >> Seq.map (fun c -> struct {| Age = c.Age; Name = c.Name |}))

      Scenario.rows
          "struct anonymous record, fields out of alphabetical order"
          (fun db -> query { for c in db.Customers do select (struct {| Name = c.Name; Age = c.Age |}) })
          (customers >> Seq.map (fun c -> struct {| Age = c.Age; Name = c.Name |}))
      |> Scenario.violating block

      Scenario.rows
          "CLIMutable record, fields in declaration order"
          (fun db -> query { for c in db.Customers do select { DtoName = c.Name; DtoAge = c.Age } })
          (customers >> Seq.map (fun c -> { DtoName = c.Name; DtoAge = c.Age }))

      Scenario.rows
          "CLIMutable record, fields out of declaration order"
          (fun db -> query { for c in db.Customers do select { DtoAge = c.Age; DtoName = c.Name } })
          (customers >> Seq.map (fun c -> { DtoName = c.Name; DtoAge = c.Age }))
      |> Scenario.violating block

      Scenario.rows
          "class with property initializers"
          (fun db -> query { for c in db.Customers do select (CustomerView(ViewName = c.Name, ViewAge = c.Age)) })
          (customers >> Seq.map (fun c -> CustomerView(ViewName = c.Name, ViewAge = c.Age)))

      Scenario.rows "option" (fun db -> query { for c in db.Customers do select (Some c.Age) }) (customers >> Seq.map (fun c -> Some c.Age))
      |> Scenario.violating [ Violation.RuntimeHelperCall "FSharpOption`1.Some" ] ]
