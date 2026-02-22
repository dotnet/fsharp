// #Regression #Conformance #TypeInference 
// Regression tests for FSHARP1.0:1348, FSHARP1.0:2949,FSHARP1.0:4927,FSHARP1.0:
// Overloads that differ for the return type only are not allowed (in general)

[<AbstractClass>]
type T() = 
    abstract X : unit -> decimal
    abstract X : unit -> int

type TT() = inherit T() with
                override x.X () = 2.5m
                override x.X () = 1

let tt = new TT()

//<Expects status="error" span="(7,14-7,15)" id="FS0438">Duplicate method\. The method 'X' has the same name and signature as another method in type 'T'\.$</Expects>
//<Expects status="error" span="(11,28-11,29)" id="FS0438">Duplicate method\. The method 'X' has the same name and signature as another method in type 'TT'\.$</Expects>
