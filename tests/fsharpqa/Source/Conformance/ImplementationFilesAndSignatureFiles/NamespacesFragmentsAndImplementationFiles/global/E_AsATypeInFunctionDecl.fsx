// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937, FSHARP1.0:5295
// Usage of 'global' - global is a keyword
//<Expects status="error" id="FS1126" span="(10,12-10,18)">'global' may only be used as the first name in a qualified path</Expects>

// OK
let f (x : global.System.Double) = x + 1.

// Err
let g (x : global) = x + 1.
