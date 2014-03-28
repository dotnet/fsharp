//This tests the basic functionality of the generi type forwarder attribute

let c = new Basic001_Class()
let gd = new Basic001_GDele<int>(c.getValue)
let rv = gd.Invoke(1)

exit rv
