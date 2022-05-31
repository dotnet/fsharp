// #Regression #NoMT #Printing #RequiresENU  
// Regression test for FSHARP1.0:4068
// Title: printing lazy property values forces the lazy value

//<Expects status="success">val lazy12: Lazy<int> = Value is not created\.</Expects>
//<Expects status="success">val it: Lazy<int> = Value is not created\.</Expects>
//<Expects status="success">val it: Lazy<int> = Value is not created\.</Expects>
//<Expects status="success">val it: Lazy<int> = Value is not created\.</Expects>
//<Expects status="success">val it: Lazy<int> = Value is not created\.</Expects>
//<Expects status="success">val it: Lazy<int> = Value is not created\.</Expects>

let lazy12 = lazy 12;;
lazy12;;
lazy12;;
lazy 13;;
lazy 13;;
it;;
printfn "Finished";;
