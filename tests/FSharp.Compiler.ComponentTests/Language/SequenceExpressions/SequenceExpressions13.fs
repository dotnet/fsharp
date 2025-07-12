module MixedRangeVersionTest
        
let a = [ yield! 1..10; 19; yield! [20..30] ]

let b = [ yield! [1..10]; 19; yield! 20..30 ]

let c = seq { yield! seq { 1..10 }; 19; yield! seq { 20..30 } }

let d = [ 1..10; 19; yield! 20..30 ]
let e = seq { yield! 1..10; 19; yield! 20..30 }
let f = [| yield! 1..10; 19; yield! 20..30 |]

if a <> d then failwithf $"a = d failed got {a}"
if b <> List.ofSeq e then failwithf $"b = d failed got {b}"
if Array.ofSeq c <> f then failwithf $"f = d failed got {f}"

printfn "All sequence tests passed!"