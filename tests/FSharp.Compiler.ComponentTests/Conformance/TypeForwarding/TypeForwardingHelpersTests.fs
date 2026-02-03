// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Tests for TypeForwardingHelpers module that enables runtime type forwarding tests.
// These tests verify the infrastructure for testing assembly substitution scenarios.

namespace Conformance.TypeForwarding

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module TypeForwardingHelpersTests =

    /// Basic test: Compile a class in C#, compile F# referencing it, swap assembly with forwarder,
    /// then execute in-process via reflection.
    [<FactForNETCOREAPP>]
    let ``TypeForwardingHelpers - basic class forwarding at runtime`` () =
        // Original C# library with the class defined directly
        let originalCSharp = """
public class MyClass
{
    public int GetValue() => 42;
}
"""
        // Target library where the class will be defined after forwarding
        let targetCSharp = """
public class MyClass
{
    public int GetValue() => 42;
}
"""
        // Forwarder library that forwards MyClass to Target.dll
        let forwarderCSharp = """
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(MyClass))]
"""
        // F# code that uses the class - no printfn to avoid Console.Out issues in parallel tests
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let obj = MyClass()
    let v = obj.GetValue()
    if v <> 42 then failwith "Expected 42"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding originalCSharp forwarderCSharp targetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed


    /// Test with generic class forwarding
    [<FactForNETCOREAPP>]
    let ``TypeForwardingHelpers - generic class forwarding at runtime`` () =
        let originalCSharp = """
public class Container<T>
{
    private T _value;
    public Container(T value) { _value = value; }
    public T GetValue() => _value;
}
"""
        let targetCSharp = """
public class Container<T>
{
    private T _value;
    public Container(T value) { _value = value; }
    public T GetValue() => _value;
}
"""
        let forwarderCSharp = """
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(Container<>))]
"""
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let c = Container<int>(123)
    let v = c.GetValue()
    if v <> 123 then failwith "Expected 123"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding originalCSharp forwarderCSharp targetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed


    /// Test with interface forwarding
    [<FactForNETCOREAPP>]
    let ``TypeForwardingHelpers - interface forwarding at runtime`` () =
        let originalCSharp = """
public interface IProvider
{
    string GetName();
}

public class DefaultProvider : IProvider
{
    public string GetName() => "Default";
}
"""
        let targetCSharp = """
public interface IProvider
{
    string GetName();
}

public class DefaultProvider : IProvider
{
    public string GetName() => "Default";
}
"""
        let forwarderCSharp = """
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(IProvider))]
[assembly: TypeForwardedTo(typeof(DefaultProvider))]
"""
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let p = DefaultProvider() :> IProvider
    let name = p.GetName()
    if name <> "Default" then failwith "Expected Default"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding originalCSharp forwarderCSharp targetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed


    /// Test with struct forwarding
    [<FactForNETCOREAPP>]
    let ``TypeForwardingHelpers - struct forwarding at runtime`` () =
        let originalCSharp = """
public struct Point
{
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
    public int Sum() => X + Y;
}
"""
        let targetCSharp = """
public struct Point
{
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
    public int Sum() => X + Y;
}
"""
        let forwarderCSharp = """
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(Point))]
"""
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let mutable p = Point(10, 20)
    let s = p.Sum()
    if s <> 30 then failwith "Expected 30"
    0
"""
        TypeForwardingHelpers.verifyTypeForwarding originalCSharp forwarderCSharp targetCSharp fsharpSource
        |> TypeForwardingHelpers.shouldSucceed


    /// Test that verifyTypeForwarding can detect execution failures
    [<FactForNETCOREAPP>]
    let ``TypeForwardingHelpers - execution failure is detected`` () =
        let originalCSharp = """
public class Failer
{
    public void Fail() { throw new System.Exception("Intentional failure"); }
}
"""
        let targetCSharp = """
public class Failer
{
    public void Fail() { throw new System.Exception("Intentional failure"); }
}
"""
        let forwarderCSharp = """
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(Failer))]
"""
        let fsharpSource = """
[<EntryPoint>]
let main _ =
    let f = Failer()
    f.Fail()  // This will throw
    0
"""
        let result = TypeForwardingHelpers.verifyTypeForwarding originalCSharp forwarderCSharp targetCSharp fsharpSource
        match result with
        | TFExecutionFailure ex ->
            Assert.Contains("Intentional failure", ex.Message)
        | _ ->
            failwith "Expected TFExecutionFailure but got success or compilation failure"
