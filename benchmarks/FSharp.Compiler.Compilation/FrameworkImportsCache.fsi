namespace FSharp.Compiler.Service

open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.TcGlobals

/// Global service state
type internal FrameworkImportsCacheKey = (*resolvedpath*)string list * string * (*TargetFrameworkDirectories*)string list* (*fsharpBinaries*)string

type internal FrameworkImportsCache =

    new: keepStrongly: int -> FrameworkImportsCache

    /// Reduce the size of the cache in low-memory scenarios
    member Downsize: CompilationThreadToken -> unit

    /// Clear the cache
    member Clear: CompilationThreadToken -> unit

    member Get: CompilationThreadToken * TcConfig -> Cancellable<TcGlobals * TcImports * AssemblyResolution list * UnresolvedAssemblyReference list>
