module internal FSharp.Compiler.TailCallChecks

open FSharp.Compiler
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// Perform the TailCall analysis on the optimized TAST for a file.
val CheckImplFile:
    g: TcGlobals * amap: Import.ImportMap * reportErrors: bool * implFileContents: ModuleOrNamespaceContents -> unit
