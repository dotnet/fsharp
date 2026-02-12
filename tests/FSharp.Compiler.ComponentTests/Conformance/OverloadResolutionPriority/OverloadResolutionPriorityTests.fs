// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.OverloadResolutionPriority

open FSharp.Test
open FSharp.Test.Compiler
open Xunit
open Conformance.SharedTestHelpers

module OverloadResolutionPriorityTests =

    [<FactForNETCOREAPP>]
    let ``OverloadResolutionPriority - comprehensive test`` () =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "ORPTestRunner.fs")
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
