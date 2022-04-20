// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.Diagnostics
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Compiler

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
                    FSharpDiagnosticSeverity.Error,
                    3228,
                    (12, 6, 12, 25),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpDiagnosticSeverity.Error,
                    3228,
                    (17, 10, 17, 19),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpDiagnosticSeverity.Error,
                    3228,
                    (21, 5, 21, 50),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpDiagnosticSeverity.Error,
                    3228,
                    (25, 6, 25, 26),
                    "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope."
                )
                (
                    FSharpDiagnosticSeverity.Error,
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
                    FSharpDiagnosticSeverity.Warning,
                    52,
                    (3, 30, 3, 45),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
                (
                    FSharpDiagnosticSeverity.Warning,
                    52,
                    (7, 5, 7, 20),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
                (
                    FSharpDiagnosticSeverity.Warning,
                    52,
                    (10, 5, 10, 26),
                    "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed"
                )
                (
                    FSharpDiagnosticSeverity.Warning,
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
            CompilationUtil.CreateCSharpCompilation(cs, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fs, Library, cmplRefs = [csCmpl])

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
                    (FSharpDiagnosticSeverity.Error, 256, (6, 13, 6, 16), "A value must be mutable in order to mutate the contents or take the address of a value type, e.g. 'let mutable x = ...'")
                |]

    [<Test>]
    let ``Returning an 'inref<_>' from a property should emit System.Runtime.CompilerServices.IsReadOnlyAttribute on the return type of the signature`` () =
        let src =
            """
module Test

type C() =
    let x = 59
    member _.X: inref<_> = &x
            """

        let verifyProperty = """.property instance int32& modreq([runtime]System.Runtime.InteropServices.InAttribute)
                X()
        {
          .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
          .get instance int32& modreq([runtime]System.Runtime.InteropServices.InAttribute) Test/C::get_X()
        }"""

        let verifyMethod = """.method public hidebysig specialname 
                instance int32& modreq([runtime]System.Runtime.InteropServices.InAttribute) 
                get_X() cil managed
        {
          .param [0]
          .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )"""

        FSharp src
        |> compile
        |> verifyIL [verifyProperty;verifyMethod]
        |> ignore

    [<Test>]
    let ``Returning an 'inref<_>' from a generic method should emit System.Runtime.CompilerServices.IsReadOnlyAttribute on the return type of the signature`` () =
        let src =
            """
module Test

type C<'T>() =
    let x = Unchecked.defaultof<'T>
    member _.X<'U>(): inref<'T> = &x
            """

        let verifyMethod = """.method public hidebysig instance !T& modreq([runtime]System.Runtime.InteropServices.InAttribute) 
                X<U>() cil managed
        {
          .param [0]
          .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )"""

        FSharp src
        |> compile
        |> verifyIL [verifyMethod]
        |> ignore

    [<Test>]
    let ``Returning an 'inref<_>' from an abstract generic method should emit System.Runtime.CompilerServices.IsReadOnlyAttribute on the return type of the signature`` () =
        let src =
            """
module Test

[<AbstractClass>]
type C<'T>() =
    abstract X<'U> : unit -> inref<'U>
            """

        let verifyMethod = """.method public hidebysig abstract virtual 
                instance !!U& modreq([runtime]System.Runtime.InteropServices.InAttribute) 
                X<U>() cil managed
        {
          .param [0]
          .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) """

        FSharp src
        |> compile
        |> verifyIL [verifyMethod]
        |> ignore

    [<Test>]
    let ``Returning an 'inref<_>' from an abstract property should emit System.Runtime.CompilerServices.IsReadOnlyAttribute on the return type of the signature`` () =
        let src =
            """
module Test

type C =
    abstract X: inref<int> 
            """

        let verifyProperty = """.property instance int32& modreq([runtime]System.Runtime.InteropServices.InAttribute)
                X()
        {
          .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
          .get instance int32& modreq([runtime]System.Runtime.InteropServices.InAttribute) Test/C::get_X()
        }"""

        let verifyMethod = """.method public hidebysig specialname abstract virtual
                instance int32& modreq([runtime]System.Runtime.InteropServices.InAttribute) 
                get_X() cil managed
        {
          .param [0]
          .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )"""

        FSharp src
        |> compile
        |> verifyIL [verifyProperty;verifyMethod]
        |> ignore