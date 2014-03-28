//This tests the basic functionality of the type forwarder on generic class
// constraint:  origin type has no constraint but forwarded type has
// violated constraint

let gc = new Constraint_OnlyForwarder<string>()
let rv =gc.getValue()

exit rv