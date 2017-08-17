// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSharp1.0:3762 - Using FastFunc explicitly is not differentiate from function types, thus causing compiler to create bad method tables, maybe other problems
//<Expects span="(7,17-7,20)" id="FS0438" status="error">Duplicate method\. The method 'Foo' has the same name and signature as another method in type 'SomeClass'\.</Expects>
// Note: as of Beta2, FastFunc became FSharpFunc
type SomeClass() =

    member this.Foo (x:int->int) =
        printfn "method 1"
    
    // Overloaded method signature is the same as previous, error expected
    member this.Foo (x:FSharpFunc<int,int>) = 
        printfn "method 2"
