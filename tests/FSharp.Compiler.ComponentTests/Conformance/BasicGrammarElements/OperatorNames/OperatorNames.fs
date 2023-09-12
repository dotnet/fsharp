// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module OperatorNames =

    // NoMT SOURCE=Atat.fsx FSIMODE=FEED COMPILE_ONLY=1                # Atat - fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Atat.fsx"|])>]
    let``Atat_fsx_fsi`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> runFsi
        |> shouldSucceed

    // SOURCE=Atat.fsx                                                 # Atat - fsc
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Atat.fsx"|])>]
    let``Atat_fsx-fsc`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=AstrSymbOper01.fs                                        # AstrSymbOper01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AstrSymbOper01.fs"|])>]
    let``AstrSymbOper01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=BasicOperatorNames.fs                                    # BasicOperatorNames.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"BasicOperatorNames.fs"|])>]
    let``BasicOperatorNames_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=EqualOperatorsOverloading.fs                             # EqualOperatorsOverloading.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EqualOperatorsOverloading.fs"|])>]
    let``EqualOperatorsOverloading_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_BasicOperatorNames01.fs SCFLAGS="--test:ErrorRanges"   # E_BasicOperatorNames01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_BasicOperatorNames01.fs"|])>]
    let``E_BasicOperatorNames01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 35, Line 7, Col 6, Line 7, Col 23, "This construct is deprecated: '$' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 8, Col 5, Line 8, Col 22, "This construct is deprecated: '$' is not permitted as a character in operator names and is reserved for future use")
        ]

    // SOURCE=RefAssignment01.fs                                       # RefAssignment01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RefAssignment01.fs"|])>]
    let``RefAssignment01_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:3370"]
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:3370"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OperatorGreaterColon01.fs"|])>]
    let``E_OperatorGreaterColon01_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:1172" ; "--nowarn:1173"]
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 35, Line 2, Col 10, Line 2, Col 12, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 4, Col 10, Line 4, Col 12, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 4, Col 15, Line 4, Col 17, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 7, Col 24, Line 7, Col 26, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 10, Col 24, Line 10, Col 26, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 10, Col 29, Line 10, Col 31, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 13, Col 24, Line 13, Col 26, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
            (Error 35, Line 13, Col 29, Line 13, Col 31, "This construct is deprecated: ':' is not permitted as a character in operator names and is reserved for future use")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OperatorGreaterColon02.fs"|])>]
    let ``E_OperatorGreaterColon02_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:1172" ; "--nowarn:1173"]
        |> asFsx
        |> runFsi
        |> shouldFail
        |> withDiagnostics [
            
        ]