// #Regression #NoMT #Printing  
// Regression test for FSHARP1.0:4068

//<Expects status="success">val lazyExit: Lazy<string> = <unevaluated></Expects>

let _ = lazy (raise (new System.Exception("LazyValues03 failed - this should never be forced")));;

()