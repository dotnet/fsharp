module ComputationExpressions.Program

let f0 () =
    let xs = ResizeArray ()
    xs.Add 1
    xs.Add 2
    xs.Add 3
    xs

let xs = f0 ()

let f1 () = resizeArray { 1; 2; 3 }
let f2 () = resizeArray { yield! xs }
let f3 () = resizeArray { for x in xs -> x * x }
