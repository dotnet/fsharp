// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

/// Tests for implicit DIM (Default Interface Method) slot coverage feature.
/// This feature allows F# types to implement interfaces where some slots
/// are covered by DIMs in the interface hierarchy, without explicitly
/// implementing the covered slots.
module ``DIM Slot Coverage Tests`` =

    let withCSharpLanguageVersion (ver: CSharpLanguageVersion) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | CS cs -> CS { cs with LangVersion = ver }
        | _ -> failwith "Only supported in C#"

    /// C# library defining an interface hierarchy with DIM coverage.
    /// IB inherits from IA, re-declares M(), and provides a DIM for IA.M.
    let csharpInterfaceWithDIM =
        CSharp """
namespace DIMTest
{
    public interface IA
    {
        int M();
    }
    
    public interface IB : IA
    {
        // Re-declare M with same signature (shadowing)
        new int M();
        
        // Provide default implementation for IA.M
        int IA.M() => this.M() + 100;
    }
}""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8 |> withName "DIMLib"

    /// Test 1: Simple DIM shadowing case from RFC
    /// C# interface IA with M(), IB : IA with new M() and DIM for IA.M
    /// F# type implementing IB only should compile because IA.M is covered by DIM.
    [<FactForNETCOREAPP>]
    let ``Simple DIM shadowing - implementing IB only should not require IA implementation`` () =
        let fsharpSource = """
module Test

open DIMTest

type C() =
    interface IB with
        member _.M() = 42
"""
        FSharp fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpInterfaceWithDIM]
        |> compile
        |> shouldSucceed

    /// Test 2: Pure F# interface hierarchy test (no DIM possible)
    /// This should STILL error with FS0361 to prevent regression.
    /// F# interfaces cannot have DIMs, so shadowing always needs explicit implementation.
    [<Fact>]
    let ``Pure F# interface hierarchy without DIM should still error`` () =
        let fsharpSource = """
module Test

type IA =
    abstract M : int -> int

type IB =
    inherit IA
    abstract M : int -> int

type C() =
    interface IB with
        member x.M(y) = y + 3
"""
        FSharp fsharpSource
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 361

    /// Test 3: Verify baseline behavior - explicit implementation works
    [<FactForNETCOREAPP>]
    let ``Explicit interface implementation for both IA and IB works`` () =
        let fsharpSource = """
module Test

open DIMTest

type C() =
    interface IA with
        member _.M() = 100
    interface IB with
        member _.M() = 42
"""
        FSharp fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpInterfaceWithDIM]
        |> compile
        |> shouldSucceed

    // =============================================================================
    // Edge Case Tests: Diamond Inheritance
    // =============================================================================

    /// C# library defining a diamond interface hierarchy with single DIM.
    /// IB provides a DIM for IA.M, IC does NOT provide a DIM, ID inherits both IB and IC.
    /// Since IB provides a DIM, ID should be implementable without ambiguity.
    let csharpDiamondSingleDIM =
        CSharp """
namespace DiamondSingleDIM
{
    public interface IA
    {
        int M();
    }
    
    public interface IB : IA
    {
        // Provide default implementation for IA.M
        int IA.M() => 42;
    }
    
    public interface IC : IA
    {
        // No DIM for IA.M - just inherits it
    }
    
    public interface ID : IB, IC
    {
        // ID inherits IA.M from both IB and IC paths
        // IB has a DIM for IA.M, so ID should be implementable
    }
}""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8 |> withName "DiamondSingleDIMLib"

    /// Test 4: Diamond with single DIM - IB provides DIM, IC does not
    /// Implementing ID should work because IB provides DIM coverage for IA.M
    [<FactForNETCOREAPP>]
    let ``Diamond with single DIM - should work because IB provides DIM`` () =
        let fsharpSource = """
module Test

open DiamondSingleDIM

type C() =
    interface ID
"""
        FSharp fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpDiamondSingleDIM]
        |> compile
        |> shouldSucceed

    /// C# library defining a diamond interface hierarchy with conflicting DIMs.
    /// IB provides DIM1 for IA.M, IC provides DIM2 for IA.M, ID inherits both.
    /// This should still error because there's no most-specific implementation.
    let csharpDiamondConflictingDIMs =
        CSharp """
namespace DiamondConflictDIM
{
    public interface IA
    {
        int M();
    }
    
    public interface IB : IA
    {
        // Provide default implementation for IA.M
        int IA.M() => 1;
    }
    
    public interface IC : IA
    {
        // Provide DIFFERENT default implementation for IA.M
        int IA.M() => 2;
    }
    
    public interface ID : IB, IC
    {
        // ID inherits conflicting DIMs from IB and IC
        // This creates ambiguity - no most-specific implementation
    }
}""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8 |> withName "DiamondConflictDIMLib"

    /// Test 5: Diamond with conflicting DIMs - IB and IC both provide different DIMs
    /// Implementing ID should fail because there's no most-specific implementation
    [<FactForNETCOREAPP>]
    let ``Diamond with conflicting DIMs - should error with no most-specific`` () =
        let fsharpSource = """
module Test

open DiamondConflictDIM

type C() =
    interface ID
"""
        FSharp fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpDiamondConflictingDIMs]
        |> compile
        |> shouldFail
        |> withErrorCode 366  // FS0366: No implementation was given for interface member

    // =============================================================================
    // Edge Case Tests: Properties with DIM
    // =============================================================================

    /// C# library defining an interface with a property that has a DIM getter.
    /// IReadable has a Value getter, IWritable extends with getter+setter and DIM for getter.
    let csharpPropertyWithDIMGetter =
        CSharp """
namespace PropertyDIM
{
    public interface IReadable
    {
        int Value { get; }
    }
    
    public interface IWritable : IReadable
    {
        // New property with getter and setter
        new int Value { get; set; }
        
        // Provide DIM for the IReadable.Value getter
        int IReadable.Value => this.Value;
    }
}""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8 |> withName "PropertyDIMLib"

    /// Test 6: Property with DIM getter
    /// Implementing IWritable should work because the IReadable.Value getter is covered by DIM
    [<FactForNETCOREAPP>]
    let ``Property with DIM getter - should work`` () =
        let fsharpSource = """
module Test

open PropertyDIM

type C() =
    let mutable value = 0
    interface IWritable with
        member _.Value with get() = value and set(v) = value <- v
"""
        FSharp fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpPropertyWithDIMGetter]
        |> compile
        |> shouldSucceed
