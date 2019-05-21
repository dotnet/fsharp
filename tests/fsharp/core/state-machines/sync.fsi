
module Tests.SyncBuilder

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open FSharp.Core
open FSharp.Core.CompilerServices
open FSharp.Control
open FSharp.Collections

[<AbstractClass>]
type SyncMachine<'T> =
    new : unit -> SyncMachine<'T>
    abstract Step : unit -> 'T
    member Start: unit -> 'T

type SyncBuilder =
    new: unit -> SyncBuilder
    member inline Combine: task1: unit * task2: (unit -> 'T) -> 'T
    member inline Delay: f: (unit -> 'T) -> (unit -> 'T)
    member inline For: sequence: seq<'T> * body: ('T -> unit) -> unit
    member inline Return: x: 'T -> 'T
    member inline Run: code: (unit -> 'T) -> 'T
    member inline TryFinally: body: (unit -> 'T) * fin: (unit -> unit) -> 'T
    member inline TryWith: body: (unit -> 'T) * catch: (exn -> 'T) -> 'T
    member inline Using: disp: 'Resource * body: ('Resource -> 'T) -> 'T when 'Resource :> IDisposable
    member inline While: condition: (unit -> bool) * body: (unit -> unit) -> unit
    member inline Zero: unit -> unit
    member inline Bind : v: 'TResult1 * continuation: ('TResult1 -> 'TResult2) -> 'TResult2

val sync : SyncBuilder

