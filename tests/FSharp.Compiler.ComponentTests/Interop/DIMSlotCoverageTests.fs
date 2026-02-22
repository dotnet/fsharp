// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

/// DIM (Default Interface Method) slot coverage tests for F# interop with C# 8+ interfaces.
module ``DIM Slot Coverage Tests`` =

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
namespace IndependentDIMs {
    public interface IX { int X(); }
    public interface IY : IX { new int X(); int IX.X() => this.X() + 1000; }
    public interface IA { int A(); }
    public interface IB : IA { new int A(); int IA.A() => this.A() + 2000; }
}
namespace BaseClassDIM {
    public interface IA { int M(); }
    public interface IB : IA { new int M(); int IA.M() => this.M() + 100; }
    public class CSharpBase : IB { public int M() => 77; }
}
namespace MixedDIM {
    public interface IMixed {
        int Covered() => 1;
        int NotCovered();
    }
}
namespace RootDIM {
    public interface IRoot {
        int M() => 1;
    }
}
namespace ChainDIM {
    public interface IA { int M() => 1; }
    public interface IB : IA { }
    public interface IC : IB { }
}
namespace PartialDIMCoverage {
    public interface IA { int M(); }
    public interface IB : IA { int IA.M() => this.M(); new int M(); }
    public interface IC { int M(); }
    public interface ID : IB, IC { }
}
namespace ExplicitIADIM {
    public interface IA { int M(); }
    public interface IB : IA { new int M(); int IA.M() => this.M() + 100; }
}
namespace SealedDIM {
    public interface IA { sealed int M() => 42; }
    public interface IB : IA { }
}
namespace EventDIM {
    public interface IA {
        event System.EventHandler E;
    }
    public interface IB : IA {
        event System.EventHandler E;
        event System.EventHandler IA.E {
            add { this.E += value; }
            remove { this.E -= value; }
        }
    }
}
        """
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8
        |> withName "DIMTestLib"

    let fsharpImplementingInterface ns typeBody =
        FSharp $"""
module Test
open {ns}
{typeBody}
"""

    let shouldCompileWithDIM libRef source =
        source |> withLangVersionPreview |> withReferences [libRef] |> compile |> shouldSucceed

    let shouldFailWithDIM libRef errorCode source =
        source |> withLangVersionPreview |> withReferences [libRef] |> compile |> shouldFail |> withErrorCode errorCode

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
    let ``Object expression - re-abstracted member requires implementation`` () =
        fsharpImplementingInterface "ReabstractTest" "let obj : IB = { new IB }"
        |> shouldFailWithDIM dimTestLib 366

    [<FactForNETCOREAPP>]
    let ``Object expression - re-abstracted with explicit implementation succeeds`` () =
        fsharpImplementingInterface "ReabstractTest" """let obj : IB = { new IB
                                                                        interface IA with member _.M() = 42 }"""
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
        |> shouldFailWithoutFeature dimTestLib "10.0" 361
        |> withDiagnosticMessageMatches "more than one abstract slot"

    [<FactForNETCOREAPP>]
    let ``Runtime - DIM forwarding calls correct method`` () =
        fsharpImplementingInterface "DIMTest" """
type C() =
    interface IB with member _.M() = 42

[<EntryPoint>]
let main _ =
    let c = C()
    let ibResult = (c :> IB).M()
    let iaResult = (c :> IA).M()
    if ibResult <> 42 then failwithf "IB.M() expected 42 but got %d" ibResult
    if iaResult <> 142 then failwithf "IA.M() via DIM expected 142 but got %d" iaResult
    0
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compileExeAndRun |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Runtime - explicit override of DIM-covered slot`` () =
        fsharpImplementingInterface "DIMTest" """
type C() =
    interface IB with member _.M() = 42
    interface IA with member _.M() = 999

[<EntryPoint>]
let main _ =
    let c = C()
    let ibResult = (c :> IB).M()
    let iaResult = (c :> IA).M()
    if ibResult <> 42 then failwithf "IB.M() expected 42 but got %d" ibResult
    if iaResult <> 999 then failwithf "IA.M() explicit override expected 999 but got %d" iaResult
    0
"""
        |> withLangVersionPreview |> withReferences [dimTestLib] |> compileExeAndRun |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Struct implementing DIM-covered interface`` () =
        fsharpImplementingInterface "DIMTest" """
[<Struct>]
type S =
    interface IB with member _.M() = 42
"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Abstract class with DIM-covered interface`` () =
        fsharpImplementingInterface "DIMTest" """
[<AbstractClass>]
type A() =
    interface IB with member _.M() = 42
"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Multiple independent DIM-covered interfaces`` () =
        fsharpImplementingInterface "IndependentDIMs" """
type C() =
    interface IY with member _.X() = 10
    interface IB with member _.A() = 20
"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Base class inheritance - F# class inheriting C# class with DIM`` () =
        fsharpImplementingInterface "BaseClassDIM" """
type Derived() =
    inherit CSharpBase()
"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Mixed DIM - must implement non-covered method`` () =
        fsharpImplementingInterface "MixedDIM" "type C() = interface IMixed"
        |> shouldFailWithDIM dimTestLib 366

    [<FactForNETCOREAPP>]
    let ``Mixed DIM - implementation of non-covered method suffices`` () =
        fsharpImplementingInterface "MixedDIM" "type C() = interface IMixed with member _.NotCovered() = 2"
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Root DIM - no implementation needed`` () =
        fsharpImplementingInterface "RootDIM" "type C() = interface IRoot"
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Chain DIM - deep inheritance picks up DIM`` () =
        fsharpImplementingInterface "ChainDIM" "type C() = interface IC"
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Class with own abstract member and DIM interface`` () =
        fsharpImplementingInterface "RootDIM" """
[<AbstractClass>]
type C() =
   interface IRoot
   abstract member MyAbstract : unit -> unit
"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Partial DIM filtering - residual FS0361 when uncovered slots remain`` () =
        fsharpImplementingInterface "PartialDIMCoverage" "type C() = interface ID with member _.M() = 42"
        |> shouldFailWithDIM dimTestLib 361

    [<FactForNETCOREAPP>]
    let ``Explicit IA declaration without providing IA.M errors`` () =
        fsharpImplementingInterface "ExplicitIADIM" """type C() =
    interface IB with member _.M() = 42
    interface IA"""
        |> shouldFailWithDIM dimTestLib 366

    [<FactForNETCOREAPP>]
    let ``Sealed DIM method - implementing derived interface succeeds without override`` () =
        fsharpImplementingInterface "SealedDIM" "type C() = interface IB"
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Event with DIM coverage - IB-only implementation succeeds`` () =
        fsharpImplementingInterface "EventDIM" """type C() =
    let e = Event<System.EventHandler, System.EventArgs>()
    interface IB with
        [<CLIEvent>]
        member _.E = e.Publish"""
        |> shouldCompileWithDIM dimTestLib

    [<FactForNETCOREAPP>]
    let ``Object expression - diamond with conflicting DIMs errors with FS3352`` () =
        fsharpImplementingInterface "DiamondConflictDIM" "let obj : ID = { new ID }"
        |> shouldFailWithDIM dimTestLib 3352
        |> withDiagnosticMessageMatches "most specific implementation"

    [<FactForNETCOREAPP>]
    let ``Diamond conflict resolved via explicit IA override`` () =
        fsharpImplementingInterface "DiamondConflictDIM" """type C() =
    interface ID
    interface IA with member _.M() = 99"""
        |> shouldCompileWithDIM dimTestLib

