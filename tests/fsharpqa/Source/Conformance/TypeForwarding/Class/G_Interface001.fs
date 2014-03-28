//This tests the basic functionality of the type forwarder on generic class
// forwarded class type changed to an interface type

let b = new Interface_Sub<int>()
let rv = b.getValue()

exit rv
