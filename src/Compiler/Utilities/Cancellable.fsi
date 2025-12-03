namespace FSharp.Compiler

[<Sealed>]
type Cancellable =
    static member CheckAndThrow: unit -> unit
