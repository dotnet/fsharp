// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSharp1.0:2236 - unexpected "record field" in accessibility error
//<Expects id="FS1096" span="(11,9-11,24)" status="error">The record, struct or class field 'foo' is not accessible from this code location</Expects>

type Foo = 
    class
        val mutable private foo : int
        new() = { foo = 12 }
    end    
    
let f = (new Foo()).foo
