// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5859

let q = <@ let mutable x = 1
           x <- 2 @>
printfn "%s" (q.ToString())

// this used to throw an exception from ToString
let mutable y = 1
let q2 = <@ y <- 2 @>
printfn "%s" (q.ToString())

exit 0
