# Runtime-async sample builders

The `async2` sample uses the compiler's [runtime-async intrinsics](../../../../docs/runtime-async.md). It is a test fixture, not a FSharp.Core API.

## `async2` ([Async2Builder.fs](Async2Builder.fs))

Produces cold, cancellable `Async2<'T>` computations. The cancellation token is threaded through the computation, checked between steps, and passed to cold sources such as `Async<'T>` and other `async2` computations. `and!` starts the right source before awaiting the left.

## Tests

* [RuntimeTasks.fs](../../../FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Control/RuntimeTasks.fs) and [TaskSequences.fs](../../../FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Control/TaskSequences.fs): FSharp.Core runtime builder tests.
* [TaskSequenceExpressions.fs](../../../FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Control/TaskSequenceExpressions.fs): FSharp.Core `taskSeq` computation expressions.
* [RuntimeAsyncAsyncLocal.fs](RuntimeAsyncAsyncLocal.fs): execution-context propagation through `task` and `async2`.
