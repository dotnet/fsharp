// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Regression test for FSharp1.0:4973
// Title: extension methods and overloading

open System.Collections.Generic

type IEnumerable<'a> with
    member x.map f = 
        x |> Seq.map f

type 'a ``[]`` with
    member x.map f = 
        x |> Array.map f

let a = [| 1; 2; 3 |]
let s = a :> seq<_>
let inc x = x + 1
let ra = a.map inc  // compile error, overload ambiguity (array or seq?)
let rs = s.map inc  // ok (seq)
