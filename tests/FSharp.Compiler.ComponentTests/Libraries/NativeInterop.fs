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

    // Regression tests for https://github.com/dotnet/fsharp/issues/20295 (Case 1): 'NativePtr.stackalloc'
    // emits the 'localloc' IL instruction, which the JIT rejects inside an exception-handling region.
    // Such code used to compile and then throw InvalidProgramException at method load; it must now be
    // rejected at compile time with FS3916.
    [<Theory>]
    [<InlineData("try () with _ -> NativePtr.stackalloc<int> 1 |> ignore")>]
    [<InlineData("try () with :? System.Exception -> NativePtr.stackalloc<int> 1 |> ignore")>]
    [<InlineData("try () finally NativePtr.stackalloc<int> 1 |> ignore")>]
    [<InlineData("try () with _ -> (try () with _ -> NativePtr.stackalloc<int> 1 |> ignore)")>]
    // An immediately-applied lambda in a handler is inlined into the handler's IL region by the
    // optimizer, so its 'localloc' still lands inside the exception region and must be rejected.
    [<InlineData("try () with _ -> (fun () -> NativePtr.stackalloc<int> 1 |> ignore) ()")>]
    let ``stackalloc in a handler is rejected`` (handler: string) =
        $"""
module Test
open Microsoft.FSharp.NativeInterop
let f () = {handler}
"""
        |> FSharp
        |> withNoWarn 9
        |> compile
        |> shouldFail
        |> withErrorCode 3916

    // A 'let inline' wrapper around 'stackalloc' is inlined into the handler's IL region, so its
    // 'localloc' still lands inside the exception region and must be rejected. The pre-codegen syntactic
    // check missed this because the wrapper hid the 'stackalloc' call behind an inlinable function.
    [<Fact>]
    let ``stackalloc via an inline wrapper inside a handler is rejected`` () =
        FSharp """
module Test
open Microsoft.FSharp.NativeInterop
let inline alloc () = NativePtr.stackalloc<int> 1 |> ignore
let f () = try () with _ -> alloc ()
"""
        |> withNoWarn 9
        |> compile
        |> shouldFail
        |> withErrorCode 3916

    // An escaping closure defined in a handler is compiled to its own method, so its 'localloc' lives
    // outside the exception region and is legal. Such code must not be rejected (regression guard against
    // the pre-codegen syntactic check's false positive).
    [<Fact>]
    let ``stackalloc in an escaping closure inside a handler is allowed`` () =
        FSharp """
module Test
open Microsoft.FSharp.NativeInterop
let f () =
    try ()
    with _ ->
        let g = fun () -> NativePtr.stackalloc<int> 1 |> ignore
        System.Action<unit>(g).Invoke()
[<EntryPoint>]
let main _ =
    f ()
    printfn "ran-closure"
    0
"""
        |> withNoWarn 9
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "ran-closure"

    // 'localloc' is legal in the protected 'try' body itself (only handler/filter/finally/fault
    // regions reject it), so 'stackalloc' directly inside a 'try' must still compile.
    [<Fact>]
    let ``stackalloc in the try body is allowed`` () =
        FSharp """
module Test
open Microsoft.FSharp.NativeInterop
let f () = try NativePtr.stackalloc<int> 1 |> ignore with _ -> ()
"""
        |> withNoWarn 9
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``stackalloc in an object-expression method inside a handler is allowed`` () =
        FSharp """
module Test
open Microsoft.FSharp.NativeInterop
let f () =
    try ()
    with _ ->
        let d = { new System.IDisposable with member _.Dispose() = NativePtr.stackalloc<int> 1 |> ignore }
        d.Dispose()
[<EntryPoint>]
let main _ =
    f ()
    printfn "ok"
    0
"""
        |> withNoWarn 9
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "ok"

    [<Fact>]
    let ``stackalloc outside any try compiles and runs`` () =
        FSharp """
module Test
open Microsoft.FSharp.NativeInterop
[<EntryPoint>]
let main _ =
    NativePtr.stackalloc<int> 1 |> ignore
    printfn "ok"
    0
"""
        |> withNoWarn 9
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``handler without stackalloc is unaffected`` () =
        FSharp """
module Test
let f () = try () with _ -> printfn "handled"
"""
        |> compile
        |> shouldSucceed

    // Regression tests for https://github.com/dotnet/fsharp/issues/20295 (Case 2): a 'NativePtr.stackalloc'
    // used as a chained base-constructor argument loads the uninitialized 'this' before evaluating the
    // argument, so its 'localloc' ran with 'this' pending on the stack and could not be spilled - the
    // emitted IL threw InvalidProgramException at load. The args are now hoisted into locals before 'this'.
    [<TheoryForNETCOREAPP>]
    // simple nativeptr<int> base-ctor arg
    [<InlineData("type A(p: nativeptr<int>) = class end",
                 "type B() = inherit A(NativePtr.stackalloc<int> 1)")>]
    // stackalloc as one of several base-ctor args
    [<InlineData("type A(n: int, p: nativeptr<int>) = class end",
                 "type B() = inherit A(1, NativePtr.stackalloc<int> 1)")>]
    // generic base type instantiated concretely
    [<InlineData("type A<'T when 'T: unmanaged>(p: nativeptr<'T>) = class end",
                 "type B() = inherit A<int>(NativePtr.stackalloc<int> 1)")>]
    let ``stackalloc as a base-ctor argument compiles and runs`` (baseType: string) (derived: string) =
        $"""
module Test
open Microsoft.FSharp.NativeInterop
{baseType}
{derived}
[<EntryPoint>]
let main _ =
    B() |> ignore
    printfn "ok"
    0
"""
        |> FSharp
        |> withNoWarn 9
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "ok"

    // The hoist evaluates the base-ctor args left-to-right into locals before pushing 'this'; a
    // side-effecting normal arg before the stackalloc arg must still run first.
    [<FactForNETCOREAPP>]
    let ``stackalloc base-ctor argument preserves left-to-right order`` () =
        FSharp """
module Test
open Microsoft.FSharp.NativeInterop
let trace = System.Text.StringBuilder()
let step (name: string) x = trace.Append name |> ignore; x
type A(n: int, p: nativeptr<int>) = class end
type B() = inherit A(step "a" 1, step "b" (NativePtr.stackalloc<int> 1))
[<EntryPoint>]
let main _ =
    B() |> ignore
    if string trace <> "ab" then failwithf "wrong order: %O" trace
    printfn "ok"
    0
"""
        |> withNoWarn 9
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "ok"

    // No-regression: an ordinary base ctor without a 'localloc' argument must not hoist - the arg is
    // pushed directly onto 'this', with no extra local introduced by the hoist.
    [<Fact>]
    let ``ordinary base-ctor argument is not hoisted`` () =
        FSharp """
module Test
type A(n: int) = class end
type B() = inherit A(1)
"""
        |> compile
        |> shouldSucceed
        |> verifyILContains [
            """.method public specialname rtspecialname instance void  .ctor() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  callvirt   instance void Test/A::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  pop
        IL_0009:  ret
      }""" ]
