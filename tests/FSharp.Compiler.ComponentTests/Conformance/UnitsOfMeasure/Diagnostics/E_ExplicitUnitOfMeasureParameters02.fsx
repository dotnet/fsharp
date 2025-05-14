// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2920
// missing attribute on type args - on return value
//<Expects status="error" id="FS0702" span="(6,45-6,47)">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
#light
let g_2920<(*[<Measure>]*)'a> () : (decimal<'a>) = 1.0<_>
