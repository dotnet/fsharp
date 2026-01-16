// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalFiltering

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module HighPrecedenceApplication =

    let private resourcePath =
        Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", "resources", "tests", "Conformance", "LexicalFiltering", "HighPrecedenceApplication")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HighPrecedenceApplication)
    //<Expects status="success"></Expects>
    [<Theory; FileInlineData("RangeOperator01.fs")>]
    let ``RangeOperator01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions [ "-a" ]
        |> withNoWarn 3873 // This construct is deprecated. Sequence expressions should be of the form 'seq { ... }
        |> compile
        |> shouldSucceed
        |> ignore

    // Verify high precedence applications
    // B(e).C  => (B(e)).C
    // B (e).C => B ((e).C)
    [<Fact>]
    let ``BasicCheck01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "BasicCheck01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // #light must not touch certain constructs
    [<Fact>]
    let ``BasicCheck02_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "BasicCheck02.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression test for FSHARP1.0:4161 - Range involving biggest negative number (FSX version)
    [<Fact>]
    let ``RangeOperator01_fsx`` () =
        FsxFromPath (Path.Combine(resourcePath, "RangeOperator01.fsx"))
        |> withNoWarn 3873 // This construct is deprecated. Sequence expressions should be of the form 'seq { ... }
        |> withNoWarn 20 // The result of this expression has type and is implicitly ignored
        |> typecheck
        |> shouldSucceed
        |> ignore
