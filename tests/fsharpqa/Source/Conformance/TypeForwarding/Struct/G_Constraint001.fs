//This tests the basic functionality of the type forwarder on generic struct
// constraint:
// origin type has constraint but forwarded type has no

type Test() = 
    member this.Foo() = 12

let gc = new Constraint_OnlyOrigin<System.Guid>()
let rv =gc.getValue()

exit rv
