// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypeForwarding/Delegate/
// These tests verify F# works correctly with runtime C# type forwarding for delegates.
//
// Original test pattern (from env.lst):
// 1. Compile C# library with delegate types defined directly (Delegate_Library.cs without FORWARD)
// 2. Compile F# exe referencing C# library
// 3. Replace C# library with forwarding version (Delegate_Library.cs with FORWARD + Delegate_Forwarder.cs as Target)
// 4. Run F# exe - should work because types are forwarded to Target.dll
//
// This file uses TypeForwardingHelpers.verifyTypeForwarding to test this pattern in-process.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module DelegateTypeForwardingTests =

    // ============================================================================
    // C# source definitions (derived from original Delegate_Library.cs and Delegate_Forwarder.cs)
    // ============================================================================

    /// Original C# library with delegate types defined directly (before type forwarding)
    let delegateOriginalCSharp = """
// Non-generic delegates
public delegate int DeleNormalDelegate();

namespace N_003
{
    internal delegate int DFoo();
}

public delegate int DeleTurnsToClass();

// Generic delegates
public delegate int Basic001_GDele<T>(T t);
public delegate int Basic002_GDele<T>(T t);
public delegate int Basic003_GDele<T>(T t);

// Support classes
public struct NormalDelegate
{
    public int getValue() => 0;
}

namespace N_002
{
    public struct MethodParameter
    {
        public int Method(DeleNormalDelegate dele)
        {
            return dele();
        }
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

// Classes with methods for delegates
public class Basic001_Class
{
    public int getValue<T>(T t) => 0;
}

public class Basic002_Class
{
    public int getValue<T>(T t) => 0;
}

public class Basic003_Class
{
    public int getValue<T>(T t) => 0;
}
"""

    /// Target C# library (where delegate types are actually defined after forwarding)
    let delegateTargetCSharp = """
// Non-generic delegates
public delegate int DeleNormalDelegate();

namespace N_003
{
    internal delegate int DFoo();
}

// DeleTurnsToClass becomes a class in the target
public class DeleTurnsToClass
{
    public int getValue() => -1;
}

// Generic delegates (unchanged)
public delegate int Basic001_GDele<T>(T t);
public delegate int Basic002_GDele<T>(T t);
public delegate int Basic003_GDele<T>(T t);

// Support classes with updated return values
public struct NormalDelegate
{
    public int getValue() => -1;
}

namespace N_002
{
    public struct MethodParameter
    {
        public int Method(DeleNormalDelegate dele)
        {
            return dele();
        }
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

public class TurnsToClass
{
    public int getValue() => -1;
}

// Classes with methods for delegates (return 0 for compatibility)
public class Basic001_Class
{
    public int getValue<T>(T t) => 0;
}

public class Basic002_Class
{
    public int getValue<T>(T t) => 0;
}

public class Basic003_Class
{
    public int getValue<T>(T t) => 0;
}
"""

    /// Forwarder C# library (type forwarding attributes pointing to Target)
    let delegateForwarderCSharp = """
using System.Runtime.CompilerServices;

// Non-generic delegate forwarding
[assembly: TypeForwardedTo(typeof(DeleNormalDelegate))]
[assembly: TypeForwardedTo(typeof(DeleTurnsToClass))]

// Generic delegate forwarding
[assembly: TypeForwardedTo(typeof(Basic001_GDele<>))]
[assembly: TypeForwardedTo(typeof(Basic002_GDele<>))]
[assembly: TypeForwardedTo(typeof(Basic003_GDele<>))]

// Support class forwarding
[assembly: TypeForwardedTo(typeof(NormalDelegate))]
[assembly: TypeForwardedTo(typeof(N_002.MethodParameter))]
[assembly: TypeForwardedTo(typeof(N_003.Foo))]
[assembly: TypeForwardedTo(typeof(N_003.Bar))]
[assembly: TypeForwardedTo(typeof(TurnsToClass))]
[assembly: TypeForwardedTo(typeof(Basic001_Class))]
[assembly: TypeForwardedTo(typeof(Basic002_Class))]
[assembly: TypeForwardedTo(typeof(Basic003_Class))]
"""

    // ============================================================================
    // NON-GENERIC DELEGATE TYPE FORWARDING TESTS
    // ============================================================================

    /// NG_NormalDelegate - Basic non-generic delegate type forwarding
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_NormalDelegate - basic non-generic forwarding`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nd = NormalDelegate()
    let dele = DeleNormalDelegate(nd.getValue)
    let rv = dele.Invoke()
    // After forwarding, NormalDelegate.getValue returns -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_MethodParam - Delegate used as method parameter
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_MethodParam - delegate as method parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nd = NormalDelegate()
    let dele = DeleNormalDelegate(nd.getValue)
    let mp = N_002.MethodParameter()
    let rv = mp.Method(dele)
    // After forwarding, NormalDelegate.getValue returns -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_WidenAccess - Widening access across namespaces
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_WidenAccess - access across namespaces`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let b = N_003.Bar()
    let rv = f.getValue() + b.getValue()
    // f.getValue() = 1, b.getValue() calls Foo.getValue2() = -2
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// NG_TurnToClass - Struct that turns to class
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_TurnToClass - struct turns to class at runtime`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = TurnsToClass()
    let rv = f.getValue()
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        // This should fail at runtime because struct changed to class
        match result with
        | TFExecutionFailure _ -> () // Expected: runtime type mismatch
        | TFSuccess _ -> failwith "Expected runtime failure when struct turns to class"
        | TFCompilationFailure (stage, _) -> failwith $"Unexpected compilation failure at {stage}"

    // ============================================================================
    // GENERIC DELEGATE TYPE FORWARDING TESTS
    // ============================================================================

    /// G_Basic001 - Basic generic delegate forwarding
    /// From original G_Basic001.fs: Tests basic functionality of the generic type forwarder attribute
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic001 - basic generic delegate forwarding`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic001_Class()
    let gd = Basic001_GDele<int>(c.getValue)
    let rv = gd.Invoke(1)
    // Basic001_Class.getValue returns 0
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Basic002 - Generic delegate forwarding with different class
    /// From original G_Basic002.fs
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic002 - generic delegate with Basic002_Class`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic002_Class()
    let gd = Basic002_GDele<int>(c.getValue)
    let rv = gd.Invoke(1)
    // Basic002_Class.getValue returns 0
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// G_Basic003 - Generic delegate forwarding with Basic003_Class
    /// From original G_Basic003.fs
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic003 - generic delegate with Basic003_Class`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic003_Class()
    let gd = Basic003_GDele<int>(c.getValue)
    let rv = gd.Invoke(1)
    // Basic003_Class.getValue returns 0
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // ADDITIONAL COVERAGE TESTS
    // ============================================================================

    /// Test with string type parameter
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic001 - with string type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic001_Class()
    let gd = Basic001_GDele<string>(c.getValue)
    let rv = gd.Invoke("test")
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with float type parameter
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic001 - with float type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic001_Class()
    let gd = Basic001_GDele<float>(c.getValue)
    let rv = gd.Invoke(1.5)
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with bool type parameter
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic001 - with bool type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic001_Class()
    let gd = Basic001_GDele<bool>(c.getValue)
    let rv = gd.Invoke(true)
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test with obj type parameter
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic001 - with obj type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic001_Class()
    let gd = Basic001_GDele<obj>(c.getValue)
    let rv = gd.Invoke(box 42)
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic002_GDele with string
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic002 - with string type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic002_Class()
    let gd = Basic002_GDele<string>(c.getValue)
    let rv = gd.Invoke("test")
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Basic003_GDele with float
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic003 - with float type parameter`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic003_Class()
    let gd = Basic003_GDele<float>(c.getValue)
    let rv = gd.Invoke(3.14)
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test multiple delegate invocations
    [<FactForNETCOREAPP>]
    let ``Delegate - G_Basic001 - multiple invocations`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Basic001_Class()
    let gd = Basic001_GDele<int>(c.getValue)
    let rv1 = gd.Invoke(1)
    let rv2 = gd.Invoke(2)
    let rv3 = gd.Invoke(3)
    let sum = rv1 + rv2 + rv3
    if sum <> 0 then failwith $"Expected 0 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test NormalDelegate getValue
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_NormalDelegate - direct getValue call`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nd = NormalDelegate()
    let rv = nd.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test multiple NormalDelegate instances
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_NormalDelegate - multiple instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nd1 = NormalDelegate()
    let nd2 = NormalDelegate()
    let sum = nd1.getValue() + nd2.getValue()
    if sum <> -2 then failwith $"Expected -2 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo and Bar interaction
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_WidenAccess - Foo getValue returns 1`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let rv = f.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo getValue2
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_WidenAccess - Foo getValue2 returns -2`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N_003.Foo()
    let rv = f.getValue2()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Bar getValue
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_WidenAccess - Bar getValue returns -2`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = N_003.Bar()
    let rv = b.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test MethodParameter with multiple delegates
    [<FactForNETCOREAPP>]
    let ``Delegate - NG_MethodParam - multiple delegate calls`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let nd1 = NormalDelegate()
    let nd2 = NormalDelegate()
    let dele1 = DeleNormalDelegate(nd1.getValue)
    let dele2 = DeleNormalDelegate(nd2.getValue)
    let mp = N_002.MethodParameter()
    let rv1 = mp.Method(dele1)
    let rv2 = mp.Method(dele2)
    let sum = rv1 + rv2
    if sum <> -2 then failwith $"Expected -2 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding delegateOriginalCSharp delegateForwarderCSharp delegateTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed
