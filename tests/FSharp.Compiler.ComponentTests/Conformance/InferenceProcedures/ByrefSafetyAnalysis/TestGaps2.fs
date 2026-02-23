module TestGaps2

open System

// Scenario 7: Generic constraints
// Span<'T> in a struct union triggers FS0412 (byref-like in type instantiation)
let fGeneric<'T when 'T : struct> (x: byref<'T>) = 
    Span<'T>(&x)

let testGeneric() =
    let mutable x = 1
    let s = fGeneric &x
    ()

// Scenario 8: Struct Union Types
// F# struct unions cannot contain ByRefLike types — FS0412
[<Struct>]
type U =
    | Case1 of Span<int>

let fUnion(x: byref<int>) =
    Case1(Span<int>(&x))

let testUnion() =
    let mutable x = 1
    let u = fUnion &x
    ()
