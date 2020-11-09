// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2920
// missing attribute on type args - on function argument
//<Expects status="error" span="(6,41-6,43)" id="FS0702">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
#light
let f_2920<(*[<Measure>]*)'a>(x:decimal<'a>) = x
