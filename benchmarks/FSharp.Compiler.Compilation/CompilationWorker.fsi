namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library

module CompilationWorker =

    val EnqueueAsync: (CompilationThreadToken -> Async<'T>) -> Async<'T>
