//This tests the basic functionality of the type forwarder on generic struct
// constraint:  both type has constraint
// vialated constraint

type Test() = 
    member this.Foo() = 12

let gc = new Constraint_BothVialated<Test>()
let rv =gc.getValue()

exit rv
