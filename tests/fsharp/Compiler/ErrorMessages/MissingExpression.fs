// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Missing Expression`` =
    
    [<Test>]
    let ``Missing expression after let``() =
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