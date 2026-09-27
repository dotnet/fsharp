// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Linq

open Xunit

type QueryShapeTests() =

    static let rows (cases: Scenario list) =
        cases |> Seq.map (fun scenario -> [| box scenario |])

    static member Projections = rows (QueryProjectionScenarios.cases ())
    static member Operators = rows (QueryOperatorScenarios.cases ())
    static member Keys = rows (QueryKeyScenarios.cases ())
    static member Structures = rows (QueryStructureScenarios.cases ())
    static member MethodSyntax = rows (QueryMethodSyntaxScenarios.cases ())

    [<Theory; MemberData(nameof QueryShapeTests.Projections)>]
    member _.Projection(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.Operators)>]
    member _.Operator(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.Keys)>]
    member _.Key(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.Structures)>]
    member _.Structure(scenario: Scenario) = Scenario.check scenario

    [<Theory; MemberData(nameof QueryShapeTests.MethodSyntax)>]
    member _.``Method syntax``(scenario: Scenario) = Scenario.check scenario
