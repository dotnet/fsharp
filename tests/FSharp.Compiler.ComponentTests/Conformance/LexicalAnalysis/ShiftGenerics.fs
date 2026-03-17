// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ShiftGenerics =

    // SOURCE: RightShift001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Shift/Generics", Includes=[|"RightShift001.fs"|])>]
    let ``Shift/Generics - RightShift001_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore
