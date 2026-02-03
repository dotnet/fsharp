// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypeForwarding/Nested/
// These tests verify F# works correctly with runtime C# type forwarding for nested types.
//
// Original test pattern (from env.lst):
// 1. Compile C# library with nested types defined directly (Nested_Library.cs without FORWARD)
// 2. Compile F# exe referencing C# library
// 3. Replace C# library with forwarding version (Nested_Library.cs with FORWARD + Nested_Forwarder.cs as Target)
// 4. Run F# exe - should work because types are forwarded to Target.dll
//
// This file uses TypeForwardingHelpers.verifyTypeForwarding to test this pattern in-process.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module NestedTypeForwardingTests =

    // ============================================================================
    // C# source definitions (derived from original Nested_Library.cs and Nested_Forwarder.cs)
    // ============================================================================

    /// Original C# library with nested types defined directly (before type forwarding)
    let nestedOriginalCSharp = """
// Top-level class with nested class
public class Foo
{
    public int getValue() => 0;

    public class Bar
    {
        public int getValue() => 0;
    }
}

// Class in namespace
namespace N002
{
    public class Foo
    {
        public int getValue() => 0;
    }
}

// Class in namespace with nested class
namespace N003
{
    public class Foo
    {
        public int getValue() => 0;
        
        public class Bar
        {
            public int getValue() => 0;
        }
    }
}

// Deeply nested namespace
namespace N0041
{
    namespace N0042
    {
        public class Foo
        {
            public int getValue() => 0;
            
            public class Bar
            {
                public int getValue() => 0;
            }
        }
    }
}

// Non-forwarded classes (Baz)
public class Baz
{
    public int getValue() => 0;
}

namespace N002
{
    public class Baz
    {
        public int getValue() => 0;
    }
}

namespace N003
{
    public class Baz
    {
        public int getValue() => 0;
    }
}

namespace N0041
{
    namespace N0042
    {
        public class Baz
        {
            public int getValue() => 0;
        }
    }
}
"""

    /// Target C# library (where nested types are actually defined after forwarding)
    let nestedTargetCSharp = """
// Top-level class with nested class (forwarded, same structure, different values)
public class Foo
{
    public int getValue() => 1;

    public class Bar
    {
        public int getValue() => -2;
    }
}

// Class in namespace
namespace N002
{
    public class Foo
    {
        public int getValue() => 1;
    }
}

// Class in namespace with nested class
namespace N003
{
    public class Foo
    {
        public int getValue() => 1;
        
        public class Bar
        {
            public int getValue() => -2;
        }
    }
}

// Deeply nested namespace
namespace N0041
{
    namespace N0042
    {
        public class Foo
        {
            public int getValue() => 1;
            
            public class Bar
            {
                public int getValue() => -2;
            }
        }
    }
}

// Non-forwarded classes (Baz) - these don't change
public class Baz
{
    public int getValue() => 0;
}

namespace N002
{
    public class Baz
    {
        public int getValue() => 0;
    }
}

namespace N003
{
    public class Baz
    {
        public int getValue() => 0;
    }
}

namespace N0041
{
    namespace N0042
    {
        public class Baz
        {
            public int getValue() => 0;
        }
    }
}
"""

    /// Forwarder C# library (type forwarding attributes pointing to Target)
    let nestedForwarderCSharp = """
using System.Runtime.CompilerServices;

// Forward top-level Foo (includes nested Bar)
[assembly: TypeForwardedTo(typeof(Foo))]

// Forward N002.Foo
[assembly: TypeForwardedTo(typeof(N002.Foo))]

// Forward N003.Foo (includes nested Bar)
[assembly: TypeForwardedTo(typeof(N003.Foo))]

// Forward N0041.N0042.Foo (includes nested Bar)
[assembly: TypeForwardedTo(typeof(N0041.N0042.Foo))]

// Baz classes are also forwarded for completeness
[assembly: TypeForwardedTo(typeof(Baz))]
[assembly: TypeForwardedTo(typeof(N002.Baz))]
[assembly: TypeForwardedTo(typeof(N003.Baz))]
[assembly: TypeForwardedTo(typeof(N0041.N0042.Baz))]
"""

    // ============================================================================
    // NESTED001 - Basic nested types
    // ============================================================================

    /// Nested001 - Nested types (top-level Foo with nested Bar)
    /// From original Nested001.fs
    [<FactForNETCOREAPP>]
    let ``Nested - Nested001 - top level with nested class`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let b = Foo.Bar()
    let bz = Baz()
    let rv = f.getValue() + b.getValue() + bz.getValue()
    // After forwarding: Foo.getValue() = 1, Bar.getValue() = -2, Baz.getValue() = 0
    // Sum = 1 + (-2) + 0 = -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // NESTED002 - Type nested in namespace
    // ============================================================================

    /// Nested002 - Type nested in namespace
    /// From original Nested002.fs
    [<FactForNETCOREAPP>]
    let ``Nested - Nested002 - type in namespace`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N002.Foo()
    let bz = N002.Baz()
    let rv = f.getValue() + bz.getValue()
    // After forwarding: N002.Foo.getValue() = 1, N002.Baz.getValue() = 0
    // Sum = 1 + 0 = 1
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Nested002b - Type nested in namespace (alternate)
    [<FactForNETCOREAPP>]
    let ``Nested - Nested002b - type in namespace alternate`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N002.Foo()
    let rv = f.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // NESTED003 - Nested types in namespace
    // ============================================================================

    /// Nested003 - Nested types in namespace
    /// From original Nested003.fs
    [<FactForNETCOREAPP>]
    let ``Nested - Nested003 - nested types in namespace`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N003.Foo()
    let b = N003.Foo.Bar()
    let bz = N003.Baz()
    let rv = f.getValue() + b.getValue() + bz.getValue()
    // After forwarding: Foo = 1, Bar = -2, Baz = 0 => sum = -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Nested003b - Nested types in namespace (alternate)
    [<FactForNETCOREAPP>]
    let ``Nested - Nested003b - nested types in namespace alternate`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N003.Foo()
    let b = N003.Foo.Bar()
    let rv = f.getValue() + b.getValue()
    // Foo = 1, Bar = -2 => sum = -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // NESTED004 - Deeply nested namespace
    // ============================================================================

    /// Nested004 - Nested type in deeply nested namespace
    /// From original Nested004.fs
    [<FactForNETCOREAPP>]
    let ``Nested - Nested004 - deeply nested namespace`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N0041.N0042.Foo()
    let b = N0041.N0042.Foo.Bar()
    let bz = N0041.N0042.Baz()
    let rv = f.getValue() + b.getValue() + bz.getValue()
    // After forwarding: Foo = 1, Bar = -2, Baz = 0 => sum = -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Nested004b - Deeply nested namespace (alternate)
    [<FactForNETCOREAPP>]
    let ``Nested - Nested004b - deeply nested namespace alternate`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N0041.N0042.Foo()
    let b = N0041.N0042.Foo.Bar()
    let rv = f.getValue() + b.getValue()
    // Foo = 1, Bar = -2 => sum = -1
    if rv <> -1 then failwith $"Expected -1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    // ============================================================================
    // ADDITIONAL COVERAGE TESTS
    // ============================================================================

    /// Test Foo getValue only
    [<FactForNETCOREAPP>]
    let ``Nested - Nested001 - Foo getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Foo()
    let rv = f.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Bar getValue only
    [<FactForNETCOREAPP>]
    let ``Nested - Nested001 - Bar getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = Foo.Bar()
    let rv = b.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test Baz getValue only
    [<FactForNETCOREAPP>]
    let ``Nested - Nested001 - Baz getValue only`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz = Baz()
    let rv = bz.getValue()
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test N002.Baz getValue
    [<FactForNETCOREAPP>]
    let ``Nested - Nested002 - N002.Baz getValue`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz = N002.Baz()
    let rv = bz.getValue()
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test N003.Foo getValue
    [<FactForNETCOREAPP>]
    let ``Nested - Nested003 - N003.Foo getValue`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N003.Foo()
    let rv = f.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test N003.Foo.Bar getValue
    [<FactForNETCOREAPP>]
    let ``Nested - Nested003 - N003.Foo.Bar getValue`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = N003.Foo.Bar()
    let rv = b.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test N003.Baz getValue
    [<FactForNETCOREAPP>]
    let ``Nested - Nested003 - N003.Baz getValue`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz = N003.Baz()
    let rv = bz.getValue()
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test N0041.N0042.Foo getValue
    [<FactForNETCOREAPP>]
    let ``Nested - Nested004 - N0041.N0042.Foo getValue`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = N0041.N0042.Foo()
    let rv = f.getValue()
    if rv <> 1 then failwith $"Expected 1 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test N0041.N0042.Foo.Bar getValue
    [<FactForNETCOREAPP>]
    let ``Nested - Nested004 - N0041.N0042.Foo.Bar getValue`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b = N0041.N0042.Foo.Bar()
    let rv = b.getValue()
    if rv <> -2 then failwith $"Expected -2 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test N0041.N0042.Baz getValue
    [<FactForNETCOREAPP>]
    let ``Nested - Nested004 - N0041.N0042.Baz getValue`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz = N0041.N0042.Baz()
    let rv = bz.getValue()
    if rv <> 0 then failwith $"Expected 0 but got {rv}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test multiple Foo instances
    [<FactForNETCOREAPP>]
    let ``Nested - Nested001 - multiple Foo instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f1 = Foo()
    let f2 = Foo()
    let sum = f1.getValue() + f2.getValue()
    if sum <> 2 then failwith $"Expected 2 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test multiple Bar instances
    [<FactForNETCOREAPP>]
    let ``Nested - Nested001 - multiple Bar instances`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b1 = Foo.Bar()
    let b2 = Foo.Bar()
    let sum = b1.getValue() + b2.getValue()
    if sum <> -4 then failwith $"Expected -4 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test all namespace Foos together
    [<FactForNETCOREAPP>]
    let ``Nested - All namespaces - all Foo classes`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f1 = Foo()
    let f2 = N002.Foo()
    let f3 = N003.Foo()
    let f4 = N0041.N0042.Foo()
    let sum = f1.getValue() + f2.getValue() + f3.getValue() + f4.getValue()
    // All return 1, so sum = 4
    if sum <> 4 then failwith $"Expected 4 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test all namespace Bars together
    [<FactForNETCOREAPP>]
    let ``Nested - All namespaces - all Bar classes`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let b1 = Foo.Bar()
    let b2 = N003.Foo.Bar()
    let b3 = N0041.N0042.Foo.Bar()
    let sum = b1.getValue() + b2.getValue() + b3.getValue()
    // All return -2, so sum = -6
    if sum <> -6 then failwith $"Expected -6 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed

    /// Test all namespace Bazs together
    [<FactForNETCOREAPP>]
    let ``Nested - All namespaces - all Baz classes`` () =
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let bz1 = Baz()
    let bz2 = N002.Baz()
    let bz3 = N003.Baz()
    let bz4 = N0041.N0042.Baz()
    let sum = bz1.getValue() + bz2.getValue() + bz3.getValue() + bz4.getValue()
    // All return 0, so sum = 0
    if sum <> 0 then failwith $"Expected 0 but got {sum}"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding nestedOriginalCSharp nestedForwarderCSharp nestedTargetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed
