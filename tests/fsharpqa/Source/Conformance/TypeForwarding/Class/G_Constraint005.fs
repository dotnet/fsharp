//This tests the basic functionality of the type forwarder on generic class
// constraint:  both type has constraint
// non-violated constraint


type Test() = 
    member this.Foo() = 12

let gc = new Constraint_BothNonViolated<Test>()
let rv =gc.getValue()

exit rv
