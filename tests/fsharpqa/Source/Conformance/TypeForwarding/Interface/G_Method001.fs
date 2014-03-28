//This tests the basic functionality of the type forwarder on generic class
//forwarded type doesn't contain the method

type Test() = 
    member this.Foo() = 12

let gc = new GenericClass<Test>()
let gi = gc :>Method_NotInForwarder<Test>
let rv =gc.getValue()+gi.getValue()

System.Console.WriteLine(rv)
exit rv
