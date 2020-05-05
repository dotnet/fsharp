// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Errors assigning to mutable objects`` =

    [<Test>]
    let ``Assign to immutable error``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
x <- 20
            """
            FSharpErrorSeverity.Error
            27
            (3, 1, 3, 8)
            "This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable x = expression'."