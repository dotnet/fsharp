// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:2097
// Make sure we can assign to a static field imported from a C# assembly
//<Expects status="success"></Expects>
#light

// Retrieve initial value
let before = C.d

// Update value
C.d <- -3.4M

(if (before = 1.2M && C.d = -3.4M) then 0 else 1) |> exit
