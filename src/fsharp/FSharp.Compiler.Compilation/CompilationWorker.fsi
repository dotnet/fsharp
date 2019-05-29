namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library

/// This is for queueing work from FSharp.Compiler.Private internals.
/// Eventually, this could go away once we ensure the compiler is thread safe.
module internal CompilationWorker =

    val EnqueueAndAwaitAsync: (CompilationThreadToken -> 'T) -> Async<'T>

    val EnqueueAsyncAndAwaitAsync: (CompilationThreadToken -> Async<'T>) -> Async<'T>
