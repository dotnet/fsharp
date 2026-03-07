// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Libraries/Core/NativeInterop/stackalloc

namespace Libraries

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for NativeInterop - stackalloc functionality
module NativeInterop =

    // negativesize01.fs - Test that stackalloc with negative size is handled properly
    // <Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Libraries/Core/NativeInterop/stackalloc", Includes=[|"negativesize01.fs"|])>]
    let ``stackalloc - negativesize01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldSucceed
        |> ignore
