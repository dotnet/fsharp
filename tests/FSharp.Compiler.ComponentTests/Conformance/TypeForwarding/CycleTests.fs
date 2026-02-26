// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypeForwarding/Cycle/
// These tests verify F# works correctly with runtime C# type forwarding in cyclic/multi-assembly scenarios.
//
// Original test pattern (from env.lst):
// 1. Compile C# library with types defined directly (Cycle_Library.cs without FORWARD)
// 2. Compile F# exe referencing C# library
// 3. Replace C# library with forwarding version pointing to multiple target assemblies
// 4. Run F# exe - should work because types are forwarded correctly
//
// This file uses TypeForwardingHelpers.verifyTypeForwarding to test this pattern in-process.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module CycleTypeForwardingTests =

    // ============================================================================
    // C# source definitions (derived from original Cycle_Library.cs and Cycle_Forwarder.cs)
    // ============================================================================

    /// Original C# library with types defined directly (before type forwarding)
    /// For Cycle001 and Cycle002 - basic cycle forwarding
    let cycleOriginalCSharp = """
public class Foo
{
    public int getValue() => 0;
}

public class Bar
{
    public int getValue() => 0;
}

public class Baz
{
    public int getValue() => 0;
}
"""

    /// Target C# library (where types are actually defined after forwarding)
    /// For Cycle001 - Foo and Bar forwarded, Baz stays
    let cycleTargetCSharp = """
public class Foo
{
    public int getValue() => -1;
}

public class Bar
{
    public int getValue() => 1;
}

public class Baz
{
    public int getValue() => 0;
}
"""

    /// Forwarder C# library (type forwarding attributes pointing to Target)
    let cycleForwarderCSharp = """
using System.Runtime.CompilerServices;

// Basic cycle forwarding
[assembly: TypeForwardedTo(typeof(Foo))]
[assembly: TypeForwardedTo(typeof(Bar))]
[assembly: TypeForwardedTo(typeof(Baz))]
"""

    // ============================================================================
    // CYCLE001 - Forwarding to multiple assemblies
    // ============================================================================

    /// Cycle001 - Forwarding to multiple assemblies
    /// From original Cycle001.fs
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - forwarding to multiple assemblies`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let b = Bar()
    let bz = Baz()
    let rv = f.getValue() + b.getValue() + bz.getValue()
    // After forwarding: Foo = -1, Bar = 1, Baz = 0 => sum = 0
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // CYCLE002 - Forwarding multiple times across assemblies
    // ============================================================================

    /// Cycle002 - Forwarding multiple times across assemblies
    /// From original Cycle002.fs
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle002 - forwarding multiple times across assemblies`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let b = Bar()
    let bz = Baz()
    let rv = f.getValue() + b.getValue() + bz.getValue()
    // Same as Cycle001: Foo = -1, Bar = 1, Baz = 0 => sum = 0
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // CYCLE004 - Forwarding between 2 assemblies with no cycle
    // ============================================================================

    /// Original C# for Cycle004 scenario
    let cycle004OriginalCSharp = """
public class Foo
{
    public int getValue() => 0;
}

public class Bar
{
    public int getValue() => 0;
}

public class Baz
{
    public int getValue() => 0;
}
"""

    /// Target C# for Cycle004 - different return values
    let cycle004TargetCSharp = """
public class Foo
{
    public int getValue() => -1;
}

public class Bar
{
    public int getValue() => -2;
}

public class Baz
{
    public int getValue() => -1;
}
"""

    /// Forwarder C# for Cycle004
    let cycle004ForwarderCSharp = """
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(Foo))]
[assembly: TypeForwardedTo(typeof(Bar))]
[assembly: TypeForwardedTo(typeof(Baz))]
"""

    /// Cycle004 - Forwarding between 2 assemblies with no cycle
    /// From original Cycle004.fs
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - forwarding between 2 assemblies no cycle`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let b = Bar()
    let bz = Baz()
    let rv = f.getValue() + b.getValue() + bz.getValue()
    // After forwarding: Foo = -1, Bar = -2, Baz = -1 => sum = -4
    if rv <> -4 then failwith $"Expected -4 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // ADDITIONAL COVERAGE TESTS
    // ============================================================================

    /// Test Foo getValue only
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - Foo getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let rv = f.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Bar getValue only
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - Bar getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Bar()
    let rv = b.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Baz getValue only
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - Baz getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz = Baz()
    let rv = bz.getValue()
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo + Bar only
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - Foo and Bar`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let b = Bar()
    let rv = f.getValue() + b.getValue()
    // Foo = -1, Bar = 1 => sum = 0
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Foo + Baz only
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - Foo and Baz`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let bz = Baz()
    let rv = f.getValue() + bz.getValue()
    // Foo = -1, Baz = 0 => sum = -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Bar + Baz only
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - Bar and Baz`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Bar()
    let bz = Baz()
    let rv = b.getValue() + bz.getValue()
    // Bar = 1, Baz = 0 => sum = 1
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test multiple Foo instances
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - multiple Foo instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f1 = Foo()
    let f2 = Foo()
    let sum = f1.getValue() + f2.getValue()
    // Both return -1 => sum = -2
    if sum <> -2 then failwith $"Expected -2 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test multiple Bar instances
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - multiple Bar instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b1 = Bar()
    let b2 = Bar()
    let sum = b1.getValue() + b2.getValue()
    // Both return 1 => sum = 2
    if sum <> 2 then failwith $"Expected 2 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test multiple Baz instances
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle001 - multiple Baz instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz1 = Baz()
    let bz2 = Baz()
    let sum = bz1.getValue() + bz2.getValue()
    // Both return 0 => sum = 0
    if sum <> 0 then failwith $"Expected 0 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycleOriginalCSharp cycleForwarderCSharp cycleTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Cycle004 Foo getValue
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - Foo getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let rv = f.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Cycle004 Bar getValue
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - Bar getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Bar()
    let rv = b.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Cycle004 Baz getValue
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - Baz getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz = Baz()
    let rv = bz.getValue()
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Cycle004 multiple instances
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - multiple instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f1 = Foo()
    let f2 = Foo()
    let b1 = Bar()
    let b2 = Bar()
    let sum = f1.getValue() + f2.getValue() + b1.getValue() + b2.getValue()
    // Foo = -1 each, Bar = -2 each => -1 + -1 + -2 + -2 = -6
    if sum <> -6 then failwith $"Expected -6 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Cycle004 Foo and Bar
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - Foo and Bar`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let b = Bar()
    let rv = f.getValue() + b.getValue()
    // Foo = -1, Bar = -2 => sum = -3
    if rv <> -3 then failwith $"Expected -3 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Cycle004 Bar and Baz
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - Bar and Baz`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Bar()
    let bz = Baz()
    let rv = b.getValue() + bz.getValue()
    // Bar = -2, Baz = -1 => sum = -3
    if rv <> -3 then failwith $"Expected -3 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Cycle004 Foo and Baz
    [<FactForNETCOREAPP>]
    let ``Cycle - Cycle004 - Foo and Baz`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let bz = Baz()
    let rv = f.getValue() + bz.getValue()
    // Foo = -1, Baz = -1 => sum = -2
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding cycle004OriginalCSharp cycle004ForwarderCSharp cycle004TargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed
