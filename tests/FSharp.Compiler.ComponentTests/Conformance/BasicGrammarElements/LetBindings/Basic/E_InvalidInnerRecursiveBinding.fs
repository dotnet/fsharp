// #Conformance #DeclarationElements #LetBindings #Regression
//<Expects status="error" id="FS0037" span="(10,13-10,24)">Duplicate definition of value 'foo'</Expects>
// Regression test for bug 6372
type C () =
    override self.ToString() =
        let rec x y = 
            foo "hello"
            y
        and foo (x:string) = printfn "string"            
        and foo (x:obj) = printfn "obj"
        x "Hi"
