// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices


module ``Module Abbreviations`` =

    [<Fact>]
    let ``Public Module Abbreviation``() =
        CompilerAssert.TypeCheckSingleError
            """
module public L1 = List
            """
            FSharpErrorSeverity.Error
            536
            (2, 1, 2, 7)
            "The 'Public' accessibility attribute is not allowed on module abbreviation. Module abbreviations are always private."
