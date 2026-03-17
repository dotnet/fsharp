// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SimpleFor =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/ControlFlowExpressions/SimpleFor
    // Test count: 3

    // SOURCE=Downto01.fs
    [<Theory; FileInlineData("Downto01.fs")>]
    let ``Downto01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_ForRequiresInt01.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_ForRequiresInt01.fs")>]
    let ``E_ForRequiresInt01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1

    // SOURCE=ForWithUppercaseIdentifier01.fs
    [<Theory; FileInlineData("ForWithUppercaseIdentifier01.fs")>]
    let ``ForWithUppercaseIdentifier01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
