//This tests the basic functionality of the type forwarder on generic class
//non-generic class contains a generic method

let b = new TurnToInterface_Sub<int>()
let rv = b.getValue()

exit rv
