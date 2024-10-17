//This tests the basic functionality of the type forwarder on generic class
// different type parameter name
// type parameter count > 1

let gc = new Basic_DiffName004<int,string>()
let rv =gc.getValue()

exit rv