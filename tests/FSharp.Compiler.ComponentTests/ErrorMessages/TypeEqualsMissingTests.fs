// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices


module ``Type definition missing equals`` =

    [<Fact>]
    let ``Missing equals in DU``() =
        CompilerAssert.TypeCheckSingleError
            """
type X | A | B
            """
            FSharpErrorSeverity.Error
            3360 
            (2, 8, 2, 9)
            "Unexpected symbol in type definition. Did you forget to use the = operator?"
