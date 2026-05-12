// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:5405
// We should not generate bad code. Instead, we should give an error
//<Expects status="error" id="FS0037" span="(9,23-9,24)">Duplicate definition of field 'x'$</Expects>

[<Struct>]
type RIP(x:int) =
   [<DefaultValue>]
   static val mutable x : RIP
