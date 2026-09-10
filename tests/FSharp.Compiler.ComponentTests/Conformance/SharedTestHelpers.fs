// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance

open FSharp.Test.Compiler

module SharedTestHelpers =

    let csharpPriorityLib =
        CSharpFromPath (System.IO.Path.Combine(__SOURCE_DIRECTORY__, "OverloadResolutionPriority", "CSharpPriorityLib.cs"))
        |> withCSharpLanguageVersionPreview
        |> withName "CSharpPriorityLib"
