// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:3051
//<Expects status="success"></Expects>
module TestModule

[<Struct>]
type S(x:int) =
    interface System.Collections.Generic.IEnumerable<int> with 
        member x.GetEnumerator() = failwith ""
    interface System.Collections.IEnumerable with 
        member x.GetEnumerator() = failwith ""
