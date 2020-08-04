// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities
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

let test4 () =
    DateTime.Now.Test()

let test5 (x: inref<DateTime>) =
    &x.Test()

let test6 () =
    DateTime.Now.Test().Test().Test()
            """

    [<Test>]
    let ``Extension method scope errors`` () =
        CompilerAssert.TypeCheckWithErrors
            """
open System
open System.Runtime.CompilerServices

[<Extension; AbstractClass; Sealed>]
type Extensions =

    [<Extension>]
    static member Test(x: inref<DateTime>) = &x

let f1 () =
    &DateTime.Now.Test()

let f2 () =
    let result =
        let dt = DateTime.Now
        &dt.Test()
    result

let f3 () =
    Extensions.Test(let dt = DateTime.Now in &dt)

let f4 () =
    let dt = DateTime.Now
    &Extensions.Test(&dt)

let f5 () =
    &Extensions.Test(let dt = DateTime.Now in &dt)
            """
            [|
                (
                    FSharpErrorSeverity.Error,
                    3228,
                    (12, 6, 12, 25),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpErrorSeverity.Error,
                    3228,
                    (17, 10, 17, 19),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpErrorSeverity.Error,
                    3228,
                    (21, 5, 21, 50),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpErrorSeverity.Error,
                    3228,
                    (25, 6, 25, 26),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpErrorSeverity.Error,
                    3228,
                    (28, 6, 28, 51),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
            |]

// TODO: A better way to test the ones below are to use a custom struct in C# code that contains explicit use of their "readonly" keyword.
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

open System.Runtime.CompilerServices
[<Extension; AbstractClass; Sealed>]
type Extensions =

    [<Extension>]
    static member Test(x: inref<DateTime>) = &x

let test1 () =
    DateTime.Now.Test().Date

let test2 () =
    DateTime.Now.Test().Test().Date.Test().Test().Date.Test()
            """
#else
    // Note: Currently this is assuming NET472. That may change which might break these tests. Consider using custom C# code.
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

open System.Runtime.CompilerServices
[<Extension; AbstractClass; Sealed>]
type Extensions =

    [<Extension>]
    static member Test(x: inref<DateTime>) = &x

let test1 () =
    DateTime.Now.Test().Date
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
                (
                    FSharpErrorSeverity.Warning,
                    52,
                    (20, 5, 20, 29),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
            |]
#endif

#if NETCOREAPP
    [<Test>]
    let ``Consume CSharp interface with a method that has a readonly byref`` () =
        let cs =
            """
using System;
using System.Buffers;

namespace Example
{
    public interface IMessageReader
    {
        bool TryParseMessage(in byte input);
    }
}
            """
        let fs =
            """
module Module1

open Example

type MyClass() =

  interface IMessageReader with
      member this.TryParseMessage(input: inref<byte>): bool = 
          failwith "Not Implemented"
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(cs, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fs, Fsx, Library, cmplRefs = [csCmpl])

        CompilerAssert.Compile fsCmpl
        
#endif

    [<Test>]
    let ``Can take native address to get a nativeptr of a mutable value`` () =
        CompilerAssert.Pass
            """
#nowarn "51"

let test () =
    let mutable x = 1
    let y = &&x
    ()
            """

    [<Test>]
    let ``Cannot take native address to get a nativeptr of an immmutable value`` () =
        CompilerAssert.TypeCheckWithErrors
            """
#nowarn "51"

let test () =
    let x = 1
    let y = &&x
    ()
            """ [|
                    (FSharpErrorSeverity.Error, 256, (6, 13, 6, 16), "A value must be mutable in order to mutate the contents or take the address of a value type, e.g. 'let mutable x = ...'")
                |]
