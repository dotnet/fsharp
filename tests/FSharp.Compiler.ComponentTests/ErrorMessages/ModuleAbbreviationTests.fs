// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Utilities.Compiler


module ``Module Abbreviations`` =

    [<Fact>]
    let ``Public Module Abbreviation``() =
        FSharp "module public L1 = List"
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 536, Line 1, Col 1, Line 1, Col 7,
                                 "The 'Public' accessibility attribute is not allowed on module abbreviation. Module abbreviations are always private.")
