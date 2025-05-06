// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2920
// missing attribute on type args - type abbreviation
//<Expects status="error" id="FS0702" span="(6,27-6,29)">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
#light
type Q_2920<'a> = float32<'a>
