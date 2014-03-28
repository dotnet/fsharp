//This tests the basic functionality of the generi type forwarder attribute

let c = new Basic003_Class()
let gd = new Basic003_GDele<int>(c.getValue)
let rv = gd.Invoke(1)

exit rv
