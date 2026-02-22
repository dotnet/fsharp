// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypeForwarding/Struct/
// These tests verify F# works correctly with runtime C# type forwarding for structs.
//
// Original test pattern (from env.lst):
// 1. Compile C# library with struct types defined directly (Struct_Library.cs without FORWARD)
// 2. Compile F# exe referencing C# library
// 3. Replace C# library with forwarding version (Struct_Library.cs with FORWARD + Struct_Forwarder.cs as Target)
// 4. Run F# exe - should work because types are forwarded to Target.dll
//
// This file uses TypeForwardingHelpers.verifyTypeForwarding to test this pattern in-process.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module StructTypeForwardingTests =

    // ============================================================================
    // C# source definitions (derived from original Struct_Library.cs and Struct_Forwarder.cs)
    // ============================================================================

    /// Original C# library with struct types defined directly (before type forwarding)
    let structOriginalCSharp = """
// Non-generic struct types
public struct NormalStruct
{
    public int getValue() => 0;
}

namespace N_002
{
    public struct MethodParameter
    {
        public void Method(NormalStruct f) { }
    }
}

namespace N_003
{
    public struct Foo
    {
        public int getValue() => 1;
        public int getValue2() => -1;
    }
    
    public class Bar
    {
        public int getValue()
        {
            Foo f = new Foo();
            return f.getValue2();
        }
    }
}

public struct TurnsToClass
{
    public int getValue() => 0;
}

// Basic generic struct types
public struct Basic_Normal<T>
{
    public int getValue() => 0;
}

public struct Basic_DiffNum<T>
{
    public int getValue() => 0;
}

public struct Basic_DiffName<T>
{
    public int getValue() => 0;
}

public struct Basic_DiffName004<T, U>
{
    public int getValue() => 0;
}

// Constraint generic struct types
public struct Constraint_OnlyOrigin<T> where T : struct
{
    public int getValue() => 0;
}

public struct Constraint_OnlyForwarder<T>
{
    public int getValue() => 0;
}

public struct Constraint_Both<T> where T : struct
{
    public int getValue() => 0;
}

public struct Constraint_BothViolated<T> where T : class
{
    public int getValue() => 0;
}

// Method generic struct types
public struct Method_NotInForwarder<T>
{
    public int getValue() => 0;
}

public struct Method_Non_Generic
{
    public int getValue<T>() => 0;
}
"""

    /// Target C# library (where struct types are actually defined after forwarding)
    let structTargetCSharp = """
// Non-generic struct types
public struct NormalStruct
{
    public int getValue() => -1;
}

namespace N_002
{
    public struct MethodParameter
    {
        public void Method(NormalStruct f) { }
    }
}

namespace N_003
{
    public struct Foo
    {
        public int getValue() => 1;
        public int getValue2() => -2;
    }
    
    public class Bar
    {
        public int getValue()
        {
            Foo f = new Foo();
            return f.getValue2();
        }
    }
}

// TurnsToClass becomes a class in the target
public class TurnsToClass
{
    public int getValue() => -1;
}

// Basic generic struct types
public struct Basic_Normal<T>
{
    public int getValue() => -1;
}

public struct Basic_DiffNum<T, U>
{
    public int getValue() => -1;
}

public struct Basic_DiffName<U>
{
    public int getValue() => -1;
}

public struct Basic_DiffName004<T, U>
{
    public int getValue() => -1;
}

// Constraint generic struct types
public struct Constraint_OnlyOrigin<T>
{
    public int getValue() => -1;
}

public struct Constraint_OnlyForwarder<T> where T : struct
{
    public int getValue() => -1;
}

public struct Constraint_Both<T> where T : struct
{
    public int getValue() => -1;
}

public struct Constraint_BothViolated<T> where T : struct
{
    public int getValue() => -1;
}

// Method generic types (Method_NotInForwarder has method renamed to notgetValue in target)
public class Method_NotInForwarder<T>
{
    public int notgetValue() => -1;
}

public class Method_Non_Generic
{
    public int getValue<T>() => -1;
}
"""

    /// Forwarder C# library (type forwarding attributes pointing to Target)
    let structForwarderCSharp = """
using System.Runtime.CompilerServices;

// Non-generic struct forwarding
[assembly: TypeForwardedTo(typeof(NormalStruct))]
[assembly: TypeForwardedTo(typeof(N_002.MethodParameter))]
[assembly: TypeForwardedTo(typeof(N_003.Foo))]
[assembly: TypeForwardedTo(typeof(N_003.Bar))]
[assembly: TypeForwardedTo(typeof(TurnsToClass))]

// Basic generic struct forwarding
[assembly: TypeForwardedTo(typeof(Basic_Normal<>))]
[assembly: TypeForwardedTo(typeof(Basic_DiffNum<,>))]
[assembly: TypeForwardedTo(typeof(Basic_DiffName<>))]
[assembly: TypeForwardedTo(typeof(Basic_DiffName004<,>))]

// Constraint generic struct forwarding
[assembly: TypeForwardedTo(typeof(Constraint_OnlyOrigin<>))]
[assembly: TypeForwardedTo(typeof(Constraint_OnlyForwarder<>))]
[assembly: TypeForwardedTo(typeof(Constraint_Both<>))]
[assembly: TypeForwardedTo(typeof(Constraint_BothViolated<>))]

// Generic struct and generic method test
[assembly: TypeForwardedTo(typeof(Method_NotInForwarder<>))]
[assembly: TypeForwardedTo(typeof(Method_Non_Generic))]
"""

    // ============================================================================
    // NON-GENERIC STRUCT TYPE FORWARDING TESTS
    // ============================================================================

    /// NG_NormalStruct - Basic non-generic struct type forwarding
    [<FactForNETCOREAPP>]
    let ``Struct - NG_NormalStruct - basic non-generic forwarding`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ns = NormalStruct()
    let rv = ns.getValue()
    // After forwarding, getValue returns -1 (from Target)
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_MethodParam - Struct used as method parameter
    [<FactForNETCOREAPP>]
    let ``Struct - NG_MethodParam - struct as method parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ns = NormalStruct()
    let mp = N_002.MethodParameter()
    mp.Method(ns)
    let rv = ns.getValue()
    // After forwarding, getValue returns -1 (from Target)
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_WidenAccess - Widening access across namespaces
    [<FactForNETCOREAPP>]
    let ``Struct - NG_WidenAccess - access across namespaces`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let b = N_003.Bar()
    let rv = f.getValue() + b.getValue()
    // f.getValue() = 1, b.getValue() calls Foo.getValue2() = -2, so rv = 1 + (-2) = -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_TurnToClass - Struct that turns to class causes runtime exception
    [<FactForNETCOREAPP>]
    let ``Struct - NG_TurnToClass - struct turns to class at runtime`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = TurnsToClass()
    let rv = f.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        // This should fail at runtime because struct changed to class
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime type mismatch
        | TFSuccess _ -> failwith "Expected runtime failure when struct turns to class"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    // ============================================================================
    // BASIC GENERIC STRUCT TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Basic001 - Basic generic struct
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic001 - basic generic struct forwarding`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_Normal<string>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Basic002 - Generic struct with different number of type parameters
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic002 - different type parameter count causes runtime error`` () =
        // Original has Basic_DiffNum<T>, Target has Basic_DiffNum<T,U>
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffNum<string>()
    let rv = gs.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        // This should fail at runtime because type parameter count differs
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime type mismatch
        | TFSuccess _ -> failwith "Expected runtime failure when type parameter count differs"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    /// G_Basic003 - Generic struct with different type parameter name
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic003 - different type parameter name`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffName<string>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Basic004 - Generic struct with multiple type parameters
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic004 - multiple type parameters with different names`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffName004<int, string>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // CONSTRAINT GENERIC STRUCT TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Constraint001 - Origin has constraint, forwarded has none
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint001 - origin has constraint forwarded has none`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Constraint_OnlyOrigin<System.Guid>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Constraint002 - Origin has no constraint, forwarded has constraint
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint002 - forwarded has constraint origin doesn't causes runtime error`` () =
        // Original has no constraint, Target has struct constraint, but we use string (class)
        // This should be able to compile against original but fail at runtime
        let fsharpSource = """
type Test() = 
    member _.Foo() = 12

[<EntryPoint>]
let main _ =
    let gs = Constraint_OnlyForwarder<Test>()
    let rv = gs.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        // This should fail at runtime because constraint is violated
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime constraint violation
        | TFSuccess _ -> failwith "Expected runtime failure when constraint is violated"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    /// G_Constraint003 - Both have same struct constraint
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint003 - both have same constraint`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Constraint_Both<System.Guid>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Constraint004 - Original class constraint, forwarded struct constraint, violated
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint004 - constraint violated at runtime`` () =
        // Original has "class" constraint, Target has "struct" constraint
        // We use a class type which satisfies original but not forwarded
        // However, the F# code uses Test which is a class, so it won't compile
        // Let's test what happens with a value that would fail at runtime
        let fsharpSource = """
type Test() = 
    member _.Foo() = 12

[<EntryPoint>]
let main _ =
    let gs = Constraint_BothViolated<Test>()
    let rv = gs.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        // This should fail at runtime because constraint is violated
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime constraint violation
        | TFSuccess _ -> failwith "Expected runtime failure when constraint is violated"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    // ============================================================================
    // METHOD GENERIC STRUCT TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Method001 - Forwarded struct type doesn't contain the method
    [<FactForNETCOREAPP>]
    let ``Struct - G_Method001 - method not in forwarded type causes runtime error`` () =
        // Original has getValue(), Target has notgetValue() (renamed), and Target is class not struct
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Method_NotInForwarder<int>()
    let rv = gs.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        // This should fail at runtime because method doesn't exist or type mismatch
        match result with
        | TFExecutionFailure _ -> () // Expected: MissingMethodException or type mismatch
        | TFSuccess _ -> failwith "Expected runtime failure when method is missing or type changes"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    /// G_Method002 - Non-generic struct with generic method
    [<FactForNETCOREAPP>]
    let ``Struct - G_Method002 - non-generic struct with generic method causes runtime error`` () =
        // In the struct case, Method_Non_Generic changes from struct to class
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ngs = Method_Non_Generic()
    let rv = ngs.getValue<int>()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        // This should fail at runtime because struct changed to class
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime type mismatch
        | TFSuccess _ -> failwith "Expected runtime failure when struct turns to class"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    // ============================================================================
    // ADDITIONAL TESTS FOR COVERAGE
    // ============================================================================

    /// Test with int type parameter
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic001 - with int type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_Normal<int>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with float type parameter
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic001 - with float type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_Normal<float>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with bool type parameter
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic001 - with bool type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_Normal<bool>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with obj type parameter
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic001 - with obj type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_Normal<obj>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName with int type parameter
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic003 - with int type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffName<int>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName004 with string, int
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic004 - with string and int type parameters`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffName004<string, int>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName004 with bool, float
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic004 - with bool and float type parameters`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffName004<bool, float>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Constraint_OnlyOrigin with int (satisfies struct constraint)
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint001 - with int type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Constraint_OnlyOrigin<int>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Constraint_Both with int
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint003 - with int type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Constraint_Both<int>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Constraint_Both with bool
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint003 - with bool type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Constraint_Both<bool>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test creating and using NormalStruct with method call
    [<FactForNETCOREAPP>]
    let ``Struct - NG_NormalStruct - create and call method`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ns1 = NormalStruct()
    let ns2 = NormalStruct()
    let sum = ns1.getValue() + ns2.getValue()
    if sum <> -2 then failwith $"Expected -2 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo and Bar interaction
    [<FactForNETCOREAPP>]
    let ``Struct - NG_WidenAccess - Foo getValue returns 1`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let rv = f.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo getValue2
    [<FactForNETCOREAPP>]
    let ``Struct - NG_WidenAccess - Foo getValue2 returns -2`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let rv = f.getValue2()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Bar getValue
    [<FactForNETCOREAPP>]
    let ``Struct - NG_WidenAccess - Bar getValue returns -2`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = N_003.Bar()
    let rv = b.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test MethodParameter with different instances
    [<FactForNETCOREAPP>]
    let ``Struct - NG_MethodParam - multiple instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ns1 = NormalStruct()
    let ns2 = NormalStruct()
    let mp = N_002.MethodParameter()
    mp.Method(ns1)
    mp.Method(ns2)
    let rv = ns1.getValue() + ns2.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Constraint_OnlyOrigin with byte (satisfies struct constraint)
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint001 - with byte type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Constraint_OnlyOrigin<byte>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Constraint_Both with decimal
    [<FactForNETCOREAPP>]
    let ``Struct - G_Constraint003 - with decimal type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Constraint_Both<decimal>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_Normal with System.DateTime
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic001 - with DateTime type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_Normal<System.DateTime>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_Normal with TimeSpan
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic001 - with TimeSpan type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_Normal<System.TimeSpan>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName004 with obj, string
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic004 - with obj and string type parameters`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffName004<obj, string>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName with obj type parameter
    [<FactForNETCOREAPP>]
    let ``Struct - G_Basic003 - with obj type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gs = Basic_DiffName<obj>()
    let rv = gs.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding structOriginalCSharp structForwarderCSharp structTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed
