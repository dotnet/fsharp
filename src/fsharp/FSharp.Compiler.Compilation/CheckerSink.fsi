namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.NameResolution

[<Sealed>]
type internal CheckerSink =

    new: TcGlobals -> CheckerSink

    member Lines: ImmutableArray.Builder<ImmutableArray.Builder<TcSymbolUseData>>

    interface ITypecheckResultsSink
