
module Sync

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the result of a computation, a value of true indicates completion
type SyncStep<'T> = 'T

[<AbstractClass>]
type SyncMachine<'T> =
    new : unit -> SyncMachine<'T>
    abstract Step : pc: int -> SyncStep<'T> 
    member Start: unit -> 'T

type SyncBuilder =
    new: unit -> SyncBuilder
    member inline Combine: task1: SyncStep<unit> * task2: (unit -> SyncStep<'T>) -> SyncStep<'T>
    member inline Delay: f: (unit -> SyncStep<'T>) -> (unit -> SyncStep<'T>)
    member inline For: sequence: seq<'T> * body: ('T -> SyncStep<unit>) -> SyncStep<unit>
    member inline Return: x: 'T -> SyncStep<'T>
    member inline ReturnFrom: task: Task<'T> -> SyncStep<'T>
    member inline Run: code: (unit -> SyncStep<'T>) -> Task<'T>
    member inline TryFinally: body: (unit -> SyncStep<'T>) * fin: (unit -> unit) -> SyncStep<'T>
    member inline TryWith: body: (unit -> SyncStep<'T>) * catch: (exn -> SyncStep<'T>) -> SyncStep<'T>
    member inline Using: disp: 'Resource * body: ('Resource -> SyncStep<'T>) -> SyncStep<'T> when 'Resource :> IDisposable
    member inline While: condition: (unit -> bool) * body: (unit -> SyncStep<unit>) -> SyncStep<unit>
    member inline Zero: unit -> SyncStep<unit>
    member inline Bind : v: 'TResult1 * continuation: ('TResult1 -> SyncStep<'TResult2>) -> SyncStep<'TResult2>
    member inline ReturnFrom: a: 'TResult1 -> SyncStep< 'TResult >

val sync : SyncBuilder

