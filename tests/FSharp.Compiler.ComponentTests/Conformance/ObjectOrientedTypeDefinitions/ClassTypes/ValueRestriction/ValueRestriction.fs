// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ValueRestriction =

    // Error tests

    [<Theory; FileInlineData("E_NotMemberOrFunction01.fsx")>]
    let ``E_NotMemberOrFunction01_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 30

    // Success tests

    [<Theory; FileInlineData("TypeInferenceVariable01.fsx")>]
    let ``TypeInferenceVariable01_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
