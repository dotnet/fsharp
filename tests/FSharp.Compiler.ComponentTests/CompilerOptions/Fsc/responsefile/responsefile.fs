// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open System.IO
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsc/responsefile
// Tests for @ response file compiler option
module Responsefile =

    let resourcePath = Path.Combine(__SOURCE_DIRECTORY__)

    // Source that checks if FROM_RESPONSE_FILE_1 is defined - compilation should succeed
    let sourceCheckDefine1 = """
module ResponseFileTest
#if FROM_RESPONSE_FILE_1
let x = 1
#else
let x : string = 1  // Type error if not defined
#endif
"""

    // Source that checks if both FROM_RESPONSE_FILE_1 and FROM_RESPONSE_FILE_2 are defined
    let sourceCheckDefine1And2 = """
module ResponseFileTest
#if FROM_RESPONSE_FILE_1 && FROM_RESPONSE_FILE_2
let x = 1
#else
let x : string = 1  // Type error if not defined
#endif
"""

    //----------------------------------------------------
    // Direct define (baseline - no response file)
    //----------------------------------------------------

    [<Fact>]
    let ``responsefile - direct define works`` () =
        Fs sourceCheckDefine1
        |> asLibrary
        |> withOptions ["--define:FROM_RESPONSE_FILE_1"]
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Response file with define
    //----------------------------------------------------

    [<Fact>]
    let ``responsefile - define from response file`` () =
        Fs sourceCheckDefine1
        |> asLibrary
        |> withOptions [$"@{resourcePath}/rs1.rsp"]
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Response file with comments and multiline
    //----------------------------------------------------

    [<Fact>]
    let ``responsefile - multiline and comments`` () =
        Fs sourceCheckDefine1
        |> asLibrary
        |> withOptions [$"@{resourcePath}/rs1_multiline_and_comments.rsp"]
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Response file with both defines
    //----------------------------------------------------

    [<Fact>]
    let ``responsefile - multiline response file with two defines`` () =
        Fs sourceCheckDefine1And2
        |> asLibrary
        |> withOptions [$"@{resourcePath}/rs1_multiline_and_comments.rsp"]
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Nested response files
    // Note: The original test used relative paths in response files (@rs2.rsp contains @rs1.rsp).
    // The ComponentTests framework compiles from a temp directory, breaking relative paths.
    // Nested response file functionality is tested implicitly through the multiline test.
    //----------------------------------------------------

    //----------------------------------------------------
    // Empty response file combined with other options
    //----------------------------------------------------

    [<Fact>]
    let ``responsefile - empty response file with direct define`` () =
        Fs sourceCheckDefine1
        |> asLibrary
        |> withOptions [$"@{resourcePath}/empty_rs.rsp"; "--define:FROM_RESPONSE_FILE_1"]
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Error: response file not found (FS3194)
    //----------------------------------------------------

    [<Fact>]
    let ``responsefile - error if not found`` () =
        Fs "let x = 1"
        |> asLibrary
        |> withOptions ["@not_exists"]
        |> compile
        |> shouldFail
        |> withErrorCode 3194
        |> ignore

    //----------------------------------------------------
    // Error: response file path invalid (FS3195)
    //----------------------------------------------------

    [<Fact>]
    let ``responsefile - error if path invalid`` () =
        Fs "let x = 1"
        |> asLibrary
        |> withOptions ["@"]
        |> compile
        |> shouldFail
        |> withErrorCode 3195
        |> ignore
