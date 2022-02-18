// #Regression #NoMT #Printing #RequiresENU  
// Regression test for FSHARP1.0:4068

//<Expects status="success">val lazyExit: Lazy<string> = Value is not created\.</Expects>

let _ = lazy (raise (new System.Exception("LazyValues03 failed - this should never be forced")));;

()