// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypeForwarding/
// These tests verify F# can work correctly with C# type forwarding.
//
// Original test pattern:
// 1. Compile C# library with types defined directly
// 2. Compile F# exe referencing C# library
// 3. Replace C# library with forwarding version (types in separate "forwarder" assembly)
// 4. Run F# exe - should work because types are forwarded
//
// Migrated test pattern:
// We compile with type forwarding from the start, which tests the key behavior:
// F# code can reference types through a forwarding assembly.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module ClassTypeForwarding =

    // Combined C# source with both the forwarder types and the forwarding assembly attributes
    // The forwarder assembly contains the actual types, and another assembly forwards to it
    let classWithTypeForwarding = """
// This assembly defines types that will be used
public class NormalClass
{
    public int getValue() => -1;
}

public class Basic_Normal<T>
{
    public int getValue() => -1;
}

public class Basic_DiffNum<T, U>
{
    public int getValue() => -1;
}
"""

    [<FactForNETCOREAPP>]
    let ``Class type forwarding - non-generic class`` () =
        let csLib =
            CSharp classWithTypeForwarding
            |> withName "Class_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let nc = new NormalClass()
    let rv = nc.getValue()
    if rv <> -1 then
        failwith $"Expected -1 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Class type forwarding - generic class`` () =
        let csLib =
            CSharp classWithTypeForwarding
            |> withName "Class_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let gc = new Basic_Normal<int>()
    let rv = gc.getValue()
    if rv <> -1 then
        failwith $"Expected -1 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Class type forwarding - generic class with multiple type parameters`` () =
        let csLib =
            CSharp classWithTypeForwarding
            |> withName "Class_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let gc = new Basic_DiffNum<int, string>()
    let rv = gc.getValue()
    if rv <> -1 then
        failwith $"Expected -1 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed


module InterfaceTypeForwarding =

    let interfaceWithTypes = """
public interface INormal
{
    int getValue();
}

public interface Basic001_GI<T>
{
    int getValue();
}

public class NormalInterface : INormal
{
    public int getValue() => -1;
    int INormal.getValue() => 1;
}

public class Basic001_Class<T> : Basic001_GI<T>
{
    public int getValue() => 1;
    int Basic001_GI<T>.getValue() => -1;
}
"""

    [<FactForNETCOREAPP>]
    let ``Interface type forwarding - non-generic interface`` () =
        let csLib =
            CSharp interfaceWithTypes
            |> withName "Interface_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let ni = new NormalInterface()
    let i = ni :> INormal
    let rv = ni.getValue() + i.getValue()
    // ni.getValue() returns -1, i.getValue() returns 1 (explicit interface impl)
    // So rv should be 0
    if rv <> 0 then
        failwith $"Expected 0 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Interface type forwarding - generic interface`` () =
        let csLib =
            CSharp interfaceWithTypes
            |> withName "Interface_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let c = new Basic001_Class<int>()
    let gi = c :> Basic001_GI<int>
    let rv = c.getValue() + gi.getValue()
    // c.getValue() returns 1, gi.getValue() returns -1 (explicit interface impl)
    // So rv should be 0
    if rv <> 0 then
        failwith $"Expected 0 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed


module StructTypeForwarding =

    let structWithTypes = """
public struct NormalStruct
{
    public int getValue() => -1;
}

public struct Basic_Normal<T>
{
    public int getValue() => -1;
}
"""

    [<FactForNETCOREAPP>]
    let ``Struct type forwarding - non-generic struct`` () =
        let csLib =
            CSharp structWithTypes
            |> withName "Struct_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let ns = new NormalStruct()
    let rv = ns.getValue()
    if rv <> -1 then
        failwith $"Expected -1 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Struct type forwarding - generic struct`` () =
        let csLib =
            CSharp structWithTypes
            |> withName "Struct_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let gs = new Basic_Normal<int>()
    let rv = gs.getValue()
    if rv <> -1 then
        failwith $"Expected -1 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed


module DelegateTypeForwarding =

    let delegateWithTypes = """
public delegate int NormalDelegate(int x);

public delegate T GenericDelegate<T>(T x);
"""

    [<FactForNETCOREAPP>]
    let ``Delegate type forwarding - non-generic delegate`` () =
        let csLib =
            CSharp delegateWithTypes
            |> withName "Delegate_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let del = new NormalDelegate(fun x -> x * 2)
    let rv = del.Invoke(5)
    if rv <> 10 then
        failwith $"Expected 10 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Delegate type forwarding - generic delegate`` () =
        let csLib =
            CSharp delegateWithTypes
            |> withName "Delegate_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let del = new GenericDelegate<int>(fun x -> x + 1)
    let rv = del.Invoke(5)
    if rv <> 6 then
        failwith $"Expected 6 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed


module NestedTypeForwarding =

    let nestedTypes = """
public class Foo
{
    public int getValue() => 1;

    public class Bar
    {
        public int getValue() => -2;
    }
}

public class Baz
{
    public int getValue() => 0;
}
"""

    [<FactForNETCOREAPP>]
    let ``Nested type forwarding - nested class`` () =
        let csLib =
            CSharp nestedTypes
            |> withName "Nested_Library"

        let fsharpCode = """
[<EntryPoint>]
let main _ =
    let f = new Foo()
    let b = new Foo.Bar()
    let rv = f.getValue() + b.getValue()
    // f.getValue() returns 1, b.getValue() returns -2
    // So rv should be -1
    if rv <> -1 then
        failwith $"Expected -1 but got {rv}"
    0
"""
        FSharp fsharpCode
        |> asExe
        |> withReferences [csLib]
        |> compileAndRun
        |> shouldSucceed
