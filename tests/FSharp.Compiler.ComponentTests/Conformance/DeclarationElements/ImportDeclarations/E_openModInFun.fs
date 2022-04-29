// #Regression #Conformance #DeclarationElements #Import 
//<Expects status="error" span="(9,5-9,9)" id="FS0010">Unexpected keyword 'open' in binding\. Expected incomplete structured construct at or before this point or other token\.$</Expects>
//<Expects status="error" span="(17,9-17,13)" id="FS0010">Unexpected keyword 'open' in binding$</Expects>
//<Expects status="error" span="(23,9-23,13)" id="FS0010">Unexpected keyword 'open' in expression$</Expects>

let f x y =
    let result = x + y
    Console.WriteLine(result.ToString())
    open System
    Console.WriteLine(result.ToString())
    

let top x y =
    let r1 = x + y
    let r2 = x * y
    let nested x y =
        open System
        Console.WriteLine(result.ToString())
    nested r1 r2
    
type Foo() =
    member public this.PrintHello() =
        open System
        Console.WriteLine("Hello!")
