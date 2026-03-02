namespace RuntimeAsync.Library

open System
open System.Collections.Generic
open System.Threading.Tasks

// ---------------------------------------------------------------------------
// Helper types for IAsyncDisposable and IAsyncEnumerable examples
// ---------------------------------------------------------------------------

/// A simple type that only implements IAsyncDisposable (not IDisposable)
/// for demonstrating async resource cleanup in runtimeTask.
type SimpleAsyncResource() =
    let mutable disposed = false
    member _.IsDisposed = disposed

    member _.DoWorkAsync() = Task.FromResult "async resource used"

    interface IAsyncDisposable with
        member _.DisposeAsync() =
            disposed <- true
            ValueTask.CompletedTask

/// A simple IAsyncEnumerable that yields integers from start to start+count-1.
type AsyncRange(start: int, count: int) =
    interface IAsyncEnumerable<int> with
        member _.GetAsyncEnumerator(_ct) =
            let mutable current = start - 1
            let mutable remaining = count

            { new IAsyncEnumerator<int> with
                member _.Current = current

                member _.MoveNextAsync() =
                    if remaining > 0 then
                        current <- current + 1
                        remaining <- remaining - 1
                        ValueTask<bool>(true)
                    else
                        ValueTask<bool>(false)

                member _.DisposeAsync() = ValueTask.CompletedTask
            }

// ---------------------------------------------------------------------------
// API examples — all use runtimeTask with NO [<MethodImplAttribute>]
// ---------------------------------------------------------------------------

module Api =

    // === Existing examples ===

    let addFromTaskAndValueTask (left: Task<int>) (right: ValueTask<int>) : Task<int> =
        runtimeTask {
            let! l = left
            let! r = right
            return l + r
        }

    let bindUnitTaskAndUnitValueTask () : Task<string> =
        runtimeTask {
            do! Task.CompletedTask
            do! ValueTask.CompletedTask
            return "completed"
        }

    let safeDivide (x: int) (y: int) : Task<int> =
        runtimeTask {
            try
                if y = 0 then
                    failwith "division by zero"

                return x / y
            with _ ->
                return 0
        }

    let nestedRuntimeTask () : Task<int> =
        runtimeTask {
            let! x = addFromTaskAndValueTask (Task.FromResult 20) (ValueTask<int>(22))
            return x + 2
        }

    // === Nested runtimeTask CEs (3 levels deep) ===
    // Each nesting level must be a separate function so that each gets its own 'cil managed async'
    // method. Inline-nested runtimeTask CEs are not supported because the intermediate cast values
    // are not real Tasks and cannot be consumed by Bind's AsyncHelpers.Await.

    let private innerInnerTask () : Task<int> =
        runtimeTask {
            return 10
        }

    let private innerTask () : Task<int> =
        runtimeTask {
            let! b = innerInnerTask ()
            return b + 20
        }

    let deeplyNestedRuntimeTask () : Task<int> =
        runtimeTask {
            let! a = innerTask ()
            return a + 70
        }

    // === Consuming tasks from the standard FSharp.Core task CE ===

    let consumeOlderTaskCE () : Task<int> =
        // Create a task using the standard state-machine based task CE from FSharp.Core
        let standardTask =
            task {
                do! Task.Delay(1)
                return 42
            }

        // Consume it in runtimeTask (runtime-async, no state machine)
        runtimeTask {
            let! result = standardTask
            return result * 2
        }

    // === Task.Delay, Task.Yield, Task.Run ===

    let taskDelayYieldAndRun () : Task<int> =
        runtimeTask {
            // Task.Delay returns Task — bound via do!
            do! Task.Delay(5000)
            // Task.Yield() returns YieldAwaitable — bound via the generic awaitable Bind extension
            do! Task.Yield()
            // Task.Run returns Task<T> — bound via let!
            let! fromRun = Task.Run(fun () -> 7 * 6)
            return fromRun
        }

    // === IAsyncDisposable ===

    let useAsyncDisposable () : Task<string> =
        runtimeTask {
            use resource = new SimpleAsyncResource()
            let! result = resource.DoWorkAsync()
            return result
        }

    // === IAsyncEnumerable ===

    let iterateAsyncEnumerable () : Task<int> =
        runtimeTask {
            let mutable sum = 0

            for x in AsyncRange(1, 5) do
                sum <- sum + x

            return sum
        }

    // === ConfigureAwait ===

    let configureAwaitExample () : Task<int> =
        runtimeTask {
            // ConfigureAwait(false) returns ConfiguredTaskAwaitable<T> — bound via intrinsic Bind
            let! value = (Task.FromResult 99).ConfigureAwait(false)
            // ConfigureAwait on unit Task returns ConfiguredTaskAwaitable — bound via intrinsic Bind
            do! Task.CompletedTask.ConfigureAwait(false)
            return value
        }

    // === Inline-nested runtimeTask CEs ===
    // Inline-nested runtimeTask { ... } CEs (nesting directly inside another runtimeTask { ... })
    // do NOT work with the current inline Run + cast design. The inner CE's cast(raw_value) produces
    // a fake Task<T> that the outer CE's Bind tries to AsyncHelpers.Await — causing NullReferenceException.
    // Workaround: each nesting level must be a separate function so each gets its own 'cil managed async'
    // method that returns a real Task<T>.
    let inlineNestedRuntimeTask () : Task<int> =
        runtimeTask {
            // Calling separate functions that return Task<int> — this works because each function
            // is a real 'cil managed async' method returning a real Task (not a fake cast value).
            let! a = innerInnerTask ()
            let! b = innerTask ()
            return a + b
        }
