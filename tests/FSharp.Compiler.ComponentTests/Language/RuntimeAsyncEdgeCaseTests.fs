module Language.RuntimeAsyncEdgeCaseTests

open Xunit
open FSharp.Test.Compiler
open System.IO
open System.Reflection.Metadata

let private runtimeAsyncDir = Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync")

// Builds a minimal direct-intrinsic compilation unit: the module header plus the opens every
// StateMachineHelpers.__runtimeAsyncReturn body needs, then the supplied one-liner body. Used for the
// IL-shape and diagnostic assertions that need a single-method assembly.
let private directIntrinsicSource body =
    String.concat "\n" [
        "module M"
        "open System"
        "open System.Threading.Tasks"
        "open System.Runtime.CompilerServices"
        "open Microsoft.FSharp.Core.CompilerServices"
        body
    ]

let private compileDirect body =
    FSharp(directIntrinsicSource body)
    |> withFSharpCoreShippedNet
    |> withLangVersionPreview
    |> compile

let private assertSingleDiagnostic errorNumber (result: CompilationResult) =
    let diagnostics =
        result.Output.Diagnostics
        |> List.filter (fun diagnostic ->
            match diagnostic.Error with
            | Error number -> number = errorNumber
            | _ -> false)

    Assert.Equal(1, List.length diagnostics)
    result

// ref-struct-across-suspension written through the CE builder: the `do!` desugars to a continuation
// lambda that captures the span, so F#'s byref-capture check rejects it (FS0406).
let private refStructAcrossAwaitCE = """
module M
open System
open System.Threading.Tasks
open Microsoft.FSharp.Control.RuntimeAsync.TaskExpressions
let f () : Task<int> =
    task {
        let data = [| 10; 20; 30 |]
        let span = ReadOnlySpan<int>(data)
        do! Task.Delay(1)
        return span[0] + span[1] + span[2]
    }
"""

let private ceStateMachineSource = """
module CeUser
open System.Threading.Tasks
open Microsoft.FSharp.Control.RuntimeAsync.TaskExpressions
let f () : Task<int> =
    task {
        let! x = Task.FromResult 41
        return x + 1
    }
"""

#if NETCOREAPP

// ============================ execution (runtime behavior) ============================

[<InlineData(false)>]
[<InlineData(true)>]
[<Theory>]
let ``runtime async edge cases execute through the CE builder`` (optimize: bool) =
    FsFromPath (Path.Combine(runtimeAsyncDir, "RuntimeAsyncEdgeCases.fs"))
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimize
    |> compileExeAndRun
    |> shouldSucceed

// ============================ emitted IL (codegen contract) ============================
// Each fact pins the whole method body captured from the PR's fsc, so dotnet/runtime reviewers get
// the exact lowering to check against docs/runtime-async.md. ILChecker normalizes both sides
// ([System.Runtime] -> [runtime], collapses the `.method` signature, strips comments); a codegen
// exhibit is meant to change when the codegen changes.

// `Await(Task); 1` — direct call to the non-generic Await overload, then push 1 and ret.
let private simpleAwaitBody = """
  .method public static class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
          f() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::Delay(int32)
    IL_0006:  call       void [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(class [System.Runtime]System.Threading.Tasks.Task)
    IL_000b:  ldc.i4.1
    IL_000c:  ret
  } // end of method M::f
"""

// `let x = Await(Task<int>) in x + 1` — the generic Await<int32> overload; result feeds directly
// into `add` with no spill local (optimized).
let private genericAwaitBody = """
  .method public static class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
          f(class [System.Runtime]System.Threading.Tasks.Task`1<int32> t) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       !!0 [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await<int32>(class [System.Runtime]System.Threading.Tasks.Task`1<!!0>)
    IL_0006:  ldc.i4.1
    IL_0007:  add
    IL_0008:  ret
  } // end of method M::f
"""

// `Await(ValueTask); 1` — the ValueTask (non-generic) Await overload bound by operand type.
let private valueTaskAwaitBody = """
  .method public static class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
          f(valuetype [System.Runtime]System.Threading.Tasks.ValueTask vt) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       void [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(valuetype [System.Runtime]System.Threading.Tasks.ValueTask)
    IL_0006:  ldc.i4.1
    IL_0007:  ret
  } // end of method M::f
"""

// The operand's type selects the AsyncHelpers.Await overload; pinning the whole body (not just the
// call line) shows the surrounding shape a runtime reviewer needs (arg load, no spill, no builder).
[<Fact>]
let ``ValueTask operand binds the non-generic Await (full body)`` () =
    compileDirect "let f (vt: ValueTask) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await(vt); 1)"
    |> verifyILContains [ valueTaskAwaitBody ]
    |> shouldSucceed

[<Fact>]
let ``Task<'T> operand binds the generic Await (full body)`` () =
    compileDirect "let f (t: Task<int>) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (let x = AsyncHelpers.Await(t) in x + 1)"
    |> verifyILContains [ genericAwaitBody ]
    |> shouldSucceed

[<Fact>]
let ``suspension lowers to the full Await body with no compiler state machine`` () =
    compileDirect "let f () : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await(Task.Delay(1)); 1)"
    |> verifyILContains [ simpleAwaitBody ]
    |> verifyILNotPresent [
        "AsyncTaskMethodBuilder"
        "IAsyncStateMachine"
    ]

[<Fact>]
let ``the CE builder lowers to Await with no state machine`` () =
    FSharp ceStateMachineSource
    |> withFSharpCoreShippedNet
    |> withLangVersionPreview
    |> compile
    |> verifyILContains [ "AsyncHelpers::UnsafeAwaitAwaiter<valuetype [runtime]System.Runtime.CompilerServices.ValueTaskAwaiter`1<int32>>(!!0)" ]
    |> verifyILNotPresent [
        "AsyncTaskMethodBuilder"
        "IAsyncStateMachine"
    ]

// Runtime-async methods must not emit `.tail`. This body is the contract pin: a function-value call
// in the runtime-async return position is normal (non-tail) so the runtime-async spec's TailRetType
// requirement remains satisfied.
let private tailPrefixBody = """
  .method public static class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
          f(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> g,
            int32 x) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::Delay(int32)
    IL_0006:  call       void [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(class [System.Runtime]System.Threading.Tasks.Task)
    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
    IL_0012:  ret
  } // end of method M::f
"""

[<Fact>]
let ``runtime async avoids a forbidden tail prefix (C1)`` () =
    compileDirect "let f (g: int -> int) (x: int) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await(Task.Delay(1)); g x)"
    |> verifyILContains [ tailPrefixBody ]
    |> shouldSucceed

let private composedLayoutProgram = """
module M
open System
open System.Threading.Tasks
open Microsoft.FSharp.Control.RuntimeAsync.TaskExpressions

let helper x = x * 2

let outer (n: int) : Task<int> =
    let inner y = y + helper n            // nested, non-async local function
    let baseline = inner 10               // non-async code BEFORE the async part
    let work : Task<int> =
        task {
            let mutable total = baseline
            for i in 1 .. n do
                let! d = Task.FromResult i
                total <- total + d
            use _ = { new IAsyncDisposable with member _.DisposeAsync() = ValueTask(Task.Delay 1) }
            try
                do! Task.Delay 1
                total <- total + 1
            finally
                total <- total + 100
            if total > 0 then return total else return -1
        }
    work                                  // non-async code AFTER the async part
"""

// MethodImplOptions.Async (0x2000) is a *method header* flag, not an IL instruction — neither the
// ildasm we use nor the sequence-points decoder render it, so both the composed IL exhibit and the
// sequence-points baseline show the lifted async body as an ordinary `outer@<line>` closure. This
// reads it from metadata and pins the placement: the flag lands only on that lifted `__runtimeAsyncReturn`
// body and never leaks onto the user's own `outer`/`helper` methods just because the async part is
// written lexically inside `outer`. The lifted `Invoke` carries 0x2000 (async).
let private assertAsyncFlagOnLiftedClosureOnly (md: MetadataReader) =
    let asyncBit = 0x2000
    let methods =
        [ for th in md.TypeDefinitions do
            let td = md.GetTypeDefinition th
            let typeName = md.GetString td.Name
            for mh in td.GetMethods() do
                let m = md.GetMethodDefinition mh
                yield typeName, md.GetString m.Name, ((int m.ImplAttributes) &&& asyncBit) <> 0 ]

    let isAsync typeName methodName =
        methods |> List.exists (fun (t, m, a) -> t = typeName && m = methodName && a)

    Assert.False(isAsync "M" "outer", "outer must not carry the async impl flag")
    Assert.False(isAsync "M" "helper", "helper must not carry the async impl flag")
    Assert.True(
        methods |> List.exists (fun (t, _, a) -> a && t.StartsWith "outer@"),
        "the lifted closure holding outer's async body must carry the async impl flag")

[<Fact>]
let ``composed CE body: await inside try, hoisted disposal, async flag on the lifted method`` () =
    FSharp composedLayoutProgram
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldSucceed
    |> verifyILContains [
        "AsyncHelpers::UnsafeAwaitAwaiter<valuetype [runtime]System.Runtime.CompilerServices.ValueTaskAwaiter`1<int32>>(!!0)"
        "AsyncHelpers::UnsafeAwaitAwaiter<valuetype [runtime]System.Runtime.CompilerServices.ValueTaskAwaiter>(!!0)"
        "callvirt   instance valuetype [runtime]System.Threading.Tasks.ValueTask [runtime]System.IAsyncDisposable::DisposeAsync()"
        "call       void [runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(valuetype [runtime]System.Threading.Tasks.ValueTask)"
        "call       class [runtime]System.Runtime.ExceptionServices.ExceptionDispatchInfo [runtime]System.Runtime.ExceptionServices.ExceptionDispatchInfo::Capture(class [runtime]System.Exception)"
        "finally"
    ]
    |> verifyRuntimeAsyncExceptionRegions [ "outer@13::Invoke", true ]
    |> withMetadataReader assertAsyncFlagOnLiftedClosureOnly

// DEMO (auduchinok's sequence-points baseline format): source spans interleaved with the IL that
// implements them, so a large async body is readable and each Await maps to its `do!`/`let!`.
// NOTE: like ildasm, this decoder cannot render the MethodImplOptions.Async flag, so in the .bsl the
// lifted `outer@<line>::Invoke` looks like a plain closure. The flag that actually makes it a
// runtime-async method is asserted on the very same compilation via assertAsyncFlagOnLiftedClosureOnly.
let private composedDirectProgram = """
module M
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let helper x = x * 2

let outer (n: int) : Task<int> =
    let inner y = y + helper n
    let baseline = inner 10
    StateMachineHelpers.__runtimeAsyncReturn (
        let mutable total = baseline
        for i in 1 .. n do
            let d = AsyncHelpers.Await(Task.FromResult i)
            total <- total + d
        AsyncHelpers.Await(Task.Delay 1)
        if total > 0 then total else -1)
"""

[<Fact>]
let ``composed runtime-async body: source-mapped IL (sequence points baseline)`` () =
    FSharp composedDirectProgram
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withPortablePdb
    |> withNoOptimize
    |> compile
    |> shouldSucceed
    |> verifySequencePointsBaseline composedDirectProgram (Path.Combine(runtimeAsyncDir, "ComposedRuntimeAsync.bsl"))
    |> withMetadataReader assertAsyncFlagOnLiftedClosureOnly

[<Theory>]
[<InlineData("await-in-finally",
             "let f () : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (try 1 finally AsyncHelpers.Await(Task.Delay(1))) in f().Result |> ignore")>]
[<InlineData("await-in-catch",
             "let f () : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (try failwith \"boom\" with _ -> AsyncHelpers.Await(Task.Delay(1)); 7) in f().Result |> ignore")>]
let ``exception handling block suspensions compile and run correctly`` (_label: string) (body: string) =
    FSharp(directIntrinsicSource body)
    |> withFSharpCoreShippedNet
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed



[<Theory>]
[<InlineData("byref-across-suspension-in-try-finally",
             "let f (x: byref<int>) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (try AsyncHelpers.Await(Task.Delay(1)); 1 finally AsyncHelpers.Await(Task.FromResult(1)) + x)")>]
[<InlineData("refstruct-across-suspension",
             "let f () : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (let data = [| 10; 20; 30 |] in let span = ReadOnlySpan<int>(data) in AsyncHelpers.Await(Task.Delay(1)); span[0] + span[1] + span[2])")>]
[<InlineData("byref-param-across-suspension",
             "let f (x: byref<int>) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await(Task.Delay(1)); x)")>]
[<InlineData("pinned-local-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in AsyncHelpers.Await(Task.Delay(1)); FSharp.NativeInterop.NativePtr.get p 0)")>]
[<InlineData("pinned-alias-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let q = p in AsyncHelpers.Await(Task.Delay(1)); FSharp.NativeInterop.NativePtr.get q 0)")>]
[<InlineData("pinned-alias-chain-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let p1 = p in let p2 = p1 in AsyncHelpers.Await(Task.Delay(1)); FSharp.NativeInterop.NativePtr.get p2 0)")>]
[<InlineData("pinned-mutable-alias-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let mutable p1 = p in AsyncHelpers.Await(Task.Delay(1)); FSharp.NativeInterop.NativePtr.get p1 0)")>]
[<InlineData("pinned-tuple-alias-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let q = (p, 1) in AsyncHelpers.Await(Task.Delay(1)); FSharp.NativeInterop.NativePtr.get (fst q) 0)")>]
[<InlineData("pinned-captured-lambda-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let q = fun () -> FSharp.NativeInterop.NativePtr.get p 0 in AsyncHelpers.Await(Task.Delay(1)); q())")>]
[<InlineData("pinned-captured-delegate-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let q = System.Func<int>(fun () -> FSharp.NativeInterop.NativePtr.get p 0) in AsyncHelpers.Await(Task.Delay(1)); q.Invoke())",
             Skip = "Known gap: pinned provenance is not propagated through captured delegates")>]
[<InlineData("pinned-ref-cell-across-suspension",
             "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let q = ref p in AsyncHelpers.Await(Task.Delay(1)); FSharp.NativeInterop.NativePtr.get q.Value 0)",
             Skip = "Known gap: pinned provenance is not propagated through ref cells")>]
[<InlineData("byref-across-loop-back-edge",
             "let f (x: byref<int>) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (let mutable i = 0 in let _ = while i < 2 do (AsyncHelpers.Await(Task.Delay(1)); x <- x + 1; i <- i + 1) in 0)")>]
let ``non-preservable values after suspension are rejected`` (_label: string) (body: string) =
    compileDirect body
    |> shouldFail
    |> withErrorCode 3919

[<Fact>]
let ``non-preservable value in an unrelated branch is allowed`` () =
    compileDirect
        "let f (x: int) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (let span = ReadOnlySpan<int>([| 1; 2; 3 |]) in if x > 0 then AsyncHelpers.Await(Task.Delay(1)); x else span[0] + span[1])"
    |> shouldSucceed

[<Fact>]
let ``non-preservable value used before suspension is allowed`` () =
    compileDirect
        "let f () : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (let span = ReadOnlySpan<int>([| 1; 2; 3 |]) in span[0] + AsyncHelpers.Await(Task.FromResult(1)))"
    |> shouldSucceed

[<Fact>]
let ``non-preservable pinned value used before suspension is allowed`` () =
    FSharp(directIntrinsicSource
        "let f (arr: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (use p = fixed arr in let value = FSharp.NativeInterop.NativePtr.get p 0 in AsyncHelpers.Await(Task.Delay(1)); value)"
    )
    |> withFSharpCoreShippedNet
    |> withLangVersionPreview
    |> withNoWarn 9
    |> compile
    |> shouldSucceed

[<Fact>]
let ``captured pinning value is rejected`` () =
    FSharp(directIntrinsicSource
        "let f (arr: int[]) : Task<int> = use p = fixed arr in StateMachineHelpers.__runtimeAsyncReturn (FSharp.NativeInterop.NativePtr.get p 0)"
    )
    |> withFSharpCoreShippedNet
    |> withLangVersionPreview
    |> withNoWarn 9
    |> compile
    |> shouldFail
    |> withErrorCode 406

[<Fact>]
let ``non-preservable value not used after suspension is allowed`` () =
    compileDirect
        "let f (x: byref<int>) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await(Task.Delay(1)); 1)"
    |> shouldSucceed

[<Fact>]
let ``runtime async reports a byref local after suspension once`` () =
    FSharp(
        directIntrinsicSource
            "let f (a: int[]) : Task<int> = StateMachineHelpers.__runtimeAsyncReturn (let p = &a.[0] in AsyncHelpers.Await(Task.Delay(1)); p)"
    )
    |> withOptions [ "--extraoptimizationloops:1" ]
    |> withFSharpCoreShippedNet
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> assertSingleDiagnostic 3919

[<Fact>]
// The CE builder rejects a ref-struct local captured by its continuation lambda (FS0406).
let ``ref struct across a suspension is rejected through the CE builder`` () =
    FSharp refStructAcrossAwaitCE
    |> withFSharpCoreShippedNet
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> withErrorCode 406

#endif
