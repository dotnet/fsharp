// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
// Migrated from: tests/fsharpqa/Source/Conformance/SpecialAttributesAndTypes/Imported/System.ThreadStatic
// Test count: 1

namespace Conformance.SpecialAttributesAndTypes

open Xunit
open FSharp.Test.Compiler

module ThreadStatic =

    [<Fact>]
    let ``W_Deprecated01 - ThreadStatic let binding deprecated`` () =
        // Regression test for FSHARP1.0:4226
        // We want to make sure the warning emits the correct suggestion (val and mutable were swapped)
        FSharp """
module M
module Foo =
    [<System.ThreadStatic>]
    let x = 42          // warning
        """
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 56, Line 5, Col 9, Line 5, Col 10, "Thread static and context static 'let' bindings are deprecated. Instead use a declaration of the form 'static val mutable <ident> : <type>' in a class. Add the 'DefaultValue' attribute to this declaration to indicate that the value is initialized to the default value on each new thread.")
        ]
