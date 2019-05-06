namespace FSharp.Compiler.Compilation

open FSharp.Compiler.TcGlobals
open FSharp.Compiler.NameResolution

[<Sealed>]
type internal CheckerSink =

    new: TcGlobals -> CheckerSink

    interface ITypecheckResultsSink
