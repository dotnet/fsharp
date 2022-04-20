// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.CustomAttributes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributeInheritance =

    let verifyCompile compilation =
        compilation
        |> asLibrary
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asLibrary
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

        // SOURCE=InheritedAttribute_001.fs  SCFLAGS="--target:library --warnaserror:3242" 	# InheritedAttribute_001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InheritedAttribute_001.fs"|])>]
    let ``InheritedAttribute_001_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:3242"]
        |> verifyCompile
        |> shouldSucceed

    // SOURCE=InheritedAttribute_002.fs  SCFLAGS="--target:library"		                # InheritedAttribute_002.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InheritedAttribute_002.fs"|])>]
    let ``InheritedAttribute_002_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:3242"]
        |> verifyCompile
        |> shouldSucceed


