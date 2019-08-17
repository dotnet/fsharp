// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ByrefTests =

    [<Test>]
    let ``No defensive copy on .NET struct`` () =
        CompilerAssert.Pass
            """
let f (x: int) = x.GetHashCode()
let f2 () =
    let x = 1
    x.GetHashCode()
            """

    [<Test>]
    let ``Defensive copy on .NET struct for inref`` () =
        CompilerAssert.TypeCheckWithErrors
            """
let f (x: inref<int>) = x.GetHashCode()
let f2 () =
    let x = 1
    let y = &x
    y.GetHashCode()
            """
            [|
                (
                    FSharpErrorSeverity.Warning,
                    52,
                    (2, 25, 2, 40),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
                (
                    FSharpErrorSeverity.Warning,
                    52,
                    (6, 5, 6, 20),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
            |]
