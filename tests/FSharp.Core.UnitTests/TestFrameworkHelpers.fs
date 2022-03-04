// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Xunit

open System
open System.Collections.Generic
open System.Linq
open System.Reflection
open System.Runtime.InteropServices

open Xunit

module Info =

    /// Use this to distinguish cases where output is deterministically different between x86 runtime or x64 runtime,
    /// for instance w.r.t. floating point arithmetic. For more info, see https://github.com/dotnet/roslyn/issues/7333
    let isX86Runtime = sizeof<IntPtr> = 4

    /// Use this to distinguish cases where output is deterministically different between x86 runtime or x64 runtime,
    /// for instance w.r.t. floating point arithmetic. For more info, see https://github.com/dotnet/roslyn/issues/7333
    let isX64Runtime = sizeof<IntPtr> = 8

    let framework = RuntimeInformation.FrameworkDescription

    /// Whether a test is run inside a .NET Core Runtime
    let isNetCore = framework.StartsWith(".NET Core")

    /// Whether a test is run using a .NET Framework Runtime
    let isNetFramework = framework.StartsWith(".NET Framework")

    /// Whether a test is run after being compiled to .NET Native
    let isNetNative = framework.StartsWith(".NET Native")


module private Impl =

    let rec equals (expected:obj) (actual:obj) =

        // get length expected
        let toArray (o:obj) =
            match o with
            | :? seq<bigint> as seq -> seq |> Seq.toArray :>obj
            | :? seq<decimal> as seq -> seq |> Seq.toArray :>obj
            | :? seq<float> as seq -> seq |> Seq.toArray :>obj
            | :? seq<float32> as seq -> seq |> Seq.toArray :>obj
            | :? seq<uint64> as seq -> seq |> Seq.toArray :>obj
            | :? seq<int64> as seq -> seq |> Seq.toArray :>obj
            | :? seq<uint32> as seq -> seq |> Seq.toArray :>obj
            | :? seq<int32> as seq -> seq |> Seq.toArray :>obj
            | :? seq<uint16> as seq -> seq |> Seq.toArray :>obj
            | :? seq<int16> as seq -> seq |> Seq.toArray :>obj
            | :? seq<sbyte> as seq -> Enumerable.ToArray(seq) :>obj
            | :? seq<byte> as seq -> seq |> Seq.toArray :>obj
            | :? seq<char> as seq -> seq |> Seq.toArray :>obj
            | :? seq<bool> as seq -> seq |> Seq.toArray :>obj
            | :? seq<string> as seq -> seq |> Seq.toArray :>obj
            | :? seq<IntPtr> as seq -> seq |> Seq.toArray :>obj
            | :? seq<UIntPtr> as seq -> seq |> Seq.toArray :>obj
            | :? seq<obj> as seq -> seq |> Seq.toArray :>obj
            | _ -> o

        // get length expected
        let expected = toArray expected
        let actual = toArray actual

        match expected, actual with 
        |   (:? Array as a1), (:? Array as a2) ->
                if a1.Rank > 1 then failwith "Rank > 1 not supported"                
                if a2.Rank > 1 then false
                else
                    let lb = a1.GetLowerBound(0)
                    let ub = a1.GetUpperBound(0)
                    if lb <> a2.GetLowerBound(0) || ub <> a2.GetUpperBound(0) then false
                    else
                        {lb..ub} |> Seq.forall(fun i -> equals (a1.GetValue(i)) (a2.GetValue(i)))    
        |   _ ->
                Object.Equals(expected, actual)

    /// Special treatment of float and float32 to get a somewhat meaningful error message 
    /// (otherwise, the missing precision leads to different values that are close together looking the same)
    let floatStr (flt1: obj) (flt2: obj)  = 
        match flt1, flt2 with
        | :? float as flt1, (:? float as flt2) ->
            flt1.ToString("R"), flt2.ToString("R")

        | :? float32 as flt1, (:? float32 as flt2) ->
            flt1.ToString("R"), flt2.ToString("R")

        | _ -> flt1.ToString(), flt2.ToString()


type Assert =

    static member AreEqual(expected : obj, actual : obj, message : string) =
        if not (Impl.equals expected actual) then
            let message = 
                let (exp, act) = Impl.floatStr expected actual
                sprintf "%s: Expected %s but got %s" message exp act

            Exception message |> raise

    static member AreNotEqual(expected : obj, actual : obj, message : string) =
        if Impl.equals expected actual then
            let message = sprintf "%s: Expected not %A but got %A" message expected actual
            Exception message |> raise

    static member AreEqual(expected : obj, actual : obj) = Assert.AreEqual(expected, actual, "Assertion")

    /// Use this to compare floats within a delta of 1e-15, useful for discrepancies
    /// between 80-bit (dotnet, RyuJIT) and 64-bit (x86, Legacy JIT) floating point calculations
    static member AreNearEqual(expected: float, actual: float) =
        let delta = 1.0e-15
        let message = 
            let ((e, a)) = Impl.floatStr expected actual
            sprintf "Are near equal: expected %s, but got %s (with delta: %f)" e a delta

        Assert.AreEqual(Math.Round(expected, 15), Math.Round(actual, 15), message)

    static member AreNotEqual(expected: obj, actual: obj) = Assert.AreNotEqual(expected, actual, "Assertion")

    static member Fail(message : string) = Exception(message) |> raise

    static member Fail() = Assert.Fail("") 

    static member Fail(message : string, args : obj[]) = Assert.Fail(String.Format(message,args))

type CollectionAssert =
    static member AreEqual(expected, actual) = 
        Assert.AreEqual(expected, actual)

/// Disposable type to implement a simple resolve handler that searches the currently loaded assemblies to see if the requested assembly is already loaded.
/// using simple names
type AlreadyLoadedBySimpleNameAppDomainResolver () =
    let resolveHandler =
        ResolveEventHandler(fun _ args ->
            let nArgs = AssemblyName(args.Name)
            let assemblies = AppDomain.CurrentDomain.GetAssemblies()
            let assembly = assemblies |>
                            Array.tryFind(fun a ->
                                              let name = AssemblyName(a.FullName)
                                              String.Compare(name.Name,nArgs.Name,StringComparison.OrdinalIgnoreCase) = 0)
            assembly |> Option.defaultValue Unchecked.defaultof<Assembly>
            )
    do AppDomain.CurrentDomain.add_AssemblyResolve(resolveHandler)

    interface IDisposable with
        member this.Dispose() = AppDomain.CurrentDomain.remove_AssemblyResolve(resolveHandler)

type TestClassWithSimpleNameAppDomainResolver () =
    let resolver = new AlreadyLoadedBySimpleNameAppDomainResolver() :> IDisposable

    interface IDisposable with
        member this.Dispose() = resolver.Dispose()

