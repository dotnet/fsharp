// #Regression #NoMT #Printing  
// Regression test for FSHARP1.0:4068

//<Expects status="success">val lazyExit: Lazy<string> = <unevaluated></Expects>

let lazyExit = lazy (exit 1; "this should never be forced");;

exit 0;;
