// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance

open FSharp.Test.Compiler

/// Shared test helpers for Conformance tests
module SharedTestHelpers =

    /// C# library with OverloadResolutionPriority test types, shared across Tiebreaker and ORP tests
    let csharpPriorityLib =
        CSharpFromPath (System.IO.Path.Combine(__SOURCE_DIRECTORY__, "OverloadResolutionPriority", "CSharpPriorityLib.cs"))
        |> withCSharpLanguageVersionPreview
        |> withName "CSharpPriorityLib"
