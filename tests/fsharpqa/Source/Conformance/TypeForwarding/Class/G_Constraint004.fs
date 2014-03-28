//This tests the basic functionality of the type forwarder on generic class
// constraint:  both type has constraint
// same constraint

let gc = new Constraint_Both<string>()
let rv =gc.getValue()

exit rv
