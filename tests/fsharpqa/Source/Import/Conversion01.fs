// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:5673
//<Expects status="success"></Expects>

let t = new CSharpTypes.T()
let p = ( char t, double t, int t, byte t)

exit <| if ('a', 2.0, 1, 1uy) = p then 0 else 1

//let q = ( float32 t)
