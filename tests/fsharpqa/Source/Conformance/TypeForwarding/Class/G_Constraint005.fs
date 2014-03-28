//This tests the basic functionality of the type forwarder on generic class
// constraint:  both type has constraint
// non-vialated constraint


type Test() = 
    member this.Foo() = 12

let gc = new Constraint_BothNonVialated<Test>()
let rv =gc.getValue()

exit rv
