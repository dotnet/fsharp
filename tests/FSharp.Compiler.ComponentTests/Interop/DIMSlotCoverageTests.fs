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

    // Helper: Generate F# code that opens a namespace and implements an interface
    let fsharpImplementingInterface ns typeBody =
        FSharp $"""
module Test
open {ns}
{typeBody}
"""

    // Helper: Compile with DIM support (preview + reference to C# lib) and expect success
    let shouldCompileWithDIM libRef source =
        source |> withLangVersionPreview |> withReferences [libRef] |> compile |> shouldSucceed

    // Helper: Compile with DIM support and expect failure with specific error code
    let shouldFailWithDIM libRef errorCode source =
        source |> withLangVersionPreview |> withReferences [libRef] |> compile |> shouldFail |> withErrorCode errorCode

    // Helper: Compile with old language version and expect failure (tests feature gating)
    let shouldFailWithoutFeature libRef langVersion errorCode source =
        source |> withLangVersion langVersion |> withReferences [libRef] |> compile |> shouldFail |> withErrorCode errorCode

    [<FactForNETCOREAPP>]
    let ``DIM shadowing - IB-only implementation succeeds with preview`` () =
        fsharpImplementingInterface "DIMTest" "type C() = interface IB with member _.M() = 42"
        |> shouldCompileWithDIM dimTestLib

    [<Fact>]
    let ``Pure F# interface hierarchy errors without DIM`` () =
        FSharp """
module Test
type IA = abstract M : int -> int
type IB = inherit IA
          abstract M : int -> int
type C() = interface IB with member x.M(y) = y + 3
"""
        |> withLangVersionPreview |> compile |> shouldFail |> withErrorCode 361

    [<FactForNETCOREAPP>]
    let ``Explicit implementation of both IA and IB works`` () =
        fsharpImplementingInterface "DIMTest" """type C() =
    interface IA with member _.M() = 100
    interface IB with member _.M() = 42"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Diamond with single DIM succeeds`` () =
        fsharpImplementingInterface "DiamondSingleDIM" "type C() = interface ID"
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Diamond with conflicting DIMs errors with FS3352`` () =
        fsharpImplementingInterface "DiamondConflictDIM" "type C() = interface ID"
        |> shouldFailWithDIM dimTestLib 3352
        |> withDiagnosticMessageMatches "most specific implementation"

    [<FactForNETCOREAPP>]
    let ``Property with DIM getter succeeds`` () =
        fsharpImplementingInterface "PropertyDIM" """type C() =
    let mutable value = 0
    interface IWritable with member _.Value with get() = value and set(v) = value <- v"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Object expression with DIM succeeds`` () =
        fsharpImplementingInterface "DIMTest" "let obj : IB = { new IB with member _.M() = 42 }"
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Object expression explicit DIM override succeeds`` () =
        fsharpImplementingInterface "DIMTest" """let obj : IB = { new IB with member _.M() = 42
                 interface IA with member _.M() = 100 }"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Object expression diamond with DIM succeeds`` () =
        fsharpImplementingInterface "DiamondSingleDIM" "let obj : ID = { new ID }"
        |> shouldCompileWithDIM dimTestLib

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
        fsharpImplementingInterface "ReabstractTest" "type C() = interface IB"
        |> shouldFailWithDIM dimTestLib 366

    [<FactForNETCOREAPP>]
    let ``Re-abstracted with explicit implementation succeeds`` () =
        fsharpImplementingInterface "ReabstractTest" """type C() =
    interface IA with member _.M() = 42
    interface IB"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Generic interfaces partial DIM coverage`` () =
        fsharpImplementingInterface "GenericDIMTest" """type C() =
    interface IContainer with member _.Get() : string = "hello" """
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Generic interfaces - missing required instantiation should fail`` () =
        fsharpImplementingInterface "GenericDIMTest" "type C() = interface IContainer"
        |> shouldFailWithDIM dimTestLib 366

    [<FactForNETCOREAPP>]
    let ``Old language version (pre-feature) requires explicit implementation`` () =
        fsharpImplementingInterface "DIMTest" "type C() = interface IB with member _.M() = 42"
        |> shouldFailWithoutFeature dimTestLib "9.0" 361
        |> withDiagnosticMessageMatches "implements"
