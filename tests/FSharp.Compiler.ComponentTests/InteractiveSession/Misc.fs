// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.InteractiveSession

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Misc =

    // This test was automatically generated (moved from FSharpQA suite - InteractiveSession/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/InteractiveSession/Misc", Includes=[|"DefinesCompiled.fs"|])>]
    let ``Misc - DefinesCompiled.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - InteractiveSession/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/InteractiveSession/Misc", Includes=[|"lib.fs"|])>]
    let ``Misc - lib.fs - --nologo -a -o aaa\lib.dll`` compilation =
        compilation
        |> withOptions ["--nologo"; "-a"; "-o"; "aaa\lib.dll"]
        |> typecheck
        |> shouldSucceed

