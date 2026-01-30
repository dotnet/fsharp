// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5861
// Previously this would give an error that quotations cannot contain inline assembly code

let f () =
    <@ let mutable arr = [| SimpleStruct.S() |]
       arr.[0].x <- 3 @>

let g () =
    <@ let mutable arr = [| SimpleStruct.S() |]
       arr.[0].x @>

let q1 = sprintf "%A" (f())
let q2 = sprintf "%A" (g())

exit 0
