
module Test

[<Struct;CLIMutable>]
type StructRecord =
    {
        X: float
        Y: float
    }

let x = StructRecord ()






type T = X<__SOURCE_DIRECTORY__>


open System.Collections.Generic

let f (x: List<'T>) = 
    use mutable d = x.GetEnumerator() // no warning expected here!
    while (d.MoveNext() ) do 
       ()
    ()

