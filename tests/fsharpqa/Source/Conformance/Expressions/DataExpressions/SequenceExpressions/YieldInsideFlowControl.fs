// #Regression #Conformance #DataExpressions #Sequences 
#light

// Regression test for FSharp1.0:3930 - "Invalid sequence expression" error when using yield combined with if-then expressions in seq.

let evens n =
    seq {
        if true then
            yield 0
            yield! [2..2..n] |> Seq.ofList
    }
    
let odds n = 
    seq {
        if false then
            yield! []
        else
            yield 1
            yield! [3..2..n]
    }
    
if ((evens 100) |> Seq.append (odds 100) |> Seq.sum ) <> ( 100 * 101 / 2 ) then exit 1

exit 0
