// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression tests for FSHARP1.0:2737
// Miscellaneous negative tests - the positive tests listed
// in the bug are already part of the FSHARP suite.
//<Expects status="error" id="FS0039" span="(11,20-11,26)">The type 'string' is not defined</Expects>

//<Expects status="error" id="FS0039" span="(17,16-17,23)">The value, namespace, type or module 'Array2D' is not defined</Expects>


let f (x : string) = ()         // ok
let f' (x : global.string) = ()    // err

let _ = typeof<int>             // ok
let _ = global.typeof<int>      // ok

let _ = Array2D.init            // ok
let _ = global.Array2D.init     // err

let _ = <@@ global.System.Double.Epsilon @@>    // ok
let _ = <@ global.System.Double.Epsilon @>      // ok

let _ = async { do () }                         // ok
let _ = global.async { do () }                  // ok
