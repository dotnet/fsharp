// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test.Compiler

module WarnForAutoOpenAttributeAlias =
    [<Fact>]
    let ``Warn user when aliasing FSharp.Core.AutoOpenAttribute`` () =
        Fsx """
type ByItsOwnNatureUnBottledAttribute = FSharp.Core.AutoOpenAttribute

[<ByItsOwnNatureUnBottled>]
module Foo =
    let bar = 0
"""
        |> withLangVersionPreview
         |> compile
         |> shouldFail
         |> withDiagnostics [
             (Warning 3559, Line 2, Col 6, Line 2, Col 38, "FSharp.Core.AutoOpenAttribute should not be aliased.");
         ]
