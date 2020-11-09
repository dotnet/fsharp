// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module InterfaceSpecificationsAndImplementations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/InterfaceSpecificationsAndImplementations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/InterfaceSpecificationsAndImplementations", Includes=[|"GenericMethodsOnInterface01.fs"|])>]
    let ``InterfaceSpecificationsAndImplementations - GenericMethodsOnInterface01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/InterfaceSpecificationsAndImplementations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/InterfaceSpecificationsAndImplementations", Includes=[|"GenericMethodsOnInterface02.fs"|])>]
    let ``InterfaceSpecificationsAndImplementations - GenericMethodsOnInterface02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/InterfaceSpecificationsAndImplementations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/InterfaceSpecificationsAndImplementations", Includes=[|"ConcreteUnitOnInterface01.fs"|])>]
    let ``InterfaceSpecificationsAndImplementations - ConcreteUnitOnInterface01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

