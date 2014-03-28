//This tests the basic functionality of the type forwarder on generic class
// constraint:  both type has constraint
// vialated constraint

let gc = new Constraint_BothVialated<string>()
let rv =gc.getValue()

exit rv
