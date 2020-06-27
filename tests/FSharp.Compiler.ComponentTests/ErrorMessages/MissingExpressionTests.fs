// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices


module ``Missing Expression`` =
    
    [<Fact>]
    let ``Missing Expression after let``() =
        CompilerAssert.TypeCheckSingleError
            """
let sum = 0
for x in 0 .. 10 do
    let sum = sum + x
            """
            FSharpErrorSeverity.Error
            588
            (4,5,4,8)
            "The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result."
