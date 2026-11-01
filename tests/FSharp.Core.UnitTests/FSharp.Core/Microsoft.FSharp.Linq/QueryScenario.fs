// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Linq

open System
open System.Collections
open System.Globalization
open System.Linq
open System.Linq.Expressions
open System.Reflection
open Microsoft.FSharp.Reflection

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type Outcome =
    | Rows of IQueryable
    | Value of obj

/// One query, what it must return, and what is known to be wrong with its expression tree today.
[<NoEquality; NoComparison>]
type Scenario =
    { Name: string
      Run: Db -> Outcome
      Expected: Data -> obj list
      Ordered: bool
      Spine: string list voption
      Violations: Violation list }

    override this.ToString() = this.Name

module Scenario =

    let private allMembers = BindingFlags.Public ||| BindingFlags.NonPublic

    [<return: Struct>]
    let private (|Grouping|_|) (value: obj) =
        match value.GetType().GetInterface(typedefof<IGrouping<_, _>>.Name) with
        | null -> ValueNone
        | grouping -> ValueSome(grouping.GetProperty("Key").GetValue value)

    [<return: Struct>]
    let private (|Tuple|_|) (value: obj) =
        if FSharpType.IsTuple(value.GetType()) then
            ValueSome(FSharpValue.GetTupleFields value)
        else
            ValueNone

    [<return: Struct>]
    let private (|Record|_|) (value: obj) =
        if FSharpType.IsRecord(value.GetType(), allMembers) then
            let names = FSharpType.GetRecordFields(value.GetType(), allMembers) |> Seq.map _.Name
            ValueSome(Seq.zip names (FSharpValue.GetRecordFields(value, allMembers)))
        else
            ValueNone

    [<return: Struct>]
    let private (|Union|_|) (value: obj) =
        if FSharpType.IsUnion(value.GetType(), allMembers) then
            let case, fields = FSharpValue.GetUnionFields(value, value.GetType(), allMembers)
            ValueSome struct (case.Name, fields)
        else
            ValueNone

    /// Text that is equal for equal results, whichever provider materialized them.
    let rec render (value: obj) =
        let renderAll (values: seq<obj>) =
            values |> Seq.map render |> String.concat "; "

        match value with
        | null -> "null"
        | :? string as text -> $"\"{text}\""
        | :? IFormattable as number -> number.ToString(null, CultureInfo.InvariantCulture)
        | Grouping key -> $"{render key} => [{renderAll (Seq.cast (value :?> IEnumerable))}]"
        | :? IEnumerable as items -> $"[{renderAll (Seq.cast items)}]"
        | Tuple elements -> $"({renderAll elements})"
        | Record fields ->
            let rendered = fields |> Seq.map (fun (name, field) -> $"{name} = {render field}")
            $"""{{ {String.concat "; " rendered} }}"""
        | Union(case, fields) -> $"{case}({renderAll fields})"
        | value -> string value

    let rows name (run: Db -> IQueryable<'T>) (expected: Data -> seq<'T>) =
        { Name = name
          Run = fun db -> Outcome.Rows(run db)
          Expected = fun data -> [ for row in expected data -> box row ]
          Ordered = false
          Spine = ValueNone
          Violations = [] }

    let value name (run: Db -> 'T) (expected: Data -> 'T) =
        { Name = name
          Run = fun db -> Outcome.Value(box (run db))
          Expected = fun data -> [ box (expected data) ]
          Ordered = true
          Spine = ValueNone
          Violations = [] }

    let ordered scenario = { scenario with Ordered = true }

    /// The operators the source chain must consist of, outermost last.
    let flat spine scenario =
        { scenario with
            Spine = ValueSome spine }

    /// What is wrong with the expression tree today; the scenario fails once that stops being true.
    let violating violations scenario =
        { scenario with
            Violations = violations }

    [<NoEquality; NoComparison>]
    type private Observed =
        { Expression: Expression
          Violations: Violation list
          Rows: obj list }

    let rec private rootCause (error: exn) =
        match error, error.InnerException with
        | :? TargetInvocationException, null -> error
        | :? TargetInvocationException, inner -> rootCause inner
        | _ -> error

    let private observe (scenario: Scenario) =
        let provider = CapturingQueryProvider()
        let data = Data()

        let observed =
            try
                match scenario.Run(Db.inMemory provider data) with
                | Outcome.Rows rows ->
                    let stayed = obj.ReferenceEquals(rows.Provider, provider)

                    { Expression = rows.Expression
                      Violations = (if stayed then [] else [ Violation.LeftProvider ])
                      Rows = [ for row in rows -> row ] }
                // The value comes from the last execution; whatever ran before it went to the provider too.
                | Outcome.Value value when provider.Executed.Count > 0 ->
                    let last = provider.Executed.[provider.Executed.Count - 1]
                    let earlier = provider.Executed |> Seq.take (provider.Executed.Count - 1)

                    { Expression = last
                      Violations =
                        [ for expression in earlier do
                              yield! ProviderShape.violations expression

                          // Enumerating the rows of a query is an execution too, so the last one has to be the
                          // operator that gives the value: a count made over those rows is made by the client.
                          if not (ProviderShape.isTerminalOperator last) then
                              Violation.LeftProvider ]
                      Rows = [ value ] }
                | Outcome.Value value ->
                    { Expression = Expression.Empty()
                      Violations = [ Violation.LeftProvider ]
                      Rows = [ value ] }
            with error ->
                { Expression = Expression.Empty()
                  Violations = [ Violation.Throws((rootCause error).GetType().Name) ]
                  Rows = [] }

        data, observed

    let private inOrder ordered (rows: obj list) =
        let rendered = List.map render rows
        if ordered then rendered else List.sort rendered

    /// Everything the scenario gets wrong, empty when it behaves as described.
    let failures (scenario: Scenario) =
        let data, observed = observe scenario

        let violations =
            observed.Violations @ ProviderShape.violations observed.Expression
            |> List.distinct

        let threw =
            violations
            |> List.exists (function
                | Violation.Throws _ -> true
                | _ -> false)

        [ if set violations <> set scenario.Violations then
              $"violations: expected %A{scenario.Violations}, got %A{violations}"

          if not threw then
              let expected = inOrder scenario.Ordered (scenario.Expected data)
              let actual = inOrder scenario.Ordered observed.Rows

              if expected <> actual then
                  $"rows: expected %A{expected}, got %A{actual}"

              match scenario.Spine with
              | ValueSome spine when spine <> ProviderShape.spine observed.Expression ->
                  $"operators: expected %A{spine}, got %A{ProviderShape.spine observed.Expression}"
              | _ -> () ]
        |> List.map (fun failure -> $"{scenario.Name}: {failure}\n  in {observed.Expression}")

    let check scenario =
        match failures scenario with
        | [] -> ()
        | failures -> failwith (String.concat "\n" failures)
