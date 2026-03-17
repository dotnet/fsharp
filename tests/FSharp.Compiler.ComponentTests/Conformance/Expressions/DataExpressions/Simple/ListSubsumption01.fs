// #Regression #Conformance #DataExpressions 
// Verify list literals have their type based on the first element of the list.
// (Regression for FSB 1180)

type A() =
    member this.AValue = 1

type B() =
    inherit A()
    member this.BValue = 2

type C() =
    inherit B()
    member this.CValue = 3

let t1 : A list = [ new A(); new B(); new C() ]
let t2 : A list = [ (new C() :> A) ; new B(); new C() ]
let t3 : B list = [ new B(); new C() ]

exit 0
