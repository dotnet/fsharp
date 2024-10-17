// #Regression #Conformance #PatternMatching 
// Regression test for FSharp1.0:4920
// Title: Can't have a quotation with an incomplete pattern match
// Regression test for FSharp1.0:4904
// Title: Incomplete pattern in quotation causes error.
// Descr: Make sure quotations compile (even with warning) when having incomplete pattern match inside





let foo x =
    <@@
        match x with 
        | 1 -> 1
        | 2 -> 2 
    @@>

let g = 
    <@ 
        let f ( int : int ) = function
            | 1 -> 0
            | 2 -> 1
        f
    @>
    
let h =
    <@@
        match 10 with
        | x -> x
        | 1 -> 20
        | 10 -> 10
    @@>
    
printfn "Finished"
