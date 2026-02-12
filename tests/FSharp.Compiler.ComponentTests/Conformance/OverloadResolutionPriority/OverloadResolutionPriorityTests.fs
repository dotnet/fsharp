// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.OverloadResolutionPriority

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

/// Tests for OverloadResolutionPriority attribute support (.NET 9)
module OverloadResolutionPriorityTests =

    [<FactForNETCOREAPP>]
    let ``OverloadResolutionPriority - comprehensive test`` () =
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "CSharpPriorityLib.cs")
            |> withCSharpLanguageVersionPreview
            |> withName "CSharpPriorityLib"
        
        FsFromPath (__SOURCE_DIRECTORY__ ++ "ORPTestRunner.fs")
        |> withReferences [csharpLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
