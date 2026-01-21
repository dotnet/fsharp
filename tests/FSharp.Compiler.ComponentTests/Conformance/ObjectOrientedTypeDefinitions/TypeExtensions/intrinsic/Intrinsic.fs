// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeExtensionsIntrinsic =

    // Multi-file error test

    [<Fact>]
    let ``E_typeext_int002`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "lib002.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "E_typeext_int002.fs")))
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 854

    // Multi-file success tests

    [<Fact>]
    let ``typeext_int001`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "lib001.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "typeext_int001.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``typeext_int003`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "lib003.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "typeext_int003.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``typeext_int004`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "lib004.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "typeext_int004.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``typeext_int005`` () =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "lib005.fs"))
        |> withAdditionalSourceFile (SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "typeext_int005.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
