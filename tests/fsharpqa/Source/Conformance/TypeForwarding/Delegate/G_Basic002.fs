//This tests the basic functionality of the generi type forwarder attribute

let c = new Basic002_Class()
let gd = new Basic002_GDele<int>(c.getValue)
let rv = gd.Invoke(1)
System.Console.WriteLine(rv)

exit rv
