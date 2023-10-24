module internal FSharp.Compiler.TailCallChecks

open FSharp.Compiler
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// Perform the TailCall analysis on the optimized TAST for a file.
/// The TAST is traversed analogously to the PostInferenceChecks phase.
/// For functions that are annotated with the [<TailCall>] attribute, a warning is emitted if they are called in a
/// non-tailrecursive manner in the recursive scope of the function.
/// The ModuleOrNamespaceContents aren't mutated in any way by performing this check.
val CheckImplFile:
    g: TcGlobals * amap: Import.ImportMap * reportErrors: bool * implFileContents: ModuleOrNamespaceContents -> unit
