// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5056
// F# expands unit-of-measure abbreviations unnecessarily
//<Expects status="success">val it: float<N> \* float<N> = \(2\.0, 2\.0\)</Expects>

[<Measure>] type kg
[<Measure>] type m 
[<Measure>] type s 
[<Measure>] type N = kg m / s^2
let f (x:float<'u>) = (x,x);;

f 2.0<N>;;

exit 0;;
