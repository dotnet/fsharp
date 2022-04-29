// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StructTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_Equals.fs"|])>]
    let ``StructTypes - Overload_Equals_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``StructTypes - Overload_GetHashCode_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/StructTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_ToString.fs"|])>]
    let ``StructTypes - Overload_ToString_f`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

