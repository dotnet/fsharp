// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ExplicitObjectConstructors =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ExplicitObjectConstructors)
    [<Theory; FileInlineData("new_while_01.fs")>]
    let ``new_while_01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ExplicitObjectConstructors)
    
    [<Theory; FileInlineData("WithAttribute01.fs")>]
    let ``WithAttribute01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0044
        |> withDiagnosticMessageMatches "Message2"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ExplicitObjectConstructors)
    
    [<Theory; FileInlineData("WithAttribute02.fs")>]
    let ``WithAttribute02_fs`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0044
        |> withDiagnosticMessageMatches "Message1"
        |> ignore
