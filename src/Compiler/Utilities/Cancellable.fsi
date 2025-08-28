namespace FSharp.Compiler

open System
open System.Threading

[<Sealed>]
type Cancellable =
    static member internal UseToken: unit -> Async<IDisposable>

    static member HasCancellationToken: bool
    static member Token: CancellationToken

    static member CheckAndThrow: unit -> unit
    static member TryCheckAndThrow: unit -> unit
