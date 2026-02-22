// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Misc/

namespace Miscellaneous

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for Misc - migrated from tests/fsharpqa/Source/Misc/
module MigratedMisc =

    let private resourcePath = __SOURCE_DIRECTORY__ + "/../resources/tests/Misc"

    // E_productioncoverage01.fs - FS0584 Successive patterns should be separated by spaces or tupled
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"E_productioncoverage01.fs"|])>]
    let ``E_productioncoverage01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 584
        |> ignore

    // E_productioncoverage02.fs - FS0008 runtime coercion from indeterminate type
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"E_productioncoverage02.fs"|])>]
    let ``E_productioncoverage02_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 8
        |> ignore

    // E_productioncoverage03.fs - FS0035 deprecated operators
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"E_productioncoverage03.fs"|])>]
    let ``E_productioncoverage03_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 35
        |> ignore

    // E_productioncoverage04.fs - FS0640 parameter with attributes must have a name
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"E_productioncoverage04.fs"|])>]
    let ``E_productioncoverage04_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 640
        |> ignore

    // productioncoverage01.fs - success test for grammar productions
    // Contains expected warnings (FS0025, FS0086)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"productioncoverage01.fs"|])>]
    let ``productioncoverage01_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // UserCodeSnippet01.fs - FS0043 Method or object constructor 'op_Addition' not found
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"UserCodeSnippet01.fs"|])>]
    let ``UserCodeSnippet01_fs`` compilation =
        compilation
        |> asLibrary
        |> typecheck
        |> shouldFail
        |> withErrorCode 43
        |> ignore

    // E_CompiledName.fs - FS0429 CompiledName AllowMultiple=false
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"E_CompiledName.fs"|])>]
    let ``E_CompiledName_fs`` compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 429
        |> ignore

    // Parsing01.fs - success test verifying if...then...else parsing
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"Parsing01.fs"|])>]
    let ``Parsing01_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // Parsing02.fs - FS0020 warning about discarded expression results
    // Contains expected warnings (FS0020)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"Parsing02.fs"|])>]
    let ``Parsing02_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // Global01.fs - success test for global keyword parsing
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"Global01.fs"|])>]
    let ``Global01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldSucceed
        |> ignore

    // ConstraintSolverRecursion01.fs - Test that constraint solver doesn't cause stack overflow
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"ConstraintSolverRecursion01.fs"|])>]
    let ``ConstraintSolverRecursion01_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // UseStatementCallDisposeOnNullValue01.fs - success test for use statement behavior
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"UseStatementCallDisposeOnNullValue01.fs"|])>]
    let ``UseStatementCallDisposeOnNullValue01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldSucceed
        |> ignore

    // FileWithSameNameDiffExt - two-file test (fs loads fsx)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Misc", Includes=[|"FileWithSameNameDiffExt.fs"|])>]
    let ``FileWithSameNameDiffExt_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldSucceed
        |> ignore
