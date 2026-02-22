// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:3051, 3144 
//<Expects id="FS0081" span="(7,6-7,7)" status="error">Implicit object constructors for structs must take at least one argument</Expects>
module TestModule

[<Struct>]
type S() =
    interface System.Collections.Generic.IEnumerable<int> with 
        member x.GetEnumerator() = failwith ""
    interface System.Collections.IEnumerable with 
        member x.GetEnumerator() = failwith ""
