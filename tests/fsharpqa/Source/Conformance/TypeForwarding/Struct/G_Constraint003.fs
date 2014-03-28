//This tests the basic functionality of the type forwarder on generic struct
// constraint:  both type has constraint
// same constraint

type Test() = 
    member this.Foo() = 12

let gc = new Constraint_Both<System.Guid>()
let rv =gc.getValue()

exit rv
