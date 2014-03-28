// #Regression #Conformance #TypeInference 
// Regression tests for FSHARP1.0:1348, FSHARP1.0:2949,FSHARP1.0:4927,FSHARP1.0:5939
// Overloads that differ for the return type only are not allowed (in general)

//<Expects status="error" span="(27,11-27,16)" id="FS0041">A unique overload for method 'X' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: member Q\.X : unit -> decimal, member Q\.X : unit -> float$</Expects>



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

