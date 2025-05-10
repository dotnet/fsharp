// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:2919
// Make sure the generic type variable is echoed back
// (notice that the next time we evaluate 'f' this
// goes back to 'u, 'v)
// This is the case where the generic function takes 2 args
//<Expects status="success">val g: x: float+</Expects>
let g (x:float<'a>) (y:float32<'b>) = x * float y;;
printfn "Finished"
