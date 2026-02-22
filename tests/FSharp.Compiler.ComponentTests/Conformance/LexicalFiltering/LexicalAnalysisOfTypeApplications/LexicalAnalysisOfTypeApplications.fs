// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalFiltering

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module LexicalAnalysisOfTypeApplications =

    let private resourcePath =
        Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", "resources", "tests", "Conformance", "LexicalFiltering", "LexicalAnalysisOfTypeApplications")

    // Migrated from: tests/fsharpqa/Source/Conformance/LexicalFiltering/LexicalAnalysisOfTypeApplications
    // Verify correct lexing of a complex type application
    [<Fact>]
    let ``ComplexTypeApp01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "ComplexTypeApp01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ] // Suppress empty main module warning
        |> compile
        |> shouldSucceed
        |> ignore
