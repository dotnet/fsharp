// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2920
// missing attribute on type args - on record type
//<Expects status="error" id="FS0702" span="(6,33-6,35)">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
#light
type T_2920<'a> = { x : float32<'a> }
