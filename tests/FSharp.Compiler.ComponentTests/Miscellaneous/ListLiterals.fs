// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Miscellaneous

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ListLiterals =

    [<Theory; FileInlineData("ReallyLongList01.fs")>]
    let ``List literals still have limited length in langversion 6`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withLangVersion60
        |> compile
        |> shouldFail
        |> withErrorCode 742
        |> withDiagnosticMessageMatches "This list expression exceeds the maximum size for list literals. Use an array for larger literals and call Array.ToList."

    [<Theory; FileInlineData("ReallyLongList01.fs")>]
    let ``List literals have no limited length in langversion preview`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withLangVersion70
        |> compile
        |> shouldSucceed