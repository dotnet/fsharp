namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System

module MethodImplAttribute =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> compile
        |> verifyILBaseline

    // SOURCE=MethodImplAttribute.ForwardRef.fs        SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.ForwardRef.dll"	# MethodImplAttribute.ForwardRef.fs
    [<Theory; FileInlineData("MethodImplAttribute.ForwardRef.fs")>]
    let ``ForwardRef_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.InternalCall.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.InternalCall.dll"	# MethodImplAttribute.InternalCall.fs
    [<Theory; FileInlineData("MethodImplAttribute.InternalCall.fs")>]
    let ``InternalCall_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoInlining.fs        SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoInlining.dll"	# MethodImplAttribute.NoInlining.fs
    [<Theory; FileInlineData("MethodImplAttribute.NoInlining.fs")>]
    let ``NoInlining_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
     
    [<Theory; FileInlineData("MethodImplAttribute.NoInlining_InlineKeyword.fs")>]
    let ``NoInlining_fs with inline keyword => should warn in preview version`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion80
        |> typecheck
        |> withSingleDiagnostic (Warning 3151, Line 3, Col 12, Line 3, Col 19, "This member, function or value declaration may not be declared 'inline'")
    
    // SOURCE=MethodImplAttribute.AggressiveInlining.fs SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.AggressiveInlining.dll"	# MethodImplAttribute.AggressiveInlining.fs
    [<Theory; FileInlineData("MethodImplAttribute.AggressiveInlining.fs")>]
    let ``AggressiveInlining_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoOptimization.fs    SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoOptimization.dll"	# MethodImplAttribute.NoOptimization.fs
    [<Theory; FileInlineData("MethodImplAttribute.NoOptimization.fs")>]
    let ``NoOptimization_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.PreserveSig.fs       SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.PreserveSig.dll"	# MethodImplAttribute.PreserveSig.fs
    [<Theory; FileInlineData("MethodImplAttribute.PreserveSig.fs")>]
    let ``PreserveSig_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Synchronized.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Synchronized.dll"	# MethodImplAttribute.Synchronized.fs
    [<Theory; FileInlineData("MethodImplAttribute.Synchronized.fs")>]
    let ``Synchronized_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Unmanaged.fs         SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Unmanaged.dll"	# MethodImplAttribute.Unmanaged.fs
    [<Theory; FileInlineData("MethodImplAttribute.Unmanaged.fs")>]
    let ``Unmanaged_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // =====================================================================================
    // Task 14: IL baseline tests for RuntimeAsync feature
    // Verify that [<MethodImpl(MethodImplOptions.Async)>] emits 'cil managed async' in IL
    // =====================================================================================

    // Verify that a simple async method with MethodImplOptions.Async emits the async IL flag.
    // The body returns int directly (runtime-async: body is type-checked against T, not Task<T>).
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - method with Async attribute emits cil managed async in IL``() =
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncMethod () : Task<int> = 42
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // The method must carry the 'async' flag in its IL method header
            """cil managed async"""
        ]

    // Verify that the async flag is present on a method returning Task (non-generic).
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - Task-returning method emits cil managed async in IL``() =
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncVoidMethod () : Task = ()
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """cil managed async"""
        ]

    // =====================================================================================
    // Task 15: Validation error tests for RuntimeAsync feature
    // =====================================================================================

    // Error 3885: async method cannot also be synchronized (0x2020 = 0x2000 | 0x20)
    [<Fact>]
    let ``RuntimeAsync - error when async method is also synchronized``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices
open System.Threading.Tasks

// Note: 0x2020 = Async (0x2000) + Synchronized (0x20)
[<MethodImplAttribute(enum<MethodImplOptions>(0x2020))>]
let invalidMethod () : Task<int> = Task.FromResult(42)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "cannot also use"

    // Error 3884: async method must return Task, Task<T>, ValueTask, or ValueTask<T>
    [<Fact>]
    let ``RuntimeAsync - error when async method does not return a Task type``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices

[<MethodImplAttribute(enum<MethodImplOptions>(0x2000))>]
let invalidMethod () : int = 42
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "must return Task"

    // Feature gate: using MethodImplOptions.Async without --langversion:preview emits a preview feature error
    [<Fact>]
    let ``RuntimeAsync - error when Async attribute used without preview langversion``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(enum<MethodImplOptions>(0x2000))>]
let asyncMethod () : Task<int> = Task.FromResult(42)
"""
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "runtime async"

    // =====================================================================================
    // Task 16: Behavioral tests for RuntimeAsync feature
    // Verify that methods with [<MethodImpl(0x2000)>] actually execute correctly at runtime
    // =====================================================================================

    // Behavioral test: simple return — method body returns T directly, Task<T> is produced by runtime
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - behavioral test: simple return``() =
        // DOTNET_RuntimeAsync=1 must be set in the test process before the compiled assembly is loaded.
        // Setting it inside the compiled code is too late (the CLR loads the type before any code runs).
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncReturn42 () : Task<int> = 42

let result = asyncReturn42().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // Behavioral test: await Task<T> — method awaits a Task<int> and returns the result
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - behavioral test: await Task<T>``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncAwaitTask () : Task<int> = AsyncHelpers.Await(Task.FromResult(42))

let result = asyncAwaitTask().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // Behavioral test: await Task (unit) — method awaits a non-generic Task
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - behavioral test: await Task (unit)``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncAwaitUnitTask () : Task = AsyncHelpers.Await(Task.CompletedTask)

asyncAwaitUnitTask().Wait()
printfn "done"
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["done"]

    // Behavioral test: await ValueTask<T> — method awaits a ValueTask<int>
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - behavioral test: await ValueTask<T>``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncAwaitValueTask () : Task<int> = AsyncHelpers.Await(ValueTask.FromResult(42))

let result = asyncAwaitValueTask().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // Behavioral test: multiple awaits — method awaits two tasks and adds results
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - behavioral test: multiple awaits``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncMultipleAwaits () : Task<int> =
    let a = AsyncHelpers.Await(Task.FromResult(10))
    let b = AsyncHelpers.Await(Task.FromResult(32))
    a + b

let result = asyncMultipleAwaits().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // =====================================================================================
    // Task 18: Edge case tests for RuntimeAsync feature
    // =====================================================================================

    // Edge case: generic method — method is generic over the awaited type
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - edge case: generic method``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let genericAsync<'T> (t: Task<'T>) : Task<'T> = AsyncHelpers.Await t

let result = genericAsync(Task.FromResult(42)).Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // Edge case: try/with — method uses try/with and succeeds (no exception)
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - edge case: try/with success``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncTryWith () : Task<int> =
    try AsyncHelpers.Await(Task.FromResult(42))
    with _ -> -1

let result = asyncTryWith().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // Edge case: try/with exception — method catches an exception and returns fallback
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - edge case: try/with exception``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncTryWithException () : Task<int> =
    try failwith "oops"
    with _ -> -1

let result = asyncTryWithException().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["-1"]

    // Edge case: interop with task CE — method awaits a task CE result
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - edge case: interop with task CE``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
[<RuntimeAsync>]
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncInteropWithTaskCE () : Task<int> =
    let t = task { return 42 }
    AsyncHelpers.Await t

let result = asyncInteropWithTaskCE().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // RuntimeAsync attribute on builder class implicitly applies NoDynamicInvocation to all
    // public inline members. Their IL bodies are replaced with 'throw NotSupportedException'.
    // Uses the new cast-free builder: Delay is 'fun () -> f()' and Run is non-inline with
    // [<MethodImplAttribute(0x2000)>] — no cast helper or sentinel needed in user code.
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - implicit NoDynamicInvocation on builder inline members``() =
        FSharp """
module TestModule

#nowarn "57"
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Control

[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    member inline _.Return(x: 'T) : 'T = x
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : unit -> Task<'T> = fun () -> f()
    member inline _.Zero() : unit = ()
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member _.Run(f: unit -> Task<'T>) : Task<'T> = AsyncHelpers.Await(f())

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    let runtimeTask = RuntimeTaskBuilder()
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Bind's IL body must be replaced with throw NotSupportedException
            // (implicit NoDynamicInvocation from [<RuntimeAsync>] on the declaring type)
            "\"Dynamic invocation of Bind is not supported\""
        ]

    // With the cast-free builder, Run is non-inline [<MethodImplAttribute(0x2000)>] and is
    // 'cil managed async'. The Delay closure also becomes 'cil managed async' via the
    // auto-injected sentinel. 'cil managed async' appears in the IL output for both.
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - consumer function gets cil managed async without MethodImpl attribute``() =
        FSharp """
module TestModule

#nowarn "57"
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Control

[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    member inline _.Return(x: 'T) : 'T = x
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : unit -> Task<'T> = fun () -> f()
    member inline _.Zero() : unit = ()
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member _.Run(f: unit -> Task<'T>) : Task<'T> = AsyncHelpers.Await(f())

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    let runtimeTask = RuntimeTaskBuilder()

// No [<MethodImplAttribute(0x2000)>] on the consumer — Run carries 'cil managed async'.
// The Delay closure is also 'cil managed async' due to auto-injected sentinel.
let myConsumer () : Task<int> =
    runtimeTask {
        let! x = Task.FromResult(42)
        return x
    }
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Run is cil managed async (MethodImplOptions.Async), and the Delay closure
            // Invoke is also cil managed async (auto-injected sentinel ensures cloIsAsync=true).
            """cil managed async"""
        ]

    // Behavioral test: consumer function using cast-free [<RuntimeAsync>] builder executes correctly.
    // Run is non-inline [<MethodImplAttribute(0x2000)>]. Delay is 'fun () -> f()' (no cast helper).
    // The compiler auto-injects the sentinel and handles 'T → Task<'T> bridging automatically.
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - behavioral test: consumer with RuntimeAsync builder``() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        FSharp """
module TestModule

#nowarn "57"
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Control

[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    member inline _.Return(x: 'T) : 'T = x
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : unit -> Task<'T> = fun () -> f()
    member inline _.Zero() : unit = ()
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member _.Run(f: unit -> Task<'T>) : Task<'T> = AsyncHelpers.Await(f())

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    let runtimeTask = RuntimeTaskBuilder()

let myConsumer () : Task<int> =
    runtimeTask {
        let! x = Task.FromResult(21)
        let! y = Task.FromResult(21)
        return x + y
    }

let result = myConsumer().Result
printfn "%d" result
"""
        |> withLangVersionPreview
        |> compileExeAndRunNewProcess
        |> shouldSucceed
        |> withOutputContainsAllInOrder ["42"]

    // =====================================================================================
    // Task 8: Cast-free builder architecture tests
    // Verify that the compiler's automatic bridging works: Delay uses 'fun () -> f()' with
    // no cast helper, and the Delay closure is emitted as 'cil managed async' by the compiler.
    // =====================================================================================

    // Verify that a [<RuntimeAsync>] builder with cast-free Delay ('fun () -> f()') compiles
    // successfully and the Delay closure's Invoke is emitted as 'cil managed async'.
    // The compiler auto-injects the sentinel into the Delay closure body and handles the
    // 'T → Task<'T> return-type bridging automatically — no cast helper required.
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - cast-free Delay closure is emitted as cil managed async``() =
        FSharp """
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Control

[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    member inline _.Return(x: 'T) : 'T = x
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    // Cast-free Delay: compiler handles 'T -> Task<'T> bridging and injects sentinel automatically.
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : unit -> Task<'T> = fun () -> f()
    member inline _.Zero() : unit = ()
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member _.Run(f: unit -> Task<'T>) : Task<'T> = AsyncHelpers.Await(f())

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    let runtimeTask = RuntimeTaskBuilder()

let useBuilder () : Task<int> =
    runtimeTask {
        let! x = Task.FromResult(42)
        return x
    }
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // The Delay closure's Invoke must be 'cil managed async':
            // the compiler auto-injects AsyncHelpers.Await(ValueTask.CompletedTask) sentinel,
            // which sets cloIsAsync=true, causing EraseClosures.fs to emit async Invoke.
            """cil managed async"""
        ]

    // Verify that a CE with no async operations ('runtimeTask { return 42 }') still produces
    // a Delay closure emitted as 'cil managed async'. The compiler auto-injects the sentinel
    // into ALL Delay closures for [<RuntimeAsync>] builders, even when the body has no let!/do!.
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - CE with no async ops produces cil managed async closure via sentinel``() =
        FSharp """
module TestModule

#nowarn "57"
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Control

[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    member inline _.Return(x: 'T) : 'T = x
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : unit -> Task<'T> = fun () -> f()
    member inline _.Zero() : unit = ()
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member _.Run(f: unit -> Task<'T>) : Task<'T> = AsyncHelpers.Await(f())

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    let runtimeTask = RuntimeTaskBuilder()

// No let!/do! — pure return. Sentinel injection ensures the Delay closure is still async.
let pureReturn () : Task<int> =
    runtimeTask {
        return 42
    }
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Even with no AsyncHelpers.Await in the user CE body, the compiler-injected
            // sentinel (AsyncHelpers.Await(ValueTask.CompletedTask)) forces cloIsAsync=true,
            // so the Delay closure Invoke is still emitted as 'cil managed async'.
            """cil managed async"""
        ]
