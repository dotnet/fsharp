// #Regression #Libraries #LanguagePrimitives #ReqNOMT 
// Regression test for FSHARP1.0:5640
// This is a sanity test: more coverage in FSHARP suite...
//<Expects status="success"></Expects>

type 'a www = W of 'a
let p = W System.Double.NaN = W System.Double.NaN             // false (PER semantic)
let q = (W System.Double.NaN).Equals(W System.Double.NaN)     // true (ER semantic)
let z = compare (W System.Double.NaN) (W System.Double.NaN)   // 0

(if (not p) && q && (z=0) then 0 else 1) |> exit
