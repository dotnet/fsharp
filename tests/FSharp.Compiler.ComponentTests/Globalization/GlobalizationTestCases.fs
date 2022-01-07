// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Globalization

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module ``Globalization Test Cases`` =

    let ``Compile global source file`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Arabic.fs"|])>]
    let ``Compile Arabic source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"gb18030.fs"|])>]
    let ``Compile gb18030 source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"HexCharEncode.fs"|])>]
    let ``Compile HexCharEncode source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Hindi.fs"|])>]
    let ``Compile Hindi source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Mongolian.fs"|])>]
    let ``Compile Mongolian source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RightToLeft.fs"|])>]
    let ``Compile RightToLeft source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Surrogates.fs"|])>]
    let ``Compile Surrogates source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tamil.fs"|])>]
    let ``Compile Tamil source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tibetan.fs"|])>]
    let ``Compile Tibetan source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"utf16.fs"|])>]
    let ``Compile utf16 source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"utf8.fs"|])>]
    let ``Compile utf8 source file`` compilation =
        ``Compile global source file`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Yi.fs"|])>]
    let ``Compile Yi source file`` compilation =
        ``Compile global source file`` compilation
