// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ClassTypes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ValueRestriction =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeArgs01.fsx"|])>]
    let ``TypeArgs01_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MemberOrFunction01.fsx"|])>]
    let ``MemberOrFunction01_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MemberOrFunction01Gen.fsx"|])>]
    let ``MemberOrFunction01Gen_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MemberOrFunction02.fsx"|])>]
    let ``"MemberOrFunction02_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MemberOrFunction02Gen.fsx"|])>]
    let ``MemberOrFunction02Gen_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ValueRestriction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeFunction01.fsx"|])>]
    let ``TypeFunction01_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"; "--test:ErrorRanges"; "--warnaserror+"]
        |> compile
        |> shouldSucceed
        |> ignore

