module YieldVsYieldBang
        
let a1 = [ yield! 1..5 ]
let a2 = seq { yield! 1..5 }
let a3 = [| yield! 1..5 |]

// let b1: int list = [ yield 1..5 ]  // Would give type error

let c1: int list = [ yield! 1..5 ]
let c2: int seq = seq { yield! 1..5 }
let c3: int array = [| yield! 1..5 |]

let expected = [1; 2; 3; 4; 5]

if a1 <> expected then failwithf $"a1 failed: got {a1}"
if List.ofSeq a2 <> expected then failwithf $"a2 failed: got {List.ofSeq a2}"
if List.ofArray a3 <> expected then failwithf $"a3 failed: got {List.ofArray a3}"
if c1 <> expected then failwithf $"c1 failed: got {c1}"
if List.ofSeq c2 <> expected then failwithf $"c2 failed: got {List.ofSeq c2}"
if List.ofArray c3 <> expected then failwithf $"c3 failed: got {List.ofArray c3}"

printfn "yield vs yield! tests passed!"