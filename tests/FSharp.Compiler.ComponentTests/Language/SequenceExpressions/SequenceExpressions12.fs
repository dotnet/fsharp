module MixedRangeVersionTest
        
let a = [ yield! 1..10 ]

let b = [ yield! [1..10] ]

let c = seq { yield! seq { 1..10 } }

let d = [ 1..10 ]
let e = seq { 1..10 }
let f = [| 1..10 |]

if a <> d then failwithf $"a = d failed got {a}"
if b <> List.ofSeq e then failwithf $"b = d failed got {b}"
if Array.ofSeq c <> f then failwithf $"f = d failed got {f}"

printfn "All sequence tests passed!"