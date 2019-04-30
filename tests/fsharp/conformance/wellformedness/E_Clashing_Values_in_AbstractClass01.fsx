// #Regression #Conformance #TypeInference 
// Regression tests for FSHARP1.0:1348, FSHARP1.0:2949,FSHARP1.0:4927,FSHARP1.0:5939
// Overloads that differ for the return type only are not allowed (in general)
[<AbstractClass>]
type T() = 
    abstract X : unit -> decimal
    abstract X : unit -> int

type TT() = inherit T() with
                override x.X () = 2.5m
                override x.X () = 1

let tt = new TT()

type Q() = class
             member x.X () = 1.0
             member x.X () = 2.5m
           end

let q = new Q()

(if 1.0 = q.X() then 0 else 1) |> exit

