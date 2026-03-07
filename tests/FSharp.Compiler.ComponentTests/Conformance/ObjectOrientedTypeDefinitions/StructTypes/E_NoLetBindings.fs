// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:3779 and FSHARP1.0:5492
//<Expects status="error" span="(7,5-7,14)" id="FS0901">Structs cannot contain value definitions because the default constructor for structs will not execute these bindings\. Consider adding additional arguments to the primary constructor for the type\.$</Expects>

[<Struct>]
type S( i: int) =
    let i = 0
