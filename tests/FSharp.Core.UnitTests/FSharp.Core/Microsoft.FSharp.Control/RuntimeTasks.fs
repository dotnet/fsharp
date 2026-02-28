// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Tests for RuntimeTaskBuilder (runtimeTask computation expression)
// Uses CompilerAssert.ExecuteAux because FSharp.Test.Compiler.FSharp cannot be used from
// FSharp.Core.UnitTests (Range.setTestSource is internal to FSharp.Compiler.Service).
//
// Design notes:
// - The preamble defines RuntimeTaskBuilder where Delay returns unit -> 'T (a thunk)
//   and Run returns 'T (not Task<'T>).
// - Each test wraps the CE in a [<MethodImplAttribute(0x2000)>] function returning Task<'T>.
//   The compiler's special handling for 0x2000 methods type-checks the body against 'T,
//   so runtimeTask { return 1 } (body returning int) is valid inside a Task<int> function.
// - [<NoDynamicInvocation>] on Bind members prevents the F# compiler from adding the
//   cil managed async flag to the Bind IL methods. Without it, the CLR rejects RuntimeTaskBuilder
//   because Bind has the async flag (0x2000) but returns 'U (not Task<'U>).
// - DOTNET_RuntimeAsync=1 is set in the test process; child processes inherit it via
//   psi.EnvironmentVariables (populated from current process env).

namespace FSharp.Core.UnitTests.Control.RuntimeTasks

open System
open Xunit
open FSharp.Test

#if NET10_0_OR_GREATER

module private RuntimeTaskTestHelpers =

    /// Preamble that defines RuntimeTaskBuilder inline.
    /// Delay returns unit -> 'T (a thunk). Run returns 'T (not Task<'T>).
    /// The [<MethodImplAttribute(0x2000)>] goes on the user's wrapper function.
    /// [<NoDynamicInvocation>] on Bind members prevents the F# compiler from adding
    /// cil managed async to the Bind IL methods (which would cause TypeLoadException).
    let private preamble =
        "open System\n" +
        "open System.Runtime.CompilerServices\n" +
        "open System.Threading.Tasks\n" +
        "\n" +
        "#nowarn \"57\"\n" +
        "#nowarn \"42\"\n" +
        "\n" +
        "module internal RuntimeTaskBuilderUnsafe =\n" +
        "    let inline cast<'a, 'b> (a: 'a) : 'b = (# \"\" a : 'b #)\n" +
        "\n" +
        "[<Sealed>]\n" +
        "type RuntimeTaskBuilder() =\n" +
        "    member inline _.Return(x: 'T) : 'T = x\n" +
        "    [<NoDynamicInvocation>]\n" +
        "    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =\n" +
        "        f(AsyncHelpers.Await t)\n" +
        "    [<NoDynamicInvocation>]\n" +
        "    member inline _.Bind(t: Task, [<InlineIfLambda>] f: unit -> 'U) : 'U =\n" +
        "        AsyncHelpers.Await t\n" +
        "        f()\n" +
        "    [<NoDynamicInvocation>]\n" +
        "    member inline _.Bind(t: ValueTask<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =\n" +
        "        f(AsyncHelpers.Await t)\n" +
        "    [<NoDynamicInvocation>]\n" +
        "    member inline _.Bind(t: ValueTask, [<InlineIfLambda>] f: unit -> 'U) : 'U =\n" +
        "        AsyncHelpers.Await t\n" +
        "        f()\n" +
        "    member inline _.Delay(f: unit -> 'T) : unit -> 'T = f\n" +
        "    member inline _.Zero() : unit = ()\n" +
        "    member inline _.Combine((): unit, [<InlineIfLambda>] f: unit -> 'T) : 'T = f()\n" +
        "    member inline _.While([<InlineIfLambda>] guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) : unit =\n" +
        "        while guard() do body()\n" +
        "    [<NoDynamicInvocation>]\n" +
        "    member inline _.For(s: seq<'T>, [<InlineIfLambda>] body: 'T -> unit) : unit =\n" +
        "        for x in s do body(x)\n" +
        "    member inline _.TryWith([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] handler: exn -> 'T) : 'T =\n" +
        "        try body() with e -> handler e\n" +
        "    member inline _.TryFinally([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] comp: unit -> unit) : 'T =\n" +
        "        try body() finally comp()\n" +
        "    [<NoDynamicInvocation>]\n" +
        "    member inline _.Using(resource: 'T when 'T :> IDisposable, [<InlineIfLambda>] body: 'T -> 'U) : 'U =\n" +
        "        try body resource finally (resource :> IDisposable).Dispose()\n" +
        "    // Run returns 'T (not Task<'T>). The [<MethodImplAttribute(0x2000)>] goes on the\n" +
        "    // user's function that wraps the CE call, not here.\n" +
        "    member inline _.Run(f: unit -> 'T) : 'T = f()\n" +
        "\n" +
        "[<AutoOpen>]\n" +
        "module RuntimeTaskBuilderModule =\n" +
        "    let runtimeTask = RuntimeTaskBuilder()\n" +
        "\n"

    /// Helper: compile and run an F# program that uses runtimeTask { }.
    /// DOTNET_RuntimeAsync=1 must be set before calling this so child processes inherit it.
    let runTest (expectedOutputs: string list) (body: string) =
        let source = preamble + body
        let cmpl = Compilation.Create(source, CompileOutput.Exe, options = [| "--langversion:preview"; "--nowarn:3541" |])
        // beforeExecute: copy deps AND FSharp.Core to the output dir so the child process can find them.
        // CompilerAssert.ExecuteAux only copies the explicit deps list, which doesn't include FSharp.Core
        // (it's referenced via -r: in defaultProjectOptions, not as a sub-compilation dep).
        let beforeExecute (outputFilePath: string) (deps: string list) =
            let outputDirectory = System.IO.Path.GetDirectoryName(outputFilePath)
            let copyIfMissing (src: string) =
                let destPath = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileName(src))
                if not (System.IO.File.Exists(destPath)) then
                    System.IO.File.Copy(src, destPath)
            for dep in deps do
                copyIfMissing dep
            // FSharp.Core is not in deps; copy it explicitly.
            let fsharpCoreLocation = typeof<RequireQualifiedAccessAttribute>.Assembly.Location
            copyIfMissing fsharpCoreLocation
        let outcome, stdout, _stderr = CompilerAssert.ExecuteAux(cmpl, ignoreWarnings = true, beforeExecute = beforeExecute, newProcess = true)
        match outcome with
        | Failure exn -> failwith $"Execution failed: {exn.Message}"
        | ExitCode n when n <> 0 -> failwith $"Process exited with code {n}.\nStdout: {stdout}\nStderr: {_stderr}"
        | _ ->
            for expected in expectedOutputs do
                if not (stdout.Contains(expected)) then
                    failwith $"Expected output to contain '{expected}', but got:\n{stdout}"
type SmokeTestsForCompilation() =

    [<FactForNETCOREAPP>]
    member _.tinyRuntimeTask() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> = runtimeTask { return 1 }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tbind() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["2"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        let! x = Task.FromResult(1)
        return 1 + x
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tnested() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let inner () : Task<int> = runtimeTask { return 1 }
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        let! x = inner()
        return x
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tcatch0() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        try
            return 1
        with _ ->
            return 2
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tcatch1() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        try
            let! x = Task.FromResult(1)
            return x
        with _ ->
            return 2
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tbindTask() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["2"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        let! x = Task.FromResult(1)
        return x + 1
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tbindUnitTask() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        do! Task.CompletedTask
        return 1
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tbindValueTask() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["2"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        let! x = ValueTask.FromResult(1)
        return x + 1
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tbindUnitValueTask() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        do! ValueTask.CompletedTask
        return 1
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.twhile() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["5"] """
let mutable i = 0
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        while i < 5 do
            i <- i + 1
        return i
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.tfor() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["done"] """
let mutable total = 0
// Note: Task<unit> return type with For causes InvalidProgramException (compiler generates generic method).
// Use Task<int> with an explicit return to avoid this.
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        for x in [1; 2; 3] do
            total <- total + x
        return total
    }
run().Wait()
printfn "done"
"""

    [<FactForNETCOREAPP>]
    member _.tusing() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["disposed"] """
let mutable disposed = false
// Note: Task<unit> return type with Using causes InvalidProgramException (compiler generates generic method).
// Use Task<int> with an explicit return to avoid this.
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        use _ = { new System.IDisposable with member _.Dispose() = disposed <- true }
        return 0
    }
run().Wait()
if disposed then printfn "disposed"
"""

type Basics() =

    [<FactForNETCOREAPP>]
    member _.testShortCircuitResult() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["3"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        let! x = Task.FromResult(1)
        let! y = Task.FromResult(2)
        return x + y
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.testCatching1() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["x=0 y=1"] """
let mutable x = 0
let mutable y = 0
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        try
            failwith "hello"
            x <- 1
        with _ ->
            ()
        y <- 1
        return 0
    }
run().Wait()
printfn "x=%d y=%d" x y
"""

    [<FactForNETCOREAPP>]
    member _.testCatching2() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["x=0 y=1"] """
let mutable x = 0
let mutable y = 0
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        try
            let! _ = Task.FromResult(0)
            failwith "hello"
            x <- 1
        with _ ->
            ()
        y <- 1
        return 0
    }
run().Wait()
printfn "x=%d y=%d" x y
"""

    [<FactForNETCOREAPP>]
    member _.testNestedCatching() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["inner=1 outer=2"] """
let mutable counter = 1
let mutable caughtInner = 0
let mutable caughtOuter = 0
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let t1 () : Task<int> =
    runtimeTask {
        try
            failwith "hello"
            return 0
        with e ->
            caughtInner <- counter
            counter <- counter + 1
            raise e
            return 0
    }
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let t2 () : Task<int> =
    runtimeTask {
        try
            let! _ = t1()
            return 0
        with _ ->
            caughtOuter <- counter
            return 0
    }
try (t2()).Wait() with _ -> ()
printfn "inner=%d outer=%d" caughtInner caughtOuter
"""

    [<FactForNETCOREAPP>]
    member _.testWhileLoop() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["10"] """
let mutable i = 0
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        while i < 10 do
            i <- i + 1
        return i
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.testTryFinallyHappyPath() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["ran=true"] """
let mutable ran = false
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        try
            let! _ = Task.FromResult(1)
            ()
        finally
            ran <- true
        return 0
    }
run().Wait()
printfn "ran=%b" ran
"""

    [<FactForNETCOREAPP>]
    member _.testTryFinallySadPath() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["ran=true"] """
let mutable ran = false
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        try
            failwith "uhoh"
            return 0
        finally
            ran <- true
    }
try run().Wait() with _ -> ()
printfn "ran=%b" ran
"""

    [<FactForNETCOREAPP>]
    member _.testTryFinallyCaught() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["result=2 ran=true"] """
let mutable ran = false
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        try
            try
                let! _ = Task.FromResult(1)
                failwith "uhoh"
            finally
                ran <- true
            return 1
        with _ ->
            return 2
    }
let t = run()
printfn "result=%d ran=%b" t.Result ran
"""

    [<FactForNETCOREAPP>]
    member _.testUsing() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["disposed=true"] """
let mutable disposed = false
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        use _ = { new System.IDisposable with member _.Dispose() = disposed <- true }
        let! _ = Task.FromResult(1)
        return 1
    }
let t = run()
t.Wait()
printfn "disposed=%b" disposed
"""

    [<FactForNETCOREAPP>]
    member _.testForLoop() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["sum=6"] """
let mutable sum = 0
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        for i in [1; 2; 3] do
            sum <- sum + i
        return sum
    }
let t = run()
t.Wait()
printfn "sum=%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.testExceptionAttachedToTask() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["got exception: boom"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        failwith "boom"
        return 1
    }
let t = run()
try
    t.Wait()
    printfn "no exception"
with
| :? System.AggregateException as ae ->
    printfn "got exception: %s" ae.InnerExceptions.[0].Message
"""

    [<FactForNETCOREAPP>]
    member _.testTypeInference() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["hello"] """
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<string> = runtimeTask { return "hello" }
let t = run()
t.Wait()
printfn "%s" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.testBindAllFourTypes() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["3"] """
// Tests all 4 Bind overloads: Task<T>, Task, ValueTask<T>, ValueTask
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        let! a = Task.FromResult(1)
        do! Task.CompletedTask
        let! b = ValueTask.FromResult(2)
        do! ValueTask.CompletedTask
        return a + b
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.testZeroAndCombine() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
// Zero is called for the `if true then ()` branch (no else), Combine sequences it with `return 1`
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        if true then ()
        return 1
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.testDelay() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["1"] """
// Delay wraps the body in a function; since it's inline, the result is still correct
let mutable x = 0
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        x <- x + 1
        return x
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

    [<FactForNETCOREAPP>]
    member _.testInteropWithTaskCE() =
        Environment.SetEnvironmentVariable("DOTNET_RuntimeAsync", "1")
        RuntimeTaskTestHelpers.runTest ["42"] """
// runtimeTask can bind the result of a task { } computation expression
[<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
let run () : Task<int> =
    runtimeTask {
        let! x = task { return 42 }
        return x
    }
let t = run()
t.Wait()
printfn "%d" t.Result
"""

#endif
