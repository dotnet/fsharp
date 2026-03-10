// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module IdentifierReplacements =

    // SOURCE: Line01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifierReplacements", Includes=[|"Line01.fs"|])>]
    let ``IdentifierReplacements - Line01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: Line02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifierReplacements", Includes=[|"Line02.fs"|])>]
    let ``IdentifierReplacements - Line02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: SourceFile01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifierReplacements", Includes=[|"SourceFile01.fs"|])>]
    let ``IdentifierReplacements - SourceFile01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore
