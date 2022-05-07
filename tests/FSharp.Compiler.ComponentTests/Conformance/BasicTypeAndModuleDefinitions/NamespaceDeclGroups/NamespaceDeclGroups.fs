// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NamespaceDeclGroups =

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

    // SOURCE="E_BeginWithNamespace01a.fs E_BeginWithNamespace01b.fs" SCFLAGS="--test:ErrorRanges --vserrors"   # E_BeginWithNamespace01
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_BeginWithNamespace01a.fs"|])>]
    let ``E_BeginWithNamespace01a_fs`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "E_BeginWithNamespace01b.fs"))
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 222, Line 6, Col 1, Line 7, Col 1, "Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration.")
        ]

    // SOURCE=NoWarnOnJustNamespace.fs SCFLAGS="--warnaserror:58"                                               # NoWarnOnJustNamespace.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoWarnOnJustNamespace.fs"|])>]
    let ``NoWarnOnJustNamespace_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TypeInGlobalNamespace01.fs SCFLAGS="-r:FooGlobal.dll"  PRECMD="\$FSC_PIPE -a FooGlobal.fs"        # TypeInGlobalNamespace01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeInGlobalNamespace01.fs"|])>]
    let ``TypeInGlobalNamespace01_fs`` compilation =
        let libFoo =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "FooGlobal.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [libFoo]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TypeInGlobalNamespace02.fs SCFLAGS="-r:FooGlobal.dll"  PRECMD="\$FSC_PIPE -a FooGlobal.fs"        # TypeInGlobalNamespace02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TypeInGlobalNamespace02.fs"|])>]
    let ``TypeInGlobalNamespace02_fs`` compilation =
        let libFoo =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "FooGlobal.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [libFoo]
        |> verifyCompileAndRun
        |> shouldSucceed


 