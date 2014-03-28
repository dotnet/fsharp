// replacements for classes which are entirely removed from the portable profile
// either calls out to hooks, or implements functionality locally for simple cases

namespace System.Diagnostics

open System

type Stopwatch() = 
    let mutable startTime = DateTime.Now

    member this.Start() = 
        startTime <- DateTime.Now

    static member StartNew() = 
        let timer = Stopwatch()
        timer.Start()
        timer

    member this.Reset() =
        startTime <- DateTime.Now

    member this.ElapsedMilliseconds with get() = int64 (DateTime.Now - startTime).TotalMilliseconds

    member this.Elapsed with get() = DateTime.Now - startTime