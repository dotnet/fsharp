// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Globalization

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ``Globalization Test Cases`` =

    let ``Compile global source file`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Theory; FileInlineData("Arabic.fs")>]
    let ``Compile Arabic source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("gb18030.fs")>]
    let ``Compile gb18030 source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("HexCharEncode.fs")>]
    let ``Compile HexCharEncode source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("Hindi.fs")>]
    let ``Compile Hindi source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("Mongolian.fs")>]
    let ``Compile Mongolian source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("RightToLeft.fs")>]
    let ``Compile RightToLeft source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("Surrogates.fs")>]
    let ``Compile Surrogates source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("Tamil.fs")>]
    let ``Compile Tamil source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("Tibetan.fs")>]
    let ``Compile Tibetan source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("utf16.fs")>]
    let ``Compile utf16 source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("utf8.fs")>]
    let ``Compile utf8 source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; FileInlineData("Yi.fs")>]
    let ``Compile Yi source file`` compilation =
        ``Compile global source file`` compilation
