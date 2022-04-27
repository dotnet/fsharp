// #Regression #Conformance #PatternMatching 
// Regression test for FSharp1.0:4920
// Title: Can't have a quotation with an incomplete pattern match
// Regression test for FSharp1.0:4904
// Title: Incomplete pattern in quotation causes error.
// Descr: Make sure quotations compile (even with warning) when having incomplete pattern match inside

//<Expects id="FS0025" span="(14,15-14,16)" status="warning">Incomplete pattern matches on this expression\.</Expects>
//<Expects id="FS0025" span="(21,31-21,39)" status="warning">Incomplete pattern matches on this expression\.</Expects>
//<Expects id="FS0026" span="(31,11-31,12)" status="warning">This rule will never be matched</Expects>
//<Expects id="FS0026" span="(32,11-32,13)" status="warning">This rule will never be matched</Expects>
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
