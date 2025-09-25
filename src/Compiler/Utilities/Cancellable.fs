namespace FSharp.Compiler

open Internal.Utilities.Library

[<Sealed>]
type Cancellable =
    static member CheckAndThrow() =
        // If we're not inside an async computation, the ambient cancellation token will be CancellationToken.None and nothing will happen
        // otherwise, if we are inside an async computation, this will throw.
        Async2.CheckAndThrowToken.Value.ThrowIfCancellationRequested()
