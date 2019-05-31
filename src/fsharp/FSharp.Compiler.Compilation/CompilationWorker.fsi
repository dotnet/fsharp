namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library

/// This is for queueing work from FSharp.Compiler.Private internals.
module internal CompilationWorker =

    val Enqueue: (CompilationThreadToken -> unit) -> unit

    val EnqueueAndAwaitAsync: (CompilationThreadToken -> 'T) -> Async<'T>

    val EnqueueAsyncAndAwaitAsync: (CompilationThreadToken -> Async<'T>) -> Async<'T>
