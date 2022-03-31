// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:2919
// Make sure the generic type variable is echoed back
// (notice that the next time we evaluate 'f' this
// goes back to 'u)
//<Expects status="success">val f: x: float+</Expects>
let f(x:float<'a>) = x*x;;
printfn "Finished"
