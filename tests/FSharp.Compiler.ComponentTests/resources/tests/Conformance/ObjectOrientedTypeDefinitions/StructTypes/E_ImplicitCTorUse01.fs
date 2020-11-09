// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:5304
//<Expects span="(7,9-7,13)" status="error" id="FS0688">The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$</Expects>

type U2(v:list<int>) = struct end 

let x = U2()              // expect an error here because "list<int>" does not admit a default value

