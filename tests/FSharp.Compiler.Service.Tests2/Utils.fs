module FSharp.Compiler.Service.Tests.Utils

#nowarn "40"

open System.Collections.Concurrent
open System.Collections.Generic

let memoize<'a, 'b when 'a : equality> f : ('a -> 'b) =
    let y = HashIdentity.Structural<'a>
    let d = new ConcurrentDictionary<'a, 'b>(y)
    fun x -> d.GetOrAdd(x, fun r -> f r)

type FileIdx =
    FileIdx of int
    with
        member this.Idx = match this with FileIdx idx -> idx
        override this.ToString() = this.Idx.ToString()
        static member make (idx : int) = FileIdx idx 
