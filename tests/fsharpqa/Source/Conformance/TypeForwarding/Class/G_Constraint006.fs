//This tests the basic functionality of the type forwarder on generic class
// constraint:  both type has constraint
// violated constraint

let gc = new Constraint_BothViolated<string>()
let rv =gc.getValue()

exit rv
