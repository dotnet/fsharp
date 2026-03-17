// #Regression #NoMT #FSI 
// Regression for FSB 3594
// Verify System.Core.dll is referenced in FSI by default

//<Expects status="success">val a: System\.Action<unit></Expects>
//<Expects status="success">stuff</Expects>
//<Expects status="success">val it: unit = \(\)</Expects>
//<Expects status="success">val hs: Collections\.Generic\.HashSet<int></Expects>
//<Expects status="success">type A = Action<int></Expects>
//<Expects status="success">type B = Action<int,int></Expects>

// Use Action
open System
let a = new Action<_>(fun () -> printfn "stuff");;


a.Invoke();;

// ---------------------------------------------------------------------------------------

// Use HashSet
open System.Collections.Generic
let hs = new HashSet<_>([1 .. 10]);;

type A = System.Action<int>
type B = System.Action<int,int>;;

exit 0;;
