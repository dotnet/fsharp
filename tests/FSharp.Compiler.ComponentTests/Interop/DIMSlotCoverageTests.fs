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
    /// SPRINT 1: This test expects FAILURE until the ImplicitDIMCoverage feature is implemented.
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
        |> shouldFail
        |> withErrorCode 361

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
