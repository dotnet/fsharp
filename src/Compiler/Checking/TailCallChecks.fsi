module internal FSharp.Compiler.TailCallChecks

open FSharp.Compiler
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val CheckImplFile:
    g: TcGlobals *
    amap: Import.ImportMap *
    reportErrors: bool *
    implFileContents: ModuleOrNamespaceContents *
    _extraAttribs: 'a ->
        unit
