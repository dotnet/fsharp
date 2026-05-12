// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypeForwarding/Class/
// These tests verify F# works correctly with runtime C# type forwarding at runtime.
//
// Original test pattern (from env.lst):
// 1. Compile C# library with types defined directly (Class_Library.cs without FORWARD)
// 2. Compile F# exe referencing C# library
// 3. Replace C# library with forwarding version (Class_Library.cs with FORWARD + Class_Forwarder.cs as Target)
// 4. Run F# exe - should work because types are forwarded to Target.dll
//
// This file uses TypeForwardingHelpers.verifyTypeForwarding to test this pattern in-process.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module ClassTypeForwardingTests =

    // ============================================================================
    // C# source definitions (derived from original Class_Library.cs and Class_Forwarder.cs)
    // ============================================================================

    /// Original C# library with types defined directly (before type forwarding)
    let classOriginalCSharp = """
// Non-generic types
public class NormalClass
{
    public int getValue() => 0;
}

namespace N_002
{
    public class MethodParameter
    {
        public void Method(NormalClass f) { }
    }
}

namespace N_003
{
    public class Foo
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

public class TurnsToStruct
{   
    public int getValue() => 0;
}

// Basic generic types
public class Basic_Normal<T>
{
    public int getValue() => 0;
}

public class Basic_DiffNum<T>
{
    public int getValue() => 0;
}

public class Basic_DiffName<T>
{
    public int getValue() => 0;
}

public class Basic_DiffName004<T, U>
{
    public int getValue() => 0;
}

// Constraint generic types
public class Constraint_OnlyOrigin<T> where T : class
{
    public int getValue() => 0;
}

public class Constraint_OnlyForwarder<T>
{
    public int getValue() => 0;
}

public class Constraint_NonViolatedForwarder<T>
{
    public int getValue() => 0;
}

public class Constraint_Both<T> where T : class
{
    public int getValue() => 0;
}

public class Constraint_BothNonViolated<T> where T : new()
{
    public int getValue() => 0;
}

public class Constraint_BothViolated<T> where T : class
{
    public int getValue() => 0;
}

// Method generic types
public class Method_NotInForwarder<T>
{
    public int getValue() => 0;
}

public class Method_Non_Generic
{
    public int getValue<T>() => 0;
}

// Interface generic types
public class Interface_Base<T>
{
}

public class Interface_Sub<T> : Interface_Base<T>
{
    public int getValue() => 0;
}

public class TurnToInterface_Base<T>
{
    public int getValue() => 0;
}

public class TurnToInterface_Sub<T> : TurnToInterface_Base<T>
{
    public new int getValue() => 0;
}
"""

    /// Target C# library (where types are actually defined after forwarding)
    let classTargetCSharp = """
// Non-generic types
public class NormalClass
{
    public int getValue() => -1;
}

namespace N_002
{
    public class MethodParameter
    {
        public void Method(NormalClass f) { }
    }
}

namespace N_003
{
    public class Foo
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

public struct TurnsToStruct
{   
    public int getValue() => -1;
}

// Basic generic types
public class Basic_Normal<T>
{
    public int getValue() => -1;
}

public class Basic_DiffNum<T, U>
{
    public int getValue() => -1;
}

public class Basic_DiffName<U>
{
    public int getValue() => -1;
}

public class Basic_DiffName004<T, U>
{
    public int getValue() => -1;
}

// Constraint generic types
public class Constraint_OnlyOrigin<T>
{
    public int getValue() => -1;
}

public class Constraint_OnlyForwarder<T> where T : struct
{
    public int getValue() => -1;
}

public class Constraint_NonViolatedForwarder<T> where T : class
{
    public int getValue() => -1;
}

public class Constraint_Both<T> where T : class
{
    public int getValue() => -1;
}

public class Constraint_BothNonViolated<T> where T : class
{
    public int getValue() => -1;
}

public class Constraint_BothViolated<T> where T : struct
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

// Interface generic types
public interface Interface_Base<T>
{
}

public class Interface_Sub<T> : Interface_Base<T>
{
    public int getValue() => -1;
}

public interface TurnToInterface_Base<T>
{
    int getValue();
}

public class TurnToInterface_Sub<T> : TurnToInterface_Base<T>
{
    public int getValue() => -1;
}
"""

    /// Forwarder C# library (type forwarding attributes pointing to Target)
    let classForwarderCSharp = """
using System.Runtime.CompilerServices;

// Non-generic test
[assembly: TypeForwardedTo(typeof(NormalClass))]
[assembly: TypeForwardedTo(typeof(N_002.MethodParameter))]
[assembly: TypeForwardedTo(typeof(N_003.Foo))]
[assembly: TypeForwardedTo(typeof(N_003.Bar))]
[assembly: TypeForwardedTo(typeof(TurnsToStruct))]

// Basic generic type forwarding
[assembly: TypeForwardedTo(typeof(Basic_Normal<>))]
[assembly: TypeForwardedTo(typeof(Basic_DiffNum<,>))]
[assembly: TypeForwardedTo(typeof(Basic_DiffName<>))]
[assembly: TypeForwardedTo(typeof(Basic_DiffName004<,>))]

// Constraint generic type forwarding
[assembly: TypeForwardedTo(typeof(Constraint_OnlyOrigin<>))]
[assembly: TypeForwardedTo(typeof(Constraint_OnlyForwarder<>))]
[assembly: TypeForwardedTo(typeof(Constraint_NonViolatedForwarder<>))]
[assembly: TypeForwardedTo(typeof(Constraint_Both<>))]
[assembly: TypeForwardedTo(typeof(Constraint_BothNonViolated<>))]
[assembly: TypeForwardedTo(typeof(Constraint_BothViolated<>))]

// Generic class and generic method test
[assembly: TypeForwardedTo(typeof(Method_NotInForwarder<>))]
[assembly: TypeForwardedTo(typeof(Method_Non_Generic))]

// Generic interface test
[assembly: TypeForwardedTo(typeof(Interface_Base<>))]
[assembly: TypeForwardedTo(typeof(Interface_Sub<>))]
[assembly: TypeForwardedTo(typeof(TurnToInterface_Base<>))]
[assembly: TypeForwardedTo(typeof(TurnToInterface_Sub<>))]
"""

    // ============================================================================
    // NON-GENERIC TYPE FORWARDING TESTS
    // ============================================================================

    /// NG_NormalClass - Basic non-generic class type forwarding
    [<FactForNETCOREAPP>]
    let ``Class - NG_NormalClass - basic non-generic forwarding`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nc = NormalClass()
    let rv = nc.getValue()
    // After forwarding, getValue returns -1 (from Target)
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_MethodParam - Type used as method parameter
    [<FactForNETCOREAPP>]
    let ``Class - NG_MethodParam - class as method parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nc = NormalClass()
    let mp = N_002.MethodParameter()
    mp.Method(nc)
    let rv = nc.getValue()
    // After forwarding, getValue returns -1 (from Target)
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_WidenAccess - Widening access across namespaces
    [<FactForNETCOREAPP>]
    let ``Class - NG_WidenAccess - access across namespaces`` () =
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
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_TurnToStruct - Class that turns to struct causes runtime exception
    [<FactForNETCOREAPP>]
    let ``Class - NG_TurnToStruct - class turns to struct at runtime`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = TurnsToStruct()
    let rv = f.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        // This should fail at runtime because class changed to struct
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime type mismatch
        | TFSuccess _ -> failwith "Expected runtime failure when class turns to struct"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    // ============================================================================
    // BASIC GENERIC TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Basic001 - Basic generic class
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic001 - basic generic class forwarding`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_Normal<string>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Basic002 - Generic class with different number of type parameters
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic002 - different type parameter count causes runtime error`` () =
        // Original has Basic_DiffNum<T>, Target has Basic_DiffNum<T,U>
        // F# code compiled against original with one type param
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_DiffNum<string>()
    let rv = gc.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        // This should fail at runtime because type parameter count differs
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime type mismatch
        | TFSuccess _ -> failwith "Expected runtime failure when type parameter count differs"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    /// G_Basic003 - Generic class with different type parameter name
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic003 - different type parameter name`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_DiffName<string>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Basic004 - Generic class with multiple type parameters, different names
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic004 - multiple type parameters with different names`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_DiffName004<int, string>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // CONSTRAINT GENERIC TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Constraint001 - Origin has constraint, forwarded has none
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint001 - origin has constraint forwarded has none`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_OnlyOrigin<string>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Constraint002 - Origin has no constraint, forwarded has violated constraint
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint002 - forwarded has constraint origin doesn't causes runtime error`` () =
        // Original has no constraint, Target has struct constraint, but we use string (class)
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_OnlyForwarder<string>()
    let rv = gc.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        // This should fail at runtime because constraint is violated
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime constraint violation
        | TFSuccess _ -> failwith "Expected runtime failure when constraint is violated"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    /// G_Constraint003 - Origin has no constraint, forwarded has non-violated constraint
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint003 - non-violated constraint at runtime`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_NonViolatedForwarder<string>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Constraint004 - Both have same constraint
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint004 - both have same constraint`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_Both<string>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Constraint005 - Both have constraints, non-violated
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint005 - both have non-violated constraints`` () =
        let fsharpSource = """
type Test() = 
    member _.Foo() = 12

[<EntryPoint>]
let main _ =
    let gc = Constraint_BothNonViolated<Test>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Constraint006 - Both have constraints, violated at runtime
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint006 - constraint violated at runtime`` () =
        // Original has "class" constraint, Target has "struct" constraint, using string (class)
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_BothViolated<string>()
    let rv = gc.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        // This should fail at runtime because constraint is violated
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime constraint violation
        | TFSuccess _ -> failwith "Expected runtime failure when constraint is violated"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    // ============================================================================
    // INTERFACE GENERIC TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Interface001 - Forwarded class type inherits from interface base
    [<FactForNETCOREAPP>]
    let ``Class - G_Interface001 - class inheriting generic interface base`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Interface_Sub<int>()
    let rv = b.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Interface002 - Class with subtype that works after base becomes interface
    [<FactForNETCOREAPP>]
    let ``Class - G_Interface002 - subtype works after base becomes interface`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = TurnToInterface_Sub<int>()
    let rv = b.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // METHOD GENERIC TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Method001 - Forwarded type doesn't contain the method
    [<FactForNETCOREAPP>]
    let ``Class - G_Method001 - method not in forwarded type causes runtime error`` () =
        // Original has getValue(), Target has notgetValue() (renamed)
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Method_NotInForwarder<int>()
    let rv = gc.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        // This should fail at runtime because method doesn't exist
        match result with
        | TFExecutionFailure _ -> () // Expected: MissingMethodException
        | TFSuccess _ -> failwith "Expected runtime failure when method is missing"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    /// G_Method002 - Non-generic class with generic method
    [<FactForNETCOREAPP>]
    let ``Class - G_Method002 - non-generic class with generic method`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ngc = Method_Non_Generic()
    let rv = ngc.getValue<int>()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // ADDITIONAL TESTS FOR COVERAGE
    // ============================================================================

    /// Test with int type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic001 - with int type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_Normal<int>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with float type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic001 - with float type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_Normal<float>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with bool type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic001 - with bool type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_Normal<bool>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with obj type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic001 - with obj type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_Normal<obj>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName with int type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic003 - with int type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_DiffName<int>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName004 with string, int
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic004 - with string and int type parameters`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_DiffName004<string, int>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic_DiffName004 with bool, float
    [<FactForNETCOREAPP>]
    let ``Class - G_Basic004 - with bool and float type parameters`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Basic_DiffName004<bool, float>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Constraint_OnlyOrigin with obj (satisfies class constraint)
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint001 - with obj type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_OnlyOrigin<obj>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Constraint_Both with obj
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint004 - with obj type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_Both<obj>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test NonViolatedForwarder with obj
    [<FactForNETCOREAPP>]
    let ``Class - G_Constraint003 - with obj type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let gc = Constraint_NonViolatedForwarder<obj>()
    let rv = gc.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Interface_Sub with string
    [<FactForNETCOREAPP>]
    let ``Class - G_Interface001 - with string type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Interface_Sub<string>()
    let rv = b.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Interface_Sub with bool
    [<FactForNETCOREAPP>]
    let ``Class - G_Interface001 - with bool type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Interface_Sub<bool>()
    let rv = b.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Method_Non_Generic with string type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Method002 - with string type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ngc = Method_Non_Generic()
    let rv = ngc.getValue<string>()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Method_Non_Generic with bool type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Method002 - with bool type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ngc = Method_Non_Generic()
    let rv = ngc.getValue<bool>()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Method_Non_Generic with float type parameter
    [<FactForNETCOREAPP>]
    let ``Class - G_Method002 - with float type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let ngc = Method_Non_Generic()
    let rv = ngc.getValue<float>()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test creating and using NormalClass with method call
    [<FactForNETCOREAPP>]
    let ``Class - NG_NormalClass - create and call method`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nc1 = NormalClass()
    let nc2 = NormalClass()
    let sum = nc1.getValue() + nc2.getValue()
    if sum <> -2 then failwith $"Expected -2 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo and Bar interaction
    [<FactForNETCOREAPP>]
    let ``Class - NG_WidenAccess - Foo getValue returns 1`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let rv = f.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo getValue2
    [<FactForNETCOREAPP>]
    let ``Class - NG_WidenAccess - Foo getValue2 returns -2`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let rv = f.getValue2()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Bar getValue
    [<FactForNETCOREAPP>]
    let ``Class - NG_WidenAccess - Bar getValue returns -2`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = N_003.Bar()
    let rv = b.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test MethodParameter with different instances
    [<FactForNETCOREAPP>]
    let ``Class - NG_MethodParam - multiple instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nc1 = NormalClass()
    let nc2 = NormalClass()
    let mp = N_002.MethodParameter()
    mp.Method(nc1)
    mp.Method(nc2)
    let rv = nc1.getValue() + nc2.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding classOriginalCSharp classForwarderCSharp classTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed
