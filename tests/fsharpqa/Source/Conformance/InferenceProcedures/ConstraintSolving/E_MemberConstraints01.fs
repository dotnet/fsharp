// #Regression #Conformance #TypeInference #TypeConstraints 
// Regression test for FSharp1.0:2262
// We should emit an error, not ICE
//<Expects id="FS0697" span="(6,42-6,75)" status="error">Invalid constraint</Expects>

let inline length (x: ^a) : int = (^a : (member Length : int with get, set) (x, ()))
