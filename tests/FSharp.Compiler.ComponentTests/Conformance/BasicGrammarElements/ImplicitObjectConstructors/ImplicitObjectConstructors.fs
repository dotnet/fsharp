// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ImplicitObjectConstructors =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ImplicitObjectConstructors)
    
    [<Theory; FileInlineData("WithAttribute.fs")>]
    let ``WithAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0044
        |> withDiagnosticMessageMatches "Message1"
        |> ignore

