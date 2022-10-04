// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler


module InterfaceImplAugmentation =
    [<Fact>]
    let ``Public Module Abbreviation``() =
        FSharp """
namespace AugmentationsTest
type MyCustomType<'T> = 
    | Data of string
    interface System.IDisposable

type MyCustomType<'T> with
    interface System.IDisposable with
                member x.Dispose() = printfn "Finish"   """
        |> typecheck
        |> shouldSucceed
        |> withSingleDiagnostic (Warning 69, Line 6, Col 20, Line 6, Col 30,
                                 """Interface implementations should normally be given on the initial declaration of a type. Interface implementations in augmentations may lead to accessing static bindings before they are initialized, though only if the interface implementation is invoked during initialization of the static data, and in turn access the static data.  You may remove this warning using #nowarn "69" if you have checked this is not the case.""")

   