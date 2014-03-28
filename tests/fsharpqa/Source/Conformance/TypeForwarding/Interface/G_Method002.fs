//This tests the basic functionality of the type forwarder on generic class
//non-generic class contains a generic method

type Test() = 
    member this.Foo() = 12

let ngc = new NonGenericClass()
let ngi = ngc :>Method_Non_Generic
let rv =ngc.getValue()+ngi.getValue()

System.Console.WriteLine(rv)
exit rv
