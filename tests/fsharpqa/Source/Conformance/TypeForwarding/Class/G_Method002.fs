//This tests the basic functionality of the type forwarder on generic class
//non-generic class contains a generic method

let ngc = new Method_Non_Generic()
let rv = ngc.getValue<int>()

exit rv
