// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler


module InterfaceImplAugmentation =
    [<Fact>]
    let ``Type in non-recursive namespace show give a warning when augmented externally``() =
        FSharp """
namespace AugmentationsTest
type MyCustomType<'T> = 
    | Data of string
    interface System.IDisposable

type MyCustomType<'T> with
    interface System.IDisposable with
                member x.Dispose() = printfn "Finish"   """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 69, Line 8, Col 15, Line 8, Col 33,
                                 """Interface implementations should normally be given on the initial declaration of a type. Interface implementations in augmentations may lead to accessing static bindings before they are initialized, though only if the interface implementation is invoked during initialization of the static data, and in turn access the static data. You may remove this warning using #nowarn "69" if you have checked this is not the case.""")

    [<Fact>]
    let ``Exception type in non-recursive namespace show give a warning when augmented externally``() =
        FSharp """
namespace AugmentationsTest
type MyCustomExcType<'T>() = 
    inherit System.Exception()   
    interface System.IDisposable

type MyCustomExcType<'T> with
    interface System.IDisposable with
                member x.Dispose() = printfn "Finish"   """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 69, Line 8, Col 15, Line 8, Col 33,
                                 """Interface implementations should normally be given on the initial declaration of a type. Interface implementations in augmentations may lead to accessing static bindings before they are initialized, though only if the interface implementation is invoked during initialization of the static data, and in turn access the static data. You may remove this warning using #nowarn "69" if you have checked this is not the case.""")


    [<Fact>]
    let ``Type in a recursive namespace can be augmented without warning``() =
        FSharp """
namespace rec AugmentationsTest
type MyCustomType<'T> = 
    | Data of string
    interface System.IDisposable

type MyCustomType<'T> with
    interface System.IDisposable with
                member x.Dispose() = printfn "Finish"   """
        |> typecheck
        |> shouldSucceed
        |> withWarnings []

    [<Fact>]
    let ``Exception in a recursive namespace can be augmented without warning``() =
        FSharp """
namespace rec AugmentationsTest
type MyCustomExcType<'T>() = 
    inherit System.Exception()   
    interface System.IDisposable

type MyCustomExcType<'T> with
    interface System.IDisposable with
                member x.Dispose() = printfn "Finish"    """
        |> typecheck
        |> shouldSucceed
        |> withWarnings []

    [<Fact>]
    let ``Type in a recursive module and in a non-recursive namespace can be augmented without warning``() =
        FSharp """
namespace MyNonRecursiveNs
module rec RecursiveModule = 
    type MyCustomType<'T> = 
        | Data of string
        interface System.IDisposable

    type MyCustomType<'T> with
        interface System.IDisposable with
                    member x.Dispose() = printfn "Finish"   """
        |> typecheck
        |> shouldSucceed
        |> withWarnings []

    [<Fact>]
    let ``Type in a nonrecursive module but in a recursive namespace can be augmented without warning``() =
        FSharp """
namespace rec AugmentationsTest
module InnerNonRecursiveModule = 
    type MyCustomType<'T> = 
        | Data of string
        interface System.IDisposable

    type MyCustomType<'T> with
        interface System.IDisposable with
                    member x.Dispose() = printfn "Finish"   """
        |> typecheck
        |> shouldSucceed
        |> withWarnings []

    [<Fact>]
    let ``Type in non-recursive namespace nested in a bigger recursive namespace shows warning``() =
        FSharp """
namespace rec OuuterRec
namespace OuuterRec.InnerNonRec
    type MyCustomType<'T> = 
        | Data of string
        interface System.IDisposable

    type MyCustomType<'T> with
        interface System.IDisposable with
                    member x.Dispose() = printfn "Finish"     """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 69, Line 9, Col 19, Line 9, Col 37,
                                 """Interface implementations should normally be given on the initial declaration of a type. Interface implementations in augmentations may lead to accessing static bindings before they are initialized, though only if the interface implementation is invoked during initialization of the static data, and in turn access the static data. You may remove this warning using #nowarn "69" if you have checked this is not the case.""")

 
   
    [<Fact>]
    let ``Type in non-rec ns show give a warning when augmented externally even when the same file has a recursive (but different) ns``() =
        FSharp """
namespace rec OuuterRec
    module Stuff = 
        let x = 5
namespace TotallyDifferentNs.InnerNonRec
    type MyCustomType<'T> = 
        | Data of string
        interface System.IDisposable

    type MyCustomType<'T> with
        interface System.IDisposable with
                    member x.Dispose() = printfn "Finish"    """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 69, Line 11, Col 19, Line 11, Col 37,
                                 """Interface implementations should normally be given on the initial declaration of a type. Interface implementations in augmentations may lead to accessing static bindings before they are initialized, though only if the interface implementation is invoked during initialization of the static data, and in turn access the static data. You may remove this warning using #nowarn "69" if you have checked this is not the case.""")

    [<Fact>]
    let ``Adding an interface to a previously defined type should still be just an 909 error and nothing else``() =
        FSharp """
type System.Random with
    interface System.IComparable 
    static member Factory() = 1     """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 909, Line 3, Col 15, Line 3, Col 33,
                                 """All implemented interfaces should be declared on the initial declaration of the type""")

 