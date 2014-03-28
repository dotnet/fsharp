// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2345
//<Expects id="FS0636" span="(12,9-12,16)" status="error">Units-of-measure supported only on float, float32, decimal and signed integer types</Expects>
//<Expects id="FS0636" span="(13,9-13,15)" status="error">Units-of-measure supported only on float, float32, decimal and signed integer types</Expects>
//<Expects id="FS0636" span="(14,9-14,16)" status="error">Units-of-measure supported only on float, float32, decimal and signed integer types</Expects>
//<Expects id="FS0636" span="(15,9-15,16)" status="error">Units-of-measure supported only on float, float32, decimal and signed integer types</Expects>

[<Measure>] type Kg

let x = 1<Kg>    // ok

let a = 1ul<Kg>     // unsigned int64
let b = 1u<Kg>      // unsigned int=int32
let c = 1us<Kg>     // unsigned int16
let d = 1uy<Kg>     // unsigned int8






