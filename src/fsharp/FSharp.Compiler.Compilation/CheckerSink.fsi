namespace FSharp.Compiler.Compilation

open FSharp.Compiler.TcGlobals
open FSharp.Compiler.NameResolution

[<Sealed>]
type internal CheckerSink =

    new: TcGlobals -> CheckerSink

    member TryFindSymbolUseData: line: int * column: int -> TcSymbolUseData option

    interface ITypecheckResultsSink
