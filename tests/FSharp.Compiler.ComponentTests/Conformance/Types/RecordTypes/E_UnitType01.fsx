// #Regression #Conformance #TypesAndModules #Records 
// Field has type 'unit' (which is kind of special)
// Trying to initialize with null is illegal.
// This is regression test for FSHARP1.0:1459
//<Expects id="FS0043" span="(10,15-10,19)" status="error">The type 'unit' does not have 'null' as a proper value</Expects>
#light

type T1 = { u : unit;}

let x = { u = null }
let y = { u = () }
