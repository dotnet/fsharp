// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2732

[<Measure>] type Kg
[<Measure>] type s

let v1 = [1.0<Kg> .. 3.0<s>]

let v2 = [1.0<Kg> .. 2.0<s> .. 5.0<Kg>]

//<Expects status="error" id="FS0001" span="(7,11-7,18)">The type 'float<Kg>' does not match the type 'float'</Expects>
//<Expects status="error" id="FS0001" span="(7,22-7,28)">Type mismatch\. Expecting a.    'float<Kg>'    .but given a.    'float<s>'    .The unit of measure 'Kg' does not match the unit of measure 's'</Expects>
//<Expects id="FS0001" span="(9,22-9,28)" status="error">The unit of measure 's' does not match the unit of measure 'Kg'</Expects>
