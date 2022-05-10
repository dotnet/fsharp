// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Diagnostics

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module General =

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects>\(18,22-18,30\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_Keyword_tailcall01.fs"|])>]
    let ``General - W_Keyword_tailcall01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> ignore

