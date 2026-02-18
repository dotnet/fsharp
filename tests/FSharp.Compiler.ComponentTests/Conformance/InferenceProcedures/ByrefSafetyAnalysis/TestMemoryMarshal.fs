module TestMemoryMarshal

open System
open System.Runtime.InteropServices

let testMarshal() =
    let mutable x = 1
    let s = MemoryMarshal.CreateSpan(&x, 1)
    s
