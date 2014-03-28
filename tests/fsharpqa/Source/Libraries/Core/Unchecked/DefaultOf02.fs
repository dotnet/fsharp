// #Regression #Libraries #Unchecked 
#light

// FSharp1.0:5417 - Unchecked.defaultof<_> on records/unions can cause structural equality check to throw
// Check that Unchecked.defaultof<_> works correctly on various types, mostly structs/unions/records

type R = { x : int; y : string }
type U = | A of int | B of string
type S = struct val mutable x : int end
type C() = class end

let shouldBeTrue = 
    Unchecked.defaultof<R> = Unchecked.defaultof<R>     // Records as null
 && Unchecked.defaultof<U> = Unchecked.defaultof<U>     // Unions  as null
 && Unchecked.defaultof<S> = Unchecked.defaultof<S>     // Structs as null
 && Unchecked.defaultof<C> = Unchecked.defaultof<C>     // Classes as null
 && Unchecked.defaultof<int> = Unchecked.defaultof<int>
