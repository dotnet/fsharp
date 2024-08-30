//This tests the basic functionality of the type forwarder on generic struct
// different type parameter name

let gc = new Basic_DiffName<string>()
let rv =gc.getValue()

exit rv
