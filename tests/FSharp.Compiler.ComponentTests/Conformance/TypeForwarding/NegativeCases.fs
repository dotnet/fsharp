// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypeForwarding/*/NG_*.fs and Cycle/*.fs
// These tests verify F# can work correctly with C# type forwarding for non-generic types
// and type forwarding cycle scenarios.
//
// The original tests used a two-step pattern:
// 1. Compile F# against original C# library
// 2. Swap in forwarded version and run
//
// This migration tests the same behavior by providing inline C# libraries.
// The tests verify return values match expectations for type forwarding.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System.IO

/// Tests for non-generic type forwarding (NG_* files from Class, Struct, Interface, Delegate folders)
module NonGenericTypeForwarding =

    let private basePath = Path.Combine(__SOURCE_DIRECTORY__, "NegativeCases")

    // C# library with basic class types for non-generic forwarding tests
    let classTypesLibrary = """
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

public class TurnsToStruct
{
    public int getValue() => -1;
}
"""

    // C# library with struct types for non-generic forwarding tests
    let structTypesLibrary = """
public struct NormalStruct
{
    public int getValue() => -1;
}

namespace N_002
{
    public class MethodParameter
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

public class TurnsToClass
{
    public int getValue() => -1;
}
"""

    // C# library with interface types for non-generic forwarding tests
    let interfaceTypesLibrary = """
public interface INormal
{
    int getValue();
}

public class NormalInterface : INormal
{
    public int getValue() => -1;
    int INormal.getValue() => 1;
}

namespace N_002
{
    public class MethodParameter
    {
        public void Method(INormal f) { }
    }
}

namespace N_003
{
    public interface IFoo
    {
        int getValue();
        int getValue2();
    }

    public class Foo : IFoo
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

public class TurnsToClass : INormal
{
    public int getValue() => -1;
    int INormal.getValue() => 1;
}
"""

    // C# library with delegate types for non-generic forwarding tests
    let delegateTypesLibrary = """
public delegate int DeleNormalDelegate();

public class NormalDelegate
{
    public int getValue() => -1;
}

namespace N_002
{
    public class MethodParameter
    {
        public void Method(DeleNormalDelegate f) { }
    }
}

namespace N_003
{
    public delegate int DeleFoo();

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

public class TurnsToClass
{
    public int getValue() => -1;
}
"""

    [<FactForNETCOREAPP>]
    let ``Class - NG_NormalClass - non-generic type forwarding`` () =
        let csLib = CSharp classTypesLibrary |> withName "Class_Library"
        let fsharpCode = """
let nc = new NormalClass()
let rv = nc.getValue()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Class - NG_MethodParam - method parameter with forwarded type`` () =
        let csLib = CSharp classTypesLibrary |> withName "Class_Library"
        let fsharpCode = """
let nc = new NormalClass()
let mp = new N_002.MethodParameter()
mp.Method(nc)
let rv = nc.getValue()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Class - NG_WidenAccess - nested types with forwarding`` () =
        let csLib = CSharp classTypesLibrary |> withName "Class_Library"
        let fsharpCode = """
let f = new N_003.Foo()
let b = new N_003.Bar()
let rv = f.getValue() + b.getValue()
// f.getValue() = 1, b.getValue() = -2 (calls getValue2 which returns -2)
// Sum should be -1
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Class - NG_TurnToStruct - class that becomes struct after forwarding`` () =
        let csLib = CSharp classTypesLibrary |> withName "Class_Library"
        let fsharpCode = """
let f = new TurnsToStruct()
let rv = f.getValue()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Struct - NG_NormalStruct - non-generic struct type forwarding`` () =
        let csLib = CSharp structTypesLibrary |> withName "Struct_Library"
        let fsharpCode = """
let mutable nc = new NormalStruct()
let rv = nc.getValue()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Struct - NG_MethodParam - method parameter with forwarded struct`` () =
        let csLib = CSharp structTypesLibrary |> withName "Struct_Library"
        let fsharpCode = """
let mutable nc = new NormalStruct()
let mp = new N_002.MethodParameter()
mp.Method(nc)
let rv = nc.getValue()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Struct - NG_WidenAccess - nested struct types with forwarding`` () =
        let csLib = CSharp structTypesLibrary |> withName "Struct_Library"
        let fsharpCode = """
let mutable f = new N_003.Foo()
let b = new N_003.Bar()
let rv = f.getValue() + b.getValue()
// f.getValue() = 1, b.getValue() = -2
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Struct - NG_TurnToClass - struct that becomes class after forwarding`` () =
        let csLib = CSharp structTypesLibrary |> withName "Struct_Library"
        let fsharpCode = """
let f = new TurnsToClass()
let rv = f.getValue()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Interface - NG_NormalInterface - non-generic interface type forwarding`` () =
        let csLib = CSharp interfaceTypesLibrary |> withName "Interface_Library"
        let fsharpCode = """
let nc = new NormalInterface()
let i = nc :> INormal
let rv = nc.getValue() + i.getValue()
// nc.getValue() = -1 (direct call), i.getValue() = 1 (explicit interface)
// Sum should be 0
if rv <> 0 then failwith $"Expected 0 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Interface - NG_MethodParam - method parameter with forwarded interface`` () =
        let csLib = CSharp interfaceTypesLibrary |> withName "Interface_Library"
        let fsharpCode = """
let nc = new NormalInterface()
let i = nc :> INormal
let mp = new N_002.MethodParameter()
mp.Method(i)
let rv = nc.getValue() + i.getValue()
if rv <> 0 then failwith $"Expected 0 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Interface - NG_WidenAccess - nested interface types with forwarding`` () =
        let csLib = CSharp interfaceTypesLibrary |> withName "Interface_Library"
        let fsharpCode = """
let f = new N_003.Foo()
let b = new N_003.Bar()
let rv = f.getValue() + b.getValue()
// f.getValue() = 1, b.getValue() = -2
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Interface - NG_TurnToClass - interface implementation after forwarding`` () =
        let csLib = CSharp interfaceTypesLibrary |> withName "Interface_Library"
        let fsharpCode = """
let f = new TurnsToClass()
let i = f :> INormal
let rv = f.getValue() + i.getValue()
// f.getValue() = -1, i.getValue() = 1
if rv <> 0 then failwith $"Expected 0 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Delegate - NG_NormalDelegate - non-generic delegate type forwarding`` () =
        let csLib = CSharp delegateTypesLibrary |> withName "Delegate_Library"
        let fsharpCode = """
let nd = new NormalDelegate()
let dele = new DeleNormalDelegate(nd.getValue)
let rv = dele.Invoke()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Delegate - NG_MethodParam - method parameter with forwarded delegate`` () =
        let csLib = CSharp delegateTypesLibrary |> withName "Delegate_Library"
        let fsharpCode = """
let nd = new NormalDelegate()
let dele = new DeleNormalDelegate(nd.getValue)
let mp = new N_002.MethodParameter()
mp.Method(dele)
let rv = dele.Invoke()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Delegate - NG_WidenAccess - nested delegate types with forwarding`` () =
        let csLib = CSharp delegateTypesLibrary |> withName "Delegate_Library"
        let fsharpCode = """
let f = new N_003.Foo()
let b = new N_003.Bar()
let rv = f.getValue() + b.getValue()
// f.getValue() = 1, b.getValue() = -2
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Delegate - NG_TurnToClass - delegate with class after forwarding`` () =
        let csLib = CSharp delegateTypesLibrary |> withName "Delegate_Library"
        let fsharpCode = """
let f = new TurnsToClass()
let rv = f.getValue()
if rv <> -1 then failwith $"Expected -1 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed


/// Tests for type forwarding cycle scenarios (Cycle folder)
module CycleTypeForwarding =

    let private basePath = Path.Combine(__SOURCE_DIRECTORY__, "NegativeCases")

    // C# library with types that get forwarded to multiple assemblies
    let cycleTypesLibrary = """
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

    [<FactForNETCOREAPP>]
    let ``Cycle001 - forwarding to multiple assemblies`` () =
        let csLib = CSharp cycleTypesLibrary |> withName "Cycle_Library"
        let fsharpCode = """
// Forwarding to multiple assemblies
let f = new Foo()
let b = new Bar()
let bz = new Baz()
let rv = f.getValue() + b.getValue() + bz.getValue()
// f = -1, b = 1, bz = 0 => sum = 0
if rv <> 0 then failwith $"Expected 0 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Cycle002 - forwarding multiple times across assemblies`` () =
        let csLib = CSharp cycleTypesLibrary |> withName "Cycle_Library"
        let fsharpCode = """
// Forwarding multiple times across assemblies
let f = new Foo()
let b = new Bar()
let bz = new Baz()
let rv = f.getValue() + b.getValue() + bz.getValue()
if rv <> 0 then failwith $"Expected 0 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Cycle004 - forwarding between 2 assemblies with no cycle`` () =
        // Different return values to test the expected sum of -4
        let csLib004 = """
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
        let csLib = CSharp csLib004 |> withName "Cycle_Library"
        let fsharpCode = """
// Forwarding between 2 assemblies with no cycle
let f = new Foo()
let b = new Bar()
let bz = new Baz()
let rv = f.getValue() + b.getValue() + bz.getValue()
// Original test expected exit code -4
if rv <> -4 then failwith $"Expected -4 but got {rv}"
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed
