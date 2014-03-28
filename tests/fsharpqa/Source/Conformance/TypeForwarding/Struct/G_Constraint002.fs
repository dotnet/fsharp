//This tests the basic functionality of the type forwarder on generic struct
// constraint:  origin type has no constraint but forwarded type has
// violated constraint

type Test() = 
    member this.Foo() = 12

let gc = new Constraint_OnlyForwarder<Test>()
let rv =gc.getValue()

exit rv
