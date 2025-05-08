// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2345
//<Expects id="FS0636" span="(9,9-9,15)" status="error">Units-of-measure are only supported on float, float32, decimal, and integer types.</Expects>

[<Measure>] type Kg

let x = 1<Kg>    // ok

let a = 1I<Kg> // BigInt
