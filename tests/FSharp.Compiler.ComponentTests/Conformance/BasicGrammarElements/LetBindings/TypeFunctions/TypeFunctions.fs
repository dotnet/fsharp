// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module LetBindings_TypeFunctions =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compileAndRun

    //SOURCE=typeofBasic001.fs                            # typeofBasic001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeofBasic001.fs"|])>]
    let ``typeofBasic001_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeofInCustomAttributes001.fs"|])>]
    let ``typeofInCustomAttributes001_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_typeof_measure_01.fs"|])>]
    let ``E_typeof_measure_01_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 704, Line 9, Col 16, Line 9, Col 17, "Expected type, not unit-of-measure")
        ]

    //SOURCE=E_NoTypeFuncsInTypes.fs                      # E_NoTypeFuncsInTypes.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_NoTypeFuncsInTypes.fs"|])>]
    let ``E_NoTypeFuncsInTypes_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 671, Line 6, Col 26, Line 6, Col 28, "A property cannot have explicit type parameters. Consider using a method instead.");
            (Error 671, Line 7, Col 27, Line 7, Col 33, "A property cannot have explicit type parameters. Consider using a method instead.")
        ]

    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_typeof_undefined_01.fs"|])>]
    let ``E_typeof_undefined_01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 929, Line 7, Col 6, Line 7, Col 7, "This type requires a definition")
        ]

    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_anonymous_01.fs"|])>]
    let ``typeof_anonymous_01_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_class_01.fs"|])>]
    let ``typeof_class_01_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_interface_01.fs"|])>]
    let ``typeof_interface_01_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_struct_01.fs"|])>]
    let ``typeof_struct_01_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=SizeOf01.fs                                  # SizeOf01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SizeOf01.fs"|])>]
    let ``SizeOf01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=typeofAsArgument.fs                          # typeofAsArgument.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeofAsArgument.fs"|])>]
    let ``typeofAsArgument_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed
