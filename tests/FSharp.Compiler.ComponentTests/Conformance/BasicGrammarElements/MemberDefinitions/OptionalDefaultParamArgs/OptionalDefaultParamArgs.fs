// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MemberDefinitions_OptionalDefaultParamArgs =

    let fsLibrary =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "Library.fs")
        |> asLibrary
        |> withName "Library"

    //let csLibrary =
    //    CSharpFromPath (__SOURCE_DIRECTORY__ ++ "CallFSharpMethods.cs")
    //    |> withName "Library"

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    // SOURCE=E_OnlyDefault.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnlyDefault.fs"|])>]
    let ``E_OnlyDefault_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 13, Col 18, Line 13, Col 20, "This expression was expected to have type\n    'int'    \nbut here has type\n    'unit'    ")
        ]

    // SOURCE=InterfaceMethod.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InterfaceMethod.fs"|])>]
    let ``InterfaceMethod_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //// SOURCE=Library.fs POSTCMD="\$CSC_PIPE -r:Library.dll CallFSharpMethods.cs && CallFSharpMethods.exe" SCFLAGS=-a
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CallFSharpMethods.cs"|])>]
    //let ``CallFSharpMethods_cs`` _compilation =
    //    compilation
    //    |> withReferences [fsLibrary]
    //    |> verifyCompileAndRun
    //    |> shouldSucceed

    // SOURCE=Library.fs POSTCMD="\$FSC_PIPE -r:Library.dll CallMethods.fs && CallMethods.exe" SCFLAGS=-a
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CallMethods.fs"|])>]
    let ``CallMethods_fs`` compilation =
        compilation
        |> withReferences [fsLibrary]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Sanity.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Sanity.fs"|])>]
    let ``Sanity_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=W_WrongDefaultType.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_WrongDefaultType.fs"|])>]
    let ``W_WrongDefaultType_fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics 
                      [ Warning 3211, Line 10, Col 62, Line 10, Col 63, "The default value does not have the same type as the argument. The DefaultParameterValue attribute and any Optional attribute will be ignored. Note: 'null' needs to be annotated with the correct type, e.g. 'DefaultParameterValue(null:obj)'."
                        Error 1, Line 13, Col 25, Line 13, Col 27, "This expression was expected to have type
                        'string' 
                        but here has type
                        'unit' "]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_WrongDefaultObj.fs"|])>]
    let ``W_WrongDefaultObjType_fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics 
            [ Warning 3211, Line 10, Col 62, Line 10, Col 67, "The default value does not have the same type as the argument. The DefaultParameterValue attribute and any Optional attribute will be ignored. Note: 'null' needs to be annotated with the correct type, e.g. 'DefaultParameterValue(null:obj)'."
              Warning 3397, Line 12, Col 19, Line 12, Col 21, """This expression uses 'unit' for an 'obj'-typed argument. This will lead to passing 'null' at runtime. This warning may be disabled using '#nowarn "3397"."""]




