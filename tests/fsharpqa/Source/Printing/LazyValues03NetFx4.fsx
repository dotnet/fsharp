// #Regression #NoMT #Printing #RequiresENU  
// Regression test for FSHARP1.0:4068

//<Expects status="success">val lazyExit: Lazy<string> = Value is not created\.</Expects>

let lazyExit = lazy (exit 1; "this should never be forced");;

exit 0;;
