//This tests the basic functionality of the type forwarder on generic class
// type parameter count > 1

let gc = new BASIC003_Class<int,string>()
let gi = gc :>BASIC003_GI<int,string>
let rv =gc.getValue()+gi.getValue()

System.Console.WriteLine(rv)
exit rv