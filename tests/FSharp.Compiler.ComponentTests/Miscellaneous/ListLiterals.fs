// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Miscellaneous

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ListLiterals =

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReallyLongList01.fs"|])>]
    let ``List literals still have limited length in langversion 6`` compilation =
        compilation
        |> asFsx
        |> withLangVersion60
        |> compile
        |> shouldFail
        |> withErrorCode 742
        |> withDiagnosticMessageMatches "This list expression exceeds the maximum size for list literals. Use an array for larger literals and call Array.ToList."

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReallyLongList01.fs"|])>]
    let ``List literals have no limited length in langversion preview`` compilation =
        compilation
        |> asFsx
        |> withLangVersion70
        |> compile
        |> shouldSucceed