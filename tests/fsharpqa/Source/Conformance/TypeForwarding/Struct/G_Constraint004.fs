//This tests the basic functionality of the type forwarder on generic struct
// constraint:  both type has constraint
// violated constraint

type Test() = 
    member this.Foo() = 12

let gc = new Constraint_BothViolated<Test>()
let rv =gc.getValue()

exit rv
