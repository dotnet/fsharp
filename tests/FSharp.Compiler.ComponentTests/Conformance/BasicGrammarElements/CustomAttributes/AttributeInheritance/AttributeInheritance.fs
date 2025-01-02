// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CustomAttributes_AttributeInheritance =

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
    [<Theory; FileInlineData("InheritedAttribute_001.fs")>]
    let ``InheritedAttribute_001_fs`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--nowarn:3242"]
        |> verifyCompile
        |> shouldSucceed

    // SOURCE=InheritedAttribute_002.fs  SCFLAGS="--target:library"		                # InheritedAttribute_002.fs
    [<Theory; FileInlineData("InheritedAttribute_002.fs")>]
    let ``InheritedAttribute_002_fs`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--nowarn:3242"]
        |> verifyCompile
        |> shouldSucceed


