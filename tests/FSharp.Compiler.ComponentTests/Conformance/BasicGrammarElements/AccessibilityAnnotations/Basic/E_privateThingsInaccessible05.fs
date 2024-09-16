// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSharp1.0:2236 - unexpected "record field" in accessibility error


type Foo = 
    class
        val mutable private foo : int
        new() = { foo = 12 }
    end    
    
let f = (new Foo()).foo
