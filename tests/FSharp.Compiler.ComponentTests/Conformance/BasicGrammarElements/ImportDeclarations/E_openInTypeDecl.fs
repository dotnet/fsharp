// #Regression #Conformance #DeclarationElements #Import 
//<Expects status="error" span="(7,5-7,9)" id="FS0010">Unexpected keyword 'open' in member definition$</Expects>

type Foo() =
    let x = 42
    
    open System
    let timeConstructed = DateTime.Now.Ticks
