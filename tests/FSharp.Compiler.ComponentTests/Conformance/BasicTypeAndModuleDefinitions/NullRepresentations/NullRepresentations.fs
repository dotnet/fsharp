// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NullRepresentations =

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

    // SOURCE=E_NullInvalidForFSTypes01.fs              # E_NullInvalidForFSTypes01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_NullInvalidForFSTypes01.fs"|])>]
    let ``E_NullInvalidForFSTypes01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 43, Line 20, Col 10, Line 20, Col 14, "The type 'int list' does not have 'null' as a proper value")
            (Error 43, Line 21, Col 10, Line 21, Col 14, "The type 'DU' does not have 'null' as a proper value")
            (Error 43, Line 22, Col 10, Line 22, Col 14, "The type 'RecType' does not have 'null' as a proper value")
        ]
