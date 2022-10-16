
module M
open System
type MyRec = { Foo: string }

let x: int = 1
let y = x.Foo 
let f1 z = z.Foo
let f2 (z: MyRec) = z.Foo


// all give a warning
let x1 = 
   [ (1,2).Item1 
     (1,2).Item2
     (1,2,3).Item1
     (1,2,3).Item2
     (1,2,3).Item3
     (1,2,3,4).Item1
     (1,2,3,4).Item2
     (1,2,3,4).Item3
     (1,2,3,4).Item4
     (1,2,3,4,5).Item1
     (1,2,3,4,5).Item2
     (1,2,3,4,5).Item3
     (1,2,3,4,5).Item4
     (1,2,3,4,5).Item5
     (1,2,3,4,5,6).Item1
     (1,2,3,4,5,6).Item2
     (1,2,3,4,5,6).Item3
     (1,2,3,4,5,6).Item4
     (1,2,3,4,5,6).Item5
     (1,2,3,4,5,6).Item6
     (1,2,3,4,5,6,7).Item1
     (1,2,3,4,5,6,7).Item2
     (1,2,3,4,5,6,7).Item3
     (1,2,3,4,5,6,7).Item4
     (1,2,3,4,5,6,7).Item5
     (1,2,3,4,5,6,7).Item6
     (1,2,3,4,5,6,7).Item7
     (1,2,3,4,5,6,7,8).Item1
     (1,2,3,4,5,6,7,8).Item2
     (1,2,3,4,5,6,7,8).Item3
     (1,2,3,4,5,6,7,8).Item4
     (1,2,3,4,5,6,7,8).Item5
     (1,2,3,4,5,6,7,8).Item6
     (1,2,3,4,5,6,7,8).Item7 ]

let x2 = (1,2,3,4,5,6,7,8).Rest // gives a warning
let x3 = (1,2).Rest // gives an actual error
let x4 = (struct (1,2)).ToTuple() // no error or warning

open System.Runtime.CompilerServices


type TupleEx() =
    [<Extension>]
    static member inline Do((x,y): (int*string)) = ()
    
let x = 1, "2"

x.Do() // no warning
