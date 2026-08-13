module Language.RuntimeAsyncEdgeCaseTests

// Edge-case tests for runtime-async, inspired by the Roslyn async2 test surface (try/catch,
// try/finally, loops, Task vs ValueTask, ref-struct/ReadOnlySpan, disposal). Two halves:
//   * Execution facts (runtime behavior) — driven through the runtimeTask CE builder, which is
//     treated as a hypothetical library (RuntimeAsync/RuntimeTaskBuilder.fs).
//   * EmittedIL facts — assert the runtime-async codegen contract so dotnet/runtime folks can be
//     pointed at concrete IL. Every asserted substring was captured from the PR's own fsc on the
//     pinned net11 preview and normalized the way ILChecker does ([System.Runtime] -> [runtime]).
//
// IL facts that assert the *absence* of a token use direct StateMachineHelpers.__runtimeAsync
// sources (single async method, clean assembly), because ILChecker's NotPresent check is
// assembly-scoped and the CE builder's own inline members legitimately contain `tail.`/`MoveNext`.
// The CE `Run` lowers `do!`/`let!` to exactly this intrinsic form (see the execution facts).
//
// The "undiagnosed forbidden pattern" facts below pin restrictions that docs/runtime-async.md
// records as known and currently NOT diagnosed by the F# compiler (tail./localloc forbidden;
// suspension forbidden inside EH regions; byref/byref-like locals not preservable across a
// suspension). They compile clean today; the comments record the observed runtime outcome.

open Xunit
open FSharp.Test.Compiler
open System.IO
open System.Reflection.Metadata

let private runtimeAsyncDir = Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync")
let private builderPath = Path.Combine(runtimeAsyncDir, "RuntimeTaskBuilder.fs")

// Builds a minimal direct-intrinsic compilation unit: the module header plus the opens every
// StateMachineHelpers.__runtimeAsync body needs, then the supplied one-liner body. Used for the
// shape / absence / undiagnosed-pattern assertions, which must run on a single-method assembly.
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
    FSharp(directIntrinsicSource body) |> withLangVersionPreview |> compile

// ---- CE-builder sources (compiled against RuntimeTaskBuilder.fs, the hypothetical library) -------

// ref-struct-across-suspension written through the CE builder: the `do!` desugars to a continuation
// lambda that captures the span, so F#'s byref-capture check rejects it (FS0406).
let private refStructAcrossAwaitCE = """
module M
open System
open System.Threading.Tasks
open RuntimeTaskBuilder.RuntimeTask
let f () : Task<int> =
    runtimeTask {
        let data = [| 10; 20; 30 |]
        let span = ReadOnlySpan<int>(data)
        do! Task.Delay(1)
        return span[0] + span[1] + span[2]
    }
"""

let private ceStateMachineSource = """
module CeUser
open System.Threading.Tasks
open RuntimeTaskBuilder.RuntimeTask
let f () : Task<int> =
    runtimeTask {
        let! x = Task.FromResult 41
        return x + 1
    }
"""

#if NETCOREAPP

// ============================ execution (runtime behavior) ============================

[<Fact>]
let ``runtime async edge cases execute through the CE builder`` () =
    FsFromPath builderPath
    |> withAdditionalSourceFile (SourceFromPath (Path.Combine(runtimeAsyncDir, "RuntimeAsyncEdgeCases.fs")))
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

// ============================ emitted IL (codegen contract) ============================
//
// Each fact below pins the *entire* method body in scope (not a single line): the normalized
// `.method { ... }` block captured from the PR's own fsc on the pinned net11 preview. This hands
// dotnet/runtime reviewers the exact lowering to check against docs/runtime-async.md. The blocks
// are deliberately brittle across SDK/codegen bumps — a codegen exhibit is supposed to change when
// the codegen changes. ILChecker.compareIL normalizes both sides identically ([System.Runtime] ->
// [runtime], collapses the multi-line `.method` signature, strips comments), so the verbatim
// ildasm text is what we assert.

// `Await(Task); 1` — direct call to the non-generic Await overload, then push 1 and ret.
let private simpleAwaitBody = """
  .method public static class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
          f() cil managed noinlining
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
          f(class [System.Runtime]System.Threading.Tasks.Task`1<int32> t) cil managed noinlining
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
          f(valuetype [System.Runtime]System.Threading.Tasks.ValueTask vt) cil managed noinlining
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
    compileDirect "let f (vt: ValueTask) : Task<int> = StateMachineHelpers.__runtimeAsync (AsyncHelpers.Await(vt); 1)"
    |> verifyILContains [ valueTaskAwaitBody ]
    |> shouldSucceed

[<Fact>]
let ``Task<'T> operand binds the generic Await (full body)`` () =
    compileDirect "let f (t: Task<int>) : Task<int> = StateMachineHelpers.__runtimeAsync (let x = AsyncHelpers.Await(t) in x + 1)"
    |> verifyILContains [ genericAwaitBody ]
    |> shouldSucceed

[<Fact>]
let ``suspension lowers to the full Await body with no compiler state machine`` () =
    compileDirect "let f () : Task<int> = StateMachineHelpers.__runtimeAsync (AsyncHelpers.Await(Task.Delay(1)); 1)"
    |> verifyILContains [ simpleAwaitBody ]
    |> verifyILNotPresent [
        "AsyncTaskMethodBuilder"
        "IAsyncStateMachine"
    ]

[<Fact>]
// The runtimeTask CE lowers `let!`/`do!` to Await calls without a compiler-generated state machine
// (unlike the task { } builder). This ties the hypothetical library to the runtime-async contract.
let ``the CE builder lowers to Await with no state machine`` () =
    FsFromPath builderPath
    |> withAdditionalSourceFile (FsSource ceStateMachineSource)
    |> withLangVersionPreview
    |> compile
    |> verifyILContains [ "AsyncHelpers::Await<int32>(class [runtime]System.Threading.Tasks.Task`1<!!0>)" ]
    |> verifyILNotPresent [
        "AsyncTaskMethodBuilder"
        "IAsyncStateMachine"
    ]

// CONTRACT VIOLATION (finding C1): the tail-position function-value call is emitted with a `tail.`
// prefix (IL_000d) inside a runtime-async body. The runtime-async contract forbids `tail.`, and
// ilverify reports `TailRetType` on this exact method. Pinning the whole body makes the offending
// prefix unambiguous; once IlxGen.CanTailcall learns about runtime-async the `IL_000d: tail.` line
// must disappear and this expected body must be updated.
let private tailPrefixBody = """
  .method public static class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
          f(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32> g,
            int32 x) cil managed noinlining
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::Delay(int32)
    IL_0006:  call       void [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(class [System.Runtime]System.Threading.Tasks.Task)
    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  tail.
    IL_000f:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::Invoke(!0)
    IL_0014:  ret
  } // end of method M::f
"""

[<Fact>]
let ``runtime async currently emits a forbidden tail prefix (C1)`` () =
    compileDirect "let f (g: int -> int) (x: int) : Task<int> = StateMachineHelpers.__runtimeAsync (AsyncHelpers.Await(Task.Delay(1)); g x)"
    |> verifyILContains [ tailPrefixBody ]
    |> shouldSucceed

// ============== async impl-flag placement — F# source layout is not authoritative ==============
//
// The async marking is a *method* impl flag (MethodImplAttributes 0x2000, MethodImplOptions.Async;
// il.fs WithAsync), written into the emitted PE method header. It is NOT rendered by the ildasm we
// use, so it can only be checked from metadata — hence withMetadataReader rather than verifyIL.
//
// This is the composed / non-trivial case the trivial single-method exhibits above do not cover: a
// larger user function (`outer`) that has a nested local function (`inner`), non-async code before
// and after, and a runtimeTask CE that itself combines control flow (`for`, `let!`, `use`,
// `try/finally`, `do!`, `if`). The property being pinned: the 0x2000 flag lands on the emitted IL
// method that actually holds the async body (a compiler-generated closure the CE body is lifted
// into), and NOT on the enclosing `outer` just because the CE is written lexically inside it.
//
// Empirically on the pinned net11 preview (implAttrs, async = 0x2000 bit):
//   M::helper            0x0000  async=false
//   M::outer             0x0000  async=false   <- encloses the CE, but is NOT the async method
//   outer@NN::Invoke     0x2008  async=true    <- lifted async body carries the flag (+noinlining)
//   RuntimeTaskBuilder::Run  0x2008  async=true (the intrinsic-bodied inline builder member)
let private composedLayoutProgram = """
module M
open System
open System.Threading.Tasks
open RuntimeTaskBuilder.RuntimeTask

let helper x = x * 2

let outer (n: int) : Task<int> =
    let inner y = y + helper n            // nested, non-async local function
    let baseline = inner 10               // non-async code BEFORE the async part
    let work : Task<int> =
        runtimeTask {
            let mutable total = baseline
            for i in 1 .. n do
                let! d = Task.FromResult i
                total <- total + d
            use _ = { new IDisposable with member _.Dispose() = () }
            try
                do! Task.Delay 1
                total <- total + 1
            finally
                total <- total + 100
            if total > 0 then return total else return -1
        }
    work                                  // non-async code AFTER the async part
"""

// Full emitted IL of the composed program, so dotnet/runtime reviewers can read the exact lowering.
//
// (1) `M::outer` -- the *enclosing* user function. It is a plain, non-async method: it runs the
// pre-async code (`baseline = inner 10`, i.e. `10 + helper n`) inline, constructs the closure that
// holds the async body, and tail-calls its `Invoke`. It returns `Task<int>` but carries NO async
// impl flag -- proof that the async marking follows the emitted async method, not the F# function the
// CE is lexically written in. (It is even allowed `tail.` here precisely because it is not async.)
let private composedOuterBody = """
  .method public static class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
          outer(int32 n) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4.2
    IL_0004:  mul
    IL_0005:  add
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void M/outer@13::.ctor(int32,
                                                         int32)
    IL_000e:  ldnull
    IL_000f:  tail.
    IL_0011:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [System.Runtime]System.Threading.Tasks.Task`1<int32>>::Invoke(!0)
    IL_0016:  ret
  } // end of method M::outer
"""

// (2) `outer@13::Invoke` -- the compiler-generated closure the CE body is lifted into: THIS is the
// runtime-async method (the one carrying MethodImplAttributes.Async, checked via metadata below).
// Its body is the full composed lowering the runtime must honour:
//   * a `for` loop (blt.s / bne.un.s) around a suspension,
//   * `AsyncHelpers::Await<int32>(Task<int32>)` -- the generic await intrinsic (the `let!`),
//   * a try/finally where `do!` suspends via `AsyncHelpers::Await(Task)` INSIDE the try while the
//     finally does only arithmetic (no await in a finally region -- that is forbidden),
//   * the `use` disposal lowered as isinst IAsyncDisposable -> DisposeAsync(); Await(ValueTask),
//     else isinst IDisposable -> Dispose(), emitted in straight-line code AFTER the protected region
//     (the builder hoists the awaited DisposeAsync out of the finally, by design), and
//   * the final `if total > 0 then ... else -1`.
let private composedAsyncClosureBody = """
    .method public strict virtual instance class [System.Runtime]System.Threading.Tasks.Task`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unit) cil managed noinlining
    {
      // Code size       231 (0xe7)
      .maxstack  7
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2,
               class [System.Runtime]System.Threading.Tasks.Task`1<int32> V_3,
               int32 V_4,
               class [System.Runtime]System.IDisposable V_5,
               class [System.Runtime]System.Exception V_6,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_7,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_8,
               class [System.Runtime]System.Threading.Tasks.Task V_9,
               class [System.Runtime]System.Exception V_10,
               object V_11,
               class [System.Runtime]System.IAsyncDisposable V_12,
               class [System.Runtime]System.IAsyncDisposable V_13,
               class [System.Runtime]System.IDisposable V_14,
               class [System.Runtime]System.IDisposable V_15)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 M/outer@13::baseline
      IL_0006:  stloc.0
      IL_0007:  ldc.i4.1
      IL_0008:  stloc.2
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 M/outer@13::n
      IL_000f:  stloc.1
      IL_0010:  ldloc.1
      IL_0011:  ldloc.2
      IL_0012:  blt.s      IL_0032

      IL_0014:  ldloc.2
      IL_0015:  call       class [System.Runtime]System.Threading.Tasks.Task`1<!!0> [System.Runtime]System.Threading.Tasks.Task::FromResult<int32>(!!0)
      IL_001a:  stloc.3
      IL_001b:  ldloc.3
      IL_001c:  call       !!0 [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await<int32>(class [System.Runtime]System.Threading.Tasks.Task`1<!!0>)
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloc.0
      IL_0024:  ldloc.s    V_4
      IL_0026:  add
      IL_0027:  stloc.0
      IL_0028:  ldloc.2
      IL_0029:  ldc.i4.1
      IL_002a:  add
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  ldloc.1
      IL_002e:  ldc.i4.1
      IL_002f:  add
      IL_0030:  bne.un.s   IL_0014

      IL_0032:  newobj     instance void M/'outer@18-1'::.ctor()
      IL_0037:  stloc.s    V_5
      .try
      {
        .try
        {
          IL_0039:  ldc.i4.1
          IL_003a:  call       class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::Delay(int32)
          IL_003f:  stloc.s    V_9
          IL_0041:  ldloc.s    V_9
          IL_0043:  call       void [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(class [System.Runtime]System.Threading.Tasks.Task)
          IL_0048:  ldloc.0
          IL_0049:  ldc.i4.1
          IL_004a:  add
          IL_004b:  stloc.0
          IL_004c:  leave.s    IL_0054

        }  // end .try
        finally
        {
          IL_004e:  ldloc.0
          IL_004f:  ldc.i4.s   100
          IL_0051:  add
          IL_0052:  stloc.0
          IL_0053:  endfinally
        }  // end handler
        IL_0054:  ldloc.0
        IL_0055:  ldc.i4.0
        IL_0056:  ble.s      IL_005b

        IL_0058:  ldloc.0
        IL_0059:  br.s       IL_005c

        IL_005b:  ldc.i4.m1
        IL_005c:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!0,!1> class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::NewChoice1Of2(!0)
        IL_0061:  stloc.s    V_8
        IL_0063:  leave.s    IL_007a

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0065:  castclass  [System.Runtime]System.Exception
        IL_006a:  stloc.s    V_10
        IL_006c:  ldloc.s    V_10
        IL_006e:  stloc.s    V_6
        IL_0070:  ldnull
        IL_0071:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!0,!1> class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::NewChoice2Of2(!1)
        IL_0076:  stloc.s    V_8
        IL_0078:  leave.s    IL_007a

      }  // end handler
      IL_007a:  ldloc.s    V_8
      IL_007c:  stloc.s    V_7
      IL_007e:  ldloc.s    V_5
      IL_0080:  box        [System.Runtime]System.IDisposable
      IL_0085:  stloc.s    V_11
      IL_0087:  ldloc.s    V_11
      IL_0089:  isinst     [System.Runtime]System.IAsyncDisposable
      IL_008e:  stloc.s    V_12
      IL_0090:  ldloc.s    V_12
      IL_0092:  brfalse.s  IL_00a6

      IL_0094:  ldloc.s    V_12
      IL_0096:  stloc.s    V_13
      IL_0098:  ldloc.s    V_13
      IL_009a:  callvirt   instance valuetype [System.Runtime]System.Threading.Tasks.ValueTask [System.Runtime]System.IAsyncDisposable::DisposeAsync()
      IL_009f:  call       void [System.Runtime]System.Runtime.CompilerServices.AsyncHelpers::Await(valuetype [System.Runtime]System.Threading.Tasks.ValueTask)
      IL_00a4:  br.s       IL_00c0

      IL_00a6:  ldloc.s    V_11
      IL_00a8:  isinst     [System.Runtime]System.IDisposable
      IL_00ad:  stloc.s    V_14
      IL_00af:  ldloc.s    V_14
      IL_00b1:  brfalse.s  IL_00c0

      IL_00b3:  ldloc.s    V_14
      IL_00b5:  stloc.s    V_15
      IL_00b7:  ldloc.s    V_15
      IL_00b9:  callvirt   instance void [System.Runtime]System.IDisposable::Dispose()
      IL_00be:  br.s       IL_00c0

      IL_00c0:  ldloc.s    V_6
      IL_00c2:  stloc.s    V_10
      IL_00c4:  ldloc.s    V_10
      IL_00c6:  brtrue.s   IL_00ca

      IL_00c8:  br.s       IL_00cd

      IL_00ca:  ldloc.s    V_10
      IL_00cc:  throw

      IL_00cd:  ldloc.s    V_7
      IL_00cf:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
      IL_00d4:  brfalse.s  IL_00d8

      IL_00d6:  br.s       IL_00e5

      IL_00d8:  ldloc.s    V_7
      IL_00da:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
      IL_00df:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::get_Item()
      IL_00e4:  ret

      IL_00e5:  ldc.i4.0
      IL_00e6:  ret
    } // end of method outer@13::Invoke
"""

[<Fact>]
let ``async impl flag is on the lifted async method, not the enclosing F# function`` () =
    let asyncBit = 0x2000
    FsFromPath builderPath
    |> withAdditionalSourceFile (FsSource composedLayoutProgram)
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed
    |> verifyILContains [ composedOuterBody; composedAsyncClosureBody ]
    |> withMetadataReader (fun md ->
        let methods =
            [ for th in md.TypeDefinitions do
                let td = md.GetTypeDefinition th
                let typeName = md.GetString td.Name
                for mh in td.GetMethods() do
                    let m = md.GetMethodDefinition mh
                    yield typeName, md.GetString m.Name, ((int m.ImplAttributes) &&& asyncBit) <> 0 ]

        let isAsync typeName methodName =
            methods |> List.exists (fun (t, m, a) -> t = typeName && m = methodName && a)

        // The user functions are plain IL methods — the async flag must NOT leak onto them just
        // because the CE (or a call to an async helper) appears lexically inside `outer`.
        Assert.False(isAsync "M" "outer", "outer must not carry the async impl flag")
        Assert.False(isAsync "M" "helper", "helper must not carry the async impl flag")

        // The CE body is lifted into a compiler-generated closure (name `outer@<line>`); that
        // emitted method is the one that carries the async flag.
        let liftedIsAsync =
            methods |> List.exists (fun (t, _, a) -> a && t.StartsWith "outer@")
        Assert.True(liftedIsAsync, "the lifted closure holding outer's async body must carry the async impl flag"))

// DEMO (auduchinok's sequence-points baseline format): source spans interleaved with the IL that
// implements them, so a large async body is readable and each Await maps to its `do!`/`let!`.
let private composedDirectProgram = """
module M
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let helper x = x * 2

let outer (n: int) : Task<int> =
    let inner y = y + helper n
    let baseline = inner 10
    StateMachineHelpers.__runtimeAsync (
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
    |> withPortablePdb
    |> withNoOptimize
    |> compile
    |> shouldSucceed
    |> verifySequencePointsBaseline composedDirectProgram (Path.Combine(runtimeAsyncDir, "ComposedRuntimeAsync.bsl"))
    |> ignore


// Each pattern is contract-forbidden but compiles with NO diagnostic today; docs/runtime-async.md
// records them as known, currently-undiagnosed restrictions. Not executed here (the observed runtime
// result is a hard process crash); the row comments record the observed symptom. C# rejects the
// analogues at compile time (await-in-finally/catch; CS4007 for ref-struct; CS1988 for byref).
[<Theory>]
[<InlineData("await-in-finally",
             "let f () : Task<int> = StateMachineHelpers.__runtimeAsync (try 1 finally AsyncHelpers.Await(Task.Delay(1)))")>] // runtime: fail-fast 0xC0000409 / SIGSEGV
[<InlineData("await-in-catch",
             "let f () : Task<int> = StateMachineHelpers.__runtimeAsync (try failwith \"boom\" with _ -> AsyncHelpers.Await(Task.Delay(1)); 7)")>] // runtime: crash
[<InlineData("refstruct-across-suspension",
             "let f () : Task<int> = StateMachineHelpers.__runtimeAsync (let data = [| 10; 20; 30 |] in let span = ReadOnlySpan<int>(data) in AsyncHelpers.Await(Task.Delay(1)); span[0] + span[1] + span[2])")>] // runtime: IndexOutOfRangeException (C14)
[<InlineData("byref-param-across-suspension",
             "let f (x: byref<int>) : Task<int> = StateMachineHelpers.__runtimeAsync (AsyncHelpers.Await(Task.Delay(1)); x)")>] // byref read after suspension; C# gives CS1988
let ``contract-forbidden suspension pattern compiles with no diagnostic`` (_label: string) (body: string) =
    compileDirect body
    |> shouldSucceed

[<Fact>]
// Positive counterpart to the ref-struct row above: the same code through the CE builder IS rejected,
// because the continuation lambda captures the ref-struct local (FS0406). The CE provides a safety
// net that the delegate-free intrinsic does not.
let ``ref struct across a suspension is rejected through the CE builder`` () =
    FsFromPath builderPath
    |> withAdditionalSourceFile (FsSource refStructAcrossAwaitCE)
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> withErrorCode 406

#endif
