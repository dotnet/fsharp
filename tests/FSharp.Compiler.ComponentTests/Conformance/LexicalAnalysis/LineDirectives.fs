// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module LineDirectives =

    // SOURCE: Line01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/LineDirectives", Includes=[|"Line01.fs"|])>]
    let ``LineDirectives - Line01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> ignore

    // SOURCE: Line01.fs SCFLAGS: --warnaserror+ --nowarn:75
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/LineDirectives", Includes=[|"Line01.fs"|])>]
    let ``LineDirectives - Line01_fs - warnaserror`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:75"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> ignore
