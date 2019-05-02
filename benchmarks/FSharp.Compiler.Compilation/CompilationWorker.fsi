namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library

module internal CompilationWorker =

    /// This is for queueing work from FSharp.Compiler.Private internals.
    /// Eventually, this could go away once we ensure the compiler is thread safe.
    val EnqueueAndAwaitAsync: (CompilationThreadToken -> 'T) -> Async<'T>
