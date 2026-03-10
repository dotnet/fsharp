// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalFiltering

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =

    let private resourcePath =
        Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", "resources", "tests", "Conformance", "LexicalFiltering", "Basic")

    // Migrated from: tests/fsharpqa/Source/Conformance/LexicalFiltering/Basic/ByExample
    // Sanity check #light functionality
    [<Fact>]
    let ``ByExample BasicCheck01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "ByExample", "BasicCheck01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ] // Suppress empty main module warning
        |> compile
        |> shouldSucceed
        |> ignore

    // Migrated from: tests/fsharpqa/Source/Conformance/LexicalFiltering/Basic/OffsideExceptions
    // Regression test for FSHARP1.0:5205 - Indentation rules
    [<Fact>]
    let ``OffsideExceptions Offside01a_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "OffsideExceptions", "Offside01a.fs"))
        |> asExe
        |> withOptions [ "--test:ErrorRanges"; "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``OffsideExceptions Offside01b_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "OffsideExceptions", "Offside01b.fs"))
        |> asExe
        |> withOptions [ "--test:ErrorRanges"; "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression test for FSHARP1.0:5205 - Indentation rules (should fail by design)
    [<Fact>]
    let ``OffsideExceptions Offside01c_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "OffsideExceptions", "Offside01c.fs"))
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> compile
        |> shouldFail
        |> withDiagnostics
            [
                (Error 10, Line 10, Col 20, Line 10, Col 21, "Unexpected symbol '[' in binding. Expected incomplete structured construct at or before this point or other token.")
                (Error 3118, Line 8, Col 1, Line 8, Col 4, "Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.")
                (Error 10, Line 11, Col 1, Line 11, Col 1, "Incomplete structured construct at or before this point in implementation file")
            ]
        |> ignore
