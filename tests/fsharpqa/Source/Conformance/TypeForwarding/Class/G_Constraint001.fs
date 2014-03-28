//This tests the basic functionality of the type forwarder on generic class
// constraint:
// origin type has constraint but forwarded type has no

let gc = new Constraint_OnlyOrigin<string>()
let rv =gc.getValue()

exit rv