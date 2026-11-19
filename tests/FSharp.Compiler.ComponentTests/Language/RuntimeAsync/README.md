# Runtime-async sample builders

These computation-expression builders are built on the compiler's runtime-async intrinsics (see [docs/runtime-async.md](../../../../docs/runtime-async.md)). They are test fixtures and examples, not FSharp.Core APIs.

## `runtimeTask` ([RuntimeTaskBuilder.fs](RuntimeTaskBuilder.fs))

Produces hot `Task<'T>` and `ValueTask<'T>` values, like `task { }`. `Source` overloads turn tasks, custom awaitables and `Async<'T>` into started delegates, so `Bind` and `MergeSources` need no per-type overloads. `and!` starts every source before awaiting any of them.

## `async2` ([Async2Builder.fs](Async2Builder.fs))

Produces cold, cancellable `Async2<'T>` computations. The cancellation token is threaded through the computation, checked between steps, and passed to cold sources such as `Async<'T>` and other `async2` computations. `and!` starts the right source before awaiting the left.

## `asyncSeq2` ([AsyncSeq2Builder.fs](AsyncSeq2Builder.fs))

Produces `IAsyncEnumerable<'T>` using `__runtimeAsyncSequence`.

* `let!` and `do!` accept tasks, value tasks, custom awaitables, `async2` and `Async` computations, and `CancellationToken -> Task` factories. Cold sources receive the enumeration's cancellation token.
* `for` and `yield!` accept both `seq<'T>` and `IAsyncEnumerable<'T>`.
* One driver enumerator runs nested `yield!` sources in a loop, so deep recursion does not grow the call stack.
* A `yield!` in tail position (`YieldFromFinal`) hands off to the new source and releases the enumerator it came from. A `yield!` that isn't in tail position keeps its parent enumerator until the nested source finishes.
* A second `MoveNextAsync`, or a `DisposeAsync`, while a move is pending throws `InvalidOperationException`.

## Tests

* [RuntimeTasks.fs](RuntimeTasks.fs): the FSharp.Core `task { }` tests, ported to `runtimeTask`.
* [EnumerableTests.fs](EnumerableTests.fs) and [RuntimeAsyncEnumerableTests.fs](RuntimeAsyncEnumerableTests.fs): `asyncSeq2` behavior, including `try/with`, cancellation and recursion.
* [RuntimeAsyncSequence.fs](RuntimeAsyncSequence.fs): a small `asyncSeq2` usage and lifecycle example.
