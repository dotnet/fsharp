//This tests the basic functionality of the type forwarder on generic class
// constraint:  origin type has no constraint but forwarded type has
// non-violated constraint

let gc = new Constraint_NonViolatedForwarder<string>()
let rv =gc.getValue()

exit rv
