module SrtpTest.Program

open SrtpTest.Types
open SrtpTest.Operations

[<EntryPoint>]
let main _argv =
    let v1 = { X = 1.0; Y = 2.0 }
    let v2 = { X = 3.0; Y = 4.0 }
    let added = v1 + v2
    let summed = sum [v1; v2; { X = 5.0; Y = 6.0 }]
    let d = dot v1 v2
    printfn "Added: (%f, %f)" added.X added.Y
    printfn "Sum: (%f, %f)" summed.X summed.Y
    printfn "Dot: %f" d
    // Also test SRTP with built-in types
    let intSum = sum [1; 2; 3; 4; 5]
    printfn "Int sum: %d" intSum
    0
