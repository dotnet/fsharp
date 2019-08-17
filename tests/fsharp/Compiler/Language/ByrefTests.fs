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
open System
let f (x: DateTime) = x.ToLocalTime()
let f2 () =
    let x = DateTime.Now
    x.ToLocalTime()
            """

#if NETCOREAPP
    // NETCORE makes DateTime a readonly struct; therefore, it should not error.
    [<Test>]
    let ``No defensive copy on .NET struct - netcore`` () =
        CompilerAssert.Pass
            """
open System
let f (x: inref<DateTime>) = x.ToLocalTime()
let f2 () =
    let x = DateTime.Now
    let y = &x
    y.ToLocalTime()
            """
#else
    [<Test>]
    let ``Defensive copy on .NET struct for inref`` () =
        CompilerAssert.TypeCheckWithErrors
            """
open System
let f (x: inref<DateTime>) = x.ToLocalTime()
let f2 () =
    let x = DateTime.Now
    let y = &x
    y.ToLocalTime()
            """
            [|
                (
                    FSharpErrorSeverity.Warning,
                    52,
                    (3, 30, 3, 45),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
                (
                    FSharpErrorSeverity.Warning,
                    52,
                    (7, 5, 7, 20),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
            |]
#endif