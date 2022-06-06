// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.AccessibilityAnnotations

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module OnTypeMembers =

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

    // SOURCE=AccessProtectedStatic01.fs SCFLAGS="-r:BaseClass.dll"    PRECMD="\$CSC_PIPE /target:library BaseClass.cs"                         # AccessProtectedStatic01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AccessProtectedStatic01.fs"|])>]
    let ``AccessProtectedStatic01_fs`` compilation =
        let lib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "BaseClass.cs")
            |> withName "ReadWriteLib"

        compilation
        |> withReferences [lib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_AccessProtectedInstance01.fs SCFLAGS="-r:BaseClass.dll --test:ErrorRanges"    PRECMD="\$CSC_PIPE /target:library BaseClass.cs"  # AccessProtectedInstance01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AccessProtectedInstance01.fs"|])>]
    let ``AccessProtectedInstance01_fs`` compilation =
        let lib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "BaseClass.cs")
            |> withName "ReadWriteLib"

        compilation
        |> withReferences [lib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_AccessPrivateMember01.fs SCFLAGS="--test:ErrorRanges"          # E_AccessPrivateMember01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AccessPrivateMember01.fs"|])>]
    let ``E_AccessPrivateMember01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 9, Col 10, Line 9, Col 17, "The struct, record or union type 'SpecSet' is not structurally comparable because the type 'SpecMulti' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'SpecSet' to clarify that the type is not comparable")
            (Error 491, Line 14, Col 13, Line 14, Col 28, "The member or object constructor 'Impl' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
        ]

    // SOURCE=E_OnProperty01.fs                                                # E_OnProperty01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnProperty01.fs"|])>]
    let ``E_OnProperty01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 491, Line 42, Col 1, Line 42, Col 6, "The member or object constructor 'Foo' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 9, Line 42, Col 16, "The member or object constructor 'test1' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 19, Line 42, Col 26, "The member or object constructor 'test2' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 29, Line 42, Col 36, "The member or object constructor 'test5' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 42, Col 40, Line 42, Col 47, "The member or object constructor 'test6' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 807, Line 42, Col 60, Line 42, Col 67, "Property 'test8' is not readable")
            (Error 491, Line 43, Col 1, Line 43, Col 13, "The member or object constructor 'test3' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 44, Col 1, Line 44, Col 13, "The member or object constructor 'test4' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 45, Col 1, Line 45, Col 13, "The member or object constructor 'test5' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 491, Line 46, Col 1, Line 46, Col 13, "The member or object constructor 'test6' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (Error 810, Line 47, Col 1, Line 47, Col 8, "Property 'test7' cannot be set")
            (Error 257, Line 47, Col 1, Line 47, Col 8, "Invalid mutation of a constant expression. Consider copying the expression to a mutable local, e.g. 'let mutable x = ...'.")
        ]

    // SOURCE=E_OnProperty02.fs SCFLAGS="--test:ErrorRanges"                   # E_OnProperty02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnProperty02.fs"|])>]
    let ``E_OnProperty02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 15, Col 49, Line 15, Col 50, "Unexpected symbol ')' in pattern")
            (Error 1244, Line 15, Col 48, Line 15, Col 50, "Attempted to parse this as an operator name, but failed")
            (Error 558, Line 16, Col 36, Line 16, Col 50, "Multiple accessibilities given for property getter or setter")
            (Error 558, Line 19, Col 35, Line 19, Col 56, "Multiple accessibilities given for property getter or setter")
            (Error 10, Line 20, Col 49, Line 20, Col 50, "Unexpected identifier in pattern")
            (Error 1244, Line 20, Col 48, Line 20, Col 57, "Attempted to parse this as an operator name, but failed")
            (Error 10, Line 23, Col 36, Line 23, Col 42, "Unexpected keyword 'public' in member definition")
        ]

    // SOURCE=OnProperty01.fs                                                  # OnProperty01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OnProperty01.fs"|])>]
    let ``OnProperty01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed
