namespace Microsoft.FSharp.Core

open System

[<AbstractClass; Sealed>]
type internal ThreadSafeRandom =
    static member Shared: Random
