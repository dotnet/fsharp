//This tests the basic functionality of the type forwarder on generic class
// different type parameter name

let gc = new Basic002_Class<int>()
let gi = gc :>Basic002_GI<int>
let rv =gc.getValue()+gi.getValue()

System.Console.WriteLine(rv)
exit rv
