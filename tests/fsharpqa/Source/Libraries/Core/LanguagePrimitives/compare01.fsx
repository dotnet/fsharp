// #Regression #Libraries #LanguagePrimitives #ReqNOMT 
// Regression test for FSHARP1.0:5640
// This is a sanity test: more coverage in FSHARP suite...
//<Expects status="success">type 'a www = \| W of 'a</Expects>
//<Expects status="success">val p : bool = false</Expects>
//<Expects status="success">val q : bool = true</Expects>
//<Expects status="success">val z : int = 0</Expects>
//<Expects status="success">0</Expects>
//<Expects status="success">val it : unit = \(\)</Expects>

type 'a www = W of 'a
let p = W System.Double.NaN = W System.Double.NaN;;

let q = (W System.Double.NaN).Equals(W System.Double.NaN);;

let z = compare (W System.Double.NaN) (W System.Double.NaN);;

printfn "%A" (if (not p) && q && (z=0) then 0 else 1);;

#q;;


