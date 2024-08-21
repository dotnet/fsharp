// #Conformance #DeclarationElements #LetBindings #Regression
//<Expects status="error" id="FS0037" span="(8,5-8,8)">Duplicate definition of value 'foo'</Expects>
// Regression test for bug 6372
let rec x y = 
    foo "hello"
    y
and foo (x:string) = printfn "string"            
and foo (x:obj) = printfn "obj"
