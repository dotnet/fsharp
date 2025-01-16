namespace FSharp.Compiler

open System
open System.Threading

[<Sealed>]
type Cancellable =
    // For testing only.
    static member internal SetTokenForTesting: CancellationToken -> unit
    static member internal UseToken: unit -> Async<unit>
    static member internal UsingToken: Async<'T> -> Async<'T>

    static member HasCancellationToken: bool
    static member Token: CancellationToken

    static member CheckAndThrow: unit -> unit
    static member TryCheckAndThrow: unit -> unit
