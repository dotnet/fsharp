// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

/// Tests for implicit DIM (Default Interface Method) slot coverage feature.
module ``DIM Slot Coverage Tests`` =

    let withCSharpLanguageVersion (ver: CSharpLanguageVersion) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | CS cs -> CS { cs with LangVersion = ver }
        | _ -> failwith "Only supported in C#"

    let dimTestLib =
        CSharp """
namespace DIMTest {
    public interface IA { int M(); }
    public interface IB : IA { new int M(); int IA.M() => this.M() + 100; }
}
namespace DiamondSingleDIM {
    public interface IA { int M(); }
    public interface IB : IA { int IA.M() => 42; }
    public interface IC : IA { }
    public interface ID : IB, IC { }
}
namespace DiamondConflictDIM {
    public interface IA { int M(); }
    public interface IB : IA { int IA.M() => 1; }
    public interface IC : IA { int IA.M() => 2; }
    public interface ID : IB, IC { }
}
namespace PropertyDIM {
    public interface IReadable { int Value { get; } }
    public interface IWritable : IReadable { new int Value { get; set; } int IReadable.Value => this.Value; }
}
namespace ReabstractTest {
    public interface IA { int M(); }
    public interface IB : IA { abstract int IA.M(); }
}
namespace GenericDIMTest {
    public interface IGet<T> { T Get(); }
    public interface IContainer : IGet<int>, IGet<string> { int IGet<int>.Get() => 42; }
}
        """
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8
        |> withName "DIMTestLib"

    [<FactForNETCOREAPP>]
    let ``DIM shadowing - IB-only implementation succeeds with preview`` () =
        FSharp """
module Test
open DIMTest
type C() =
    interface IB with
        member _.M() = 42
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<Fact>]
    let ``Pure F# interface hierarchy errors without DIM`` () =
        FSharp """
module Test
type IA = abstract M : int -> int
type IB = inherit IA
          abstract M : int -> int
type C() =
    interface IB with
        member x.M(y) = y + 3
"""
        |> withLangVersionPreview |> compile |> shouldFail |> withErrorCode 361

    [<FactForNETCOREAPP>]
    let ``Explicit implementation of both IA and IB works`` () =
        FSharp """
module Test
open DIMTest
type C() =
    interface IA with member _.M() = 100
    interface IB with member _.M() = 42
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Diamond with single DIM succeeds`` () =
        FSharp """
module Test
open DiamondSingleDIM
type C() = interface ID
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Diamond with conflicting DIMs errors with FS3352`` () =
        FSharp """
module Test
open DiamondConflictDIM
type C() = interface ID
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile
        |> shouldFail |> withErrorCode 3352 |> withDiagnosticMessageMatches "most specific implementation"

    [<FactForNETCOREAPP>]
    let ``Property with DIM getter succeeds`` () =
        FSharp """
module Test
open PropertyDIM
type C() =
    let mutable value = 0
    interface IWritable with
        member _.Value with get() = value and set(v) = value <- v
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Object expression with DIM succeeds`` () =
        FSharp """
module Test
open DIMTest
let obj : IB = { new IB with member _.M() = 42 }
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Object expression explicit DIM override succeeds`` () =
        FSharp """
module Test
open DIMTest
let obj : IB = { new IB with member _.M() = 42
                 interface IA with member _.M() = 100 }
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Object expression diamond with DIM succeeds`` () =
        FSharp """
module Test
open DiamondSingleDIM
let obj : ID = { new ID }
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<Fact>]
    let ``Object expression pure F# hierarchy errors`` () =
        FSharp """
module Test
type IA = abstract M : int -> int
type IB = inherit IA
          abstract M : int -> int
let obj : IB = { new IB with member x.M(y) = y + 3 }
"""
        |> withLangVersionPreview |> compile |> shouldFail |> withErrorCode 3213

    [<FactForNETCOREAPP>]
    let ``Re-abstracted member requires implementation`` () =
        FSharp """
module Test
open ReabstractTest
type C() = interface IB
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile
        |> shouldFail |> withErrorCode 366

    [<FactForNETCOREAPP>]
    let ``Re-abstracted with explicit implementation succeeds`` () =
        FSharp """
module Test
open ReabstractTest
type C() =
    interface IA with member _.M() = 42
    interface IB
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Generic interfaces partial DIM coverage`` () =
        FSharp """
module Test
open GenericDIMTest
type C() =
    interface IContainer with
        member _.Get() : string = "hello"
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Generic interfaces - missing required instantiation should fail`` () =
        FSharp """
module Test
open GenericDIMTest
type C() =
    interface IContainer
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compile
        |> shouldFail |> withErrorCode 366

    [<FactForNETCOREAPP>]
    let ``Old language version (pre-feature) requires explicit implementation`` () =
        FSharp """
module Test
open DIMTest
type C() =
    interface IB with
        member _.M() = 42
"""
        |> withLangVersion "9.0" |> withReferences [dimTestLib] |> compile
        |> shouldFail |> withErrorCode 361 |> withDiagnosticMessageMatches "implements"
