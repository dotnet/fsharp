// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

/// DIM (Default Interface Method) slot coverage tests for F# interop with C# 8+ interfaces.
///
/// Testing Dimensions (8):
///   1. DIM availability: C# interface with DIM vs pure F# interface hierarchy
///   2. Construct type: F# class vs object expression implementing interfaces
///   3. Hierarchy shape: Linear (IA->IB) vs diamond (IA->IB,IC->ID) inheritance
///   4. DIM conflict: Single unambiguous DIM vs conflicting DIMs requiring resolution (FS3352)
///   5. Member type: Methods vs properties (with DIM getters/setters)
///   6. Generics: Generic interface instantiation with partial DIM coverage
///   7. Re-abstraction: C# "abstract" DIM forcing F# to provide implementation
///   8. Language version: Preview feature gating (DIM support requires --langversion:preview)
///
/// Why 3-level type depth suffices: Diamond inheritance (IA->IB,IC->ID) is the maximal
/// complexity for DIM resolution—the compiler must find the "most specific" implementation.
/// Deeper hierarchies don't introduce new DIM behaviors; they only repeat the same patterns.
///
/// Test Coverage by Dimension:
///   DIM availability     → Tests 1-2 (DIM shadowing vs pure F# error)
///   Construct type       → Tests 7-10 (class tests 1-6, object expression tests 7-10)
///   Hierarchy shape      → Tests 4-5 (diamond single DIM, diamond conflict)
///   DIM conflict         → Test 5 (FS3352 "most specific implementation")
///   Member type          → Test 6 (property with DIM getter)
///   Generics             → Tests 13-14 (partial coverage, missing instantiation)
///   Re-abstraction       → Tests 11-12 (requires impl, explicit impl succeeds)
///   Language version     → Test 15 (pre-feature version errors with FS0361)
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
