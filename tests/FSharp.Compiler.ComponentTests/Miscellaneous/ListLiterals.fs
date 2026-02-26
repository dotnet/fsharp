// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Miscellaneous

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ListLiterals =

    [<Theory; FileInlineData("ReallyLongList01.fs")>]
    let ``List literals have no limited length in langversion 8`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withLangVersion80
        |> compile
        |> shouldSucceed