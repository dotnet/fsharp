// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ValueRestriction =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction", Includes=[|"TypeArgs01.fsx"|])>]
    let ``ValueRestriction - TypeArgs01.fsx - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction", Includes=[|"MemberOrFunction01.fsx"|])>]
    let ``ValueRestriction - MemberOrFunction01.fsx - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction", Includes=[|"MemberOrFunction01Gen.fsx"|])>]
    let ``ValueRestriction - MemberOrFunction01Gen.fsx - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction", Includes=[|"MemberOrFunction02.fsx"|])>]
    let ``ValueRestriction - MemberOrFunction02.fsx - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction", Includes=[|"MemberOrFunction02Gen.fsx"|])>]
    let ``ValueRestriction - MemberOrFunction02Gen.fsx - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ValueRestriction", Includes=[|"TypeFunction01.fsx"|])>]
    let ``ValueRestriction - TypeFunction01.fsx - -a --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

