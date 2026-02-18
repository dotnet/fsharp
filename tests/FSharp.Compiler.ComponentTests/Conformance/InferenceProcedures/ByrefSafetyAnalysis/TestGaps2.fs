module TestGaps2

open System

// Scenario 7: Generic constraints
// Can we return a Span<'T> from byref<'T> where 'T is a struct?
let fGeneric<'T when 'T : struct> (x: byref<'T>) = 
    Span<'T>(&x) // Should error

let testGeneric() =
    let mutable x = 1
    let s = fGeneric &x // Should error
    ()

// Scenario 8: Struct Union Types
// Can we hide a Span in a struct union?
// F# struct unions cannot contain ByRefLike types?
// Let's try.
[<Struct>]
type U =
    | Case1 of Span<int>

let fUnion(x: byref<int>) =
    Case1(Span<int>(&x)) // Should error

let testUnion() =
    let mutable x = 1
    let u = fUnion &x // Should error
    ()
