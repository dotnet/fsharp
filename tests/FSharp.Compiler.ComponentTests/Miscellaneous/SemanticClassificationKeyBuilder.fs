// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Miscellaneous.SemanticClassificationKeyStoreBuilder

open Xunit
open FSharp.Compiler.EditorServices

[<Fact>]
let ``Build empty`` () =
    let sckBuilder = SemanticClassificationKeyStoreBuilder()
    sckBuilder.WriteAll [||]

    let res = sckBuilder.TryBuildAndReset()
    Assert.Equal(None, res)
