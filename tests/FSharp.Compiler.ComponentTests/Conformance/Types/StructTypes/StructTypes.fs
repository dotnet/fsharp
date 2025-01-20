// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StructTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/StructTypes)
    [<Theory; FileInlineData("Overload_Equals.fs")>]
    let ``StructTypes - Overload_Equals_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/StructTypes)
    [<Theory; FileInlineData("Overload_GetHashCode.fs")>]
    let ``StructTypes - Overload_GetHashCode_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/StructTypes)
    [<Theory; FileInlineData("Overload_ToString.fs")>]
    let ``StructTypes - Overload_ToString_f`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("E_Inheritance.fs")>]
    let ``StructTypes - E_Inheritance.fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 931, Line 6, Col 12, Line 6, Col 20, "Structs, interfaces, enums and delegates cannot inherit from other types");
            (Error 946, Line 6, Col 12, Line 6, Col 20, "Cannot inherit from interface type. Use interface ... with instead.")
        ]

