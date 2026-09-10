// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Libraries/Core/NativeInterop/stackalloc

namespace Libraries

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for NativeInterop - stackalloc functionality
module NativeInterop =

    // negativesize01.fs - Test that stackalloc with negative size is handled properly
    // <Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Libraries/Core/NativeInterop/stackalloc", Includes=[|"negativesize01.fs"|])>]
    let ``stackalloc - negativesize01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldSucceed
        |> ignore

    // Regression tests for https://github.com/dotnet/fsharp/issues/8083: a 'stackalloc' nested in a
    // larger expression used to emit IL the JIT rejects, throwing InvalidProgramException at load.
    [<FactForNETCOREAPP>]
    let ``stackalloc nested in a larger expression`` () =
        """
module Test
open System
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

// NoInlining keeps the receiver/argument pending on the stack; without it the optimizer inlines
// these bodies away and the tests silently stop exercising the spill.
[<Sealed>]
type Sink() =
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member _.Put(a: ReadOnlySpan<byte>) = a.Length

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member _.Put(a: ReadOnlySpan<byte>, b: ReadOnlySpan<byte>) = a.Length - b.Length

    [<MethodImpl(MethodImplOptions.NoInlining ||| MethodImplOptions.NoOptimization)>]
    static member StaticPut(n: byte, a: ReadOnlySpan<byte>) = a[0] + n

let inline alloc<'T when 'T: unmanaged> n =
    Span<'T>(NativePtr.toVoidPtr (NativePtr.stackalloc<'T> n), n)

let arrayElementAssignment () =
    let a: nativeint[] = Array.zeroCreate 1
    a[0] <-
        let h = NativePtr.stackalloc<int> 1
        NativePtr.set h 0 42
        NativePtr.toNativeInt h
    if NativePtr.get (NativePtr.ofNativeInt<int> a[0]) 0 <> 42 then failwith "wrong value"

let instanceMethodArguments () =
    if Sink().Put(alloc<byte> 16, alloc<byte> 8) <> 8 then failwith "wrong value"

let staticMethodArguments () =
    if Sink.StaticPut(42uy, alloc<byte> 16) <> 42uy then failwith "wrong value"

let forInLoopOverSequence () =
    let sink = Sink()
    let mutable total = 0
    for n in seq { 1..4 } do
        total <- total + sink.Put(alloc<byte> n)
    if total <> 10 then failwith "wrong value"

// The size expression allocates a local; it must not reuse the slot holding a spilled pending value.
[<MethodImpl(MethodImplOptions.NoInlining)>]
let spilledValuesSurviveTheSizeExpression (i: int) (n: int) =
    let a: nativeint[] = Array.zeroCreate 32
    a[i] <- (# "localloc" (let k = n * 3 in k + k) : nativeint #)
    if a[i] = 0n then failwith "wrong element"

// Inline IL can put 'localloc' anywhere in a multi-instruction sequence, not just on its own.
let multiInstructionInlineIL () =
    let inline alloc8 (n: int) : nativeptr<byte> = (# "conv.i localloc" n : nativeptr<byte> #)
    if Sink().Put(Span<byte>(NativePtr.toVoidPtr (alloc8 8), 8)) <> 8 then failwith "wrong value"

let evaluationOrder () =
    let trace = Text.StringBuilder()
    let step name x =
        trace.Append(name: string) |> ignore
        x
    (step "a" (Sink()))
        .Put(Span<byte>(NativePtr.toVoidPtr (NativePtr.stackalloc<byte> (step "b" 8)), step "c" 8))
    |> ignore
    if string trace <> "abc" then failwithf "wrong order: %O" trace

[<EntryPoint>]
let main _ =
    arrayElementAssignment ()
    instanceMethodArguments ()
    staticMethodArguments ()
    forInLoopOverSequence ()
    multiInstructionInlineIL ()
    spilledValuesSurviveTheSizeExpression 5 3
    evaluationOrder ()
    printfn "ok"
    0
        """
        |> FSharp
        |> withNoWarn 9
        |> withNoWarn 42
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "ok"

    [<Fact>]
    let ``stackalloc spills the pending evaluation stack`` () =
        FSharp """
module Test
open Microsoft.FSharp.NativeInterop
open System.Runtime.CompilerServices

[<Sealed>]
type Sink() =
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member _.Put(p: nativeint) = int p

let store (a: nativeint[]) =
    a[0] <- NativePtr.toNativeInt (NativePtr.stackalloc<int> 1)

let call (s: Sink) =
    s.Put(NativePtr.toNativeInt (NativePtr.stackalloc<int> 1))
        """
        |> withNoWarn 9
        |> compile
        |> shouldSucceed
        |> verifyILContains [
            """.method public static void  store(native int[] a) cil managed
    {

      .maxstack  5
      .locals init (int32 V_0,
               native int[] V_1,
               native int V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  stloc.1
      IL_0004:  ldc.i4.1
      IL_0005:  sizeof     [runtime]System.Int32
      IL_000b:  mul
      IL_000c:  localloc
      IL_000e:  stloc.2
      IL_000f:  ldloc.1
      IL_0010:  ldloc.0
      IL_0011:  ldloc.2
      IL_0012:  stelem.i
      IL_0013:  ret
    }"""
            """.method public static int32  'call'(class Test/Sink s) cil managed
    {

      .maxstack  4
      .locals init (class Test/Sink V_0,
               native int V_1)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.1
      IL_0003:  sizeof     [runtime]System.Int32
      IL_0009:  mul
      IL_000a:  localloc
      IL_000c:  stloc.1
      IL_000d:  ldloc.0
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance int32 Test/Sink::Put(native int)
      IL_0014:  ret
    }""" ]
