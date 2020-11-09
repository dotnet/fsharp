// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2465
//<Expects status="error" id="FS0001" span="(8,16-8,23)">The type 'decimal<Kg>' does not match the type 'decimal'</Expects>
// Used to emit: error FS0001: The type 'decimal<Kg>' does not support any operators named ..
[<Measure>] type Kg

let qq_ok = [ 10M<Kg> .. 10.0M<Kg> .. 100M<Kg> ]
let qq_err = [ 10M<Kg>..100M<Kg> ]
