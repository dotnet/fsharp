// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO

module AutoOpen =

    // SOURCE="Module01.fs library_with_namespaces01.fs" SCFLAGS=-a
    // The env.lst specifies "Module01.fs library_with_namespaces01.fs" but that's for single file compilation
    // For multi-file, the library needs to come first
    [<Fact>]
    let ``Module01_fs`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "library_with_namespaces01.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "Module01.fs")))
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE="library_with_namespaces01.fs redundant_open01.fs" SCFLAGS=-a
    [<Fact>]
    let ``redundant_open01_fs`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "library_with_namespaces01.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "redundant_open01.fs")))
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE="library_with_namespaces01.fs type_abbreviation01.fs" SCFLAGS=-a
    [<Fact>]
    let ``type_abbreviation01_fs`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "library_with_namespaces01.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "type_abbreviation01.fs")))
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE="library_with_namespaces01.fs namespace01.fs" SCFLAGS=-a
    [<Fact>]
    let ``namespace01_fs`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "library_with_namespaces01.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "namespace01.fs")))
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE="E_library_with_namespaces01.fs E_module02.fs" SCFLAGS="-a --test:ErrorRanges"
    [<Fact>]
    let ``E_module02_fs`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "E_library_with_namespaces01.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "E_module02.fs")))
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 39
        |> ignore
