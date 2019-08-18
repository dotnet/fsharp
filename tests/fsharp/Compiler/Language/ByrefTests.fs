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
open System.Runtime.CompilerServices

let f (x: DateTime) = x.ToLocalTime()
let f2 () =
    let x = DateTime.Now
    x.ToLocalTime()

[<Extension; AbstractClass; Sealed>]
type Extensions =

    [<Extension>]
    static member Test(x: inref<DateTime>) = &x

    [<Extension>]
    static member Test2(x: byref<DateTime>) = &x

let test (x: inref<DateTime>) =
    x.Test()

let test2 (x: byref<DateTime>) =
    x.Test2()

let test3 (x: byref<DateTime>) =
    x.Test()
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
let f3 (x: inref<DateTime>) = &x
let f4 (x: inref<DateTime>) =
    (f3 &x).ToLocalTime()
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
let f3 (x: inref<DateTime>) = &x
let f4 (x: inref<DateTime>) =
    (f3 &x).ToLocalTime()
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
                (
                    FSharpErrorSeverity.Warning,
                    52,
                    (10, 5, 10, 26),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
            |]
#endif