namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library

module CompilationWorker =

    val EnqueueAndAwaitAsync: (CompilationThreadToken -> 'T) -> Async<'T>
