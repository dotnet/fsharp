// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Interop

open Xunit
open FSharp.Test.Compiler

module ``Simple interop verification`` =

    [<Fact(Skip = "TODO: This is broken RN, since netcoreapp30 is used for C# and 3.1 for F#, should be fixed as part of https://github.com/dotnet/fsharp/issues/9740")>]
    let ``Instantiate C# type from F#`` () =

        let CSLib =
            CSharp """
public class A { }
      """   |> withName "CSLib"

        let FSLib =
             FSharp """
module AMaker
let makeA () : A = A()
         """ |> withName "FSLib" |> withReferences [CSLib]

        let app =
            FSharp """
module ReferenceCSfromFS
let a = AMaker.makeA()
        """ |> withReferences [CSLib; FSLib]

        app
        |> compile
        |> shouldSucceed


    [<Fact(Skip = "TODO: This is broken RN, since netcoreapp30 is used for C# and 3.1 for F#, should be fixed as part of https://github.com/dotnet/fsharp/issues/9740")>]
    let ``Instantiate F# type from C#`` () =
        let FSLib =
            FSharp """
namespace Interop.FS
type Bicycle(manufacturer: string) =
    member this.Manufactirer = manufacturer
        """ |> withName "FSLib"

        let app =
            CSharp """
using Interop.FS;
public class BicycleShop {
    public Bicycle[] cycles;
}
        """ |> withReferences [FSLib]

        app
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Instantiate F# type from C# fails without import`` () =
        let FSLib =
            FSharp """
namespace Interop.FS
type Bicycle(manufacturer: string) =
    member this.Manufactirer = manufacturer
        """ |> withName "FSLib"

        let app =
            CSharp """
public class BicycleShop {
    public Bicycle[] cycles;
}
        """ |> withReferences [FSLib]

        app
        |> compile
        |> shouldFail

    [<Fact>]
    let ``can't mutably set a C#-const field in F#`` () =
        let csLib =
            CSharp """
public static class Holder {
    public const string Label = "label";
}
            """
            |> withName "CsharpConst"

        let fsLib =
            FSharp """
module CannotSetCSharpConst
Holder.Label <- "nope"
            """
            |> withReferences [csLib]

        fsLib
        |> compile
        |> shouldFail

    [<Fact>]
    let ``can't mutably set a C#-readonly field in F#`` () =
        let csLib =
            CSharp """
public static class Holder {
    public static readonly string Label = "label";
}
            """
            |> withName "CsharpReadonly"

        let fsLib =
            FSharp """
module CannotSetCSharpReadonly
Holder.Label <- "nope"
            """
            |> withReferences [csLib]

        fsLib
        |> compile
        |> shouldFail
