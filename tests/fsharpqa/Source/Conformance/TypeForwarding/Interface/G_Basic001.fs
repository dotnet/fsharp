//This tests the basic functionality of the type forwarder on generic interface

let c = new Basic001_Class<int>()
let gi = c :>Basic001_GI<int>
let rv =c.getValue()+gi.getValue()

System.Console.WriteLine(rv)
exit rv
