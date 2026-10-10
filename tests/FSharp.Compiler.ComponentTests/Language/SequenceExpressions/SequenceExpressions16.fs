module MixedRangeVersionTest
        
let a: int list = [ yield! 1..10 ]
let a1: int list = [ yield! [1..10] ]

let b: int seq = seq { yield! seq { 1..10 } }

let c: int list = [ if true then yield 1 else yield! 1..10]
let c1: int list = [ if true then yield! 1..10]

let d: int list =  [if true then yield 1 else yield! [1..10]]
let d1 : int list =  [if true then yield! [1..10]]

let e: int seq = seq { if true then yield 1 else yield! seq { 1..10 } }
let e1: int seq = seq { if true then yield! seq { 1..10 } }

let f: int list = [ match true with | true -> yield 1 | false -> yield! 1..10]

let f1: int list = [ match true with | true -> yield 1 | false -> yield! [1..10]]

let g: int seq = seq { match true with | true -> yield 1 | false -> yield! seq { 1..10 } }

let expected = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10]
let expectedOne = [1]

if a <> expected then failwithf $"a failed: got {a}"
if a1 <> expected then failwithf $"a1 failed: got {a1}"
if List.ofSeq b <> expected then failwithf $"b failed: got {List.ofSeq b}"
if c <> expectedOne then failwithf $"c failed: got {c}"
if c1 <> expected then failwithf $"c1 failed: got {c1}"
if d <> expectedOne then failwithf $"d failed: got {d}"
if d1 <> expected then failwithf $"d1 failed: got {d1}"
if List.ofSeq e <> expectedOne then failwithf $"e failed: got {List.ofSeq e}"
if List.ofSeq e1 <> expected then failwithf $"e1 failed: got {List.ofSeq e1}"
if f <> expectedOne then failwithf $"f failed: got {f}"
if f1 <> expectedOne then failwithf $"f1 failed: got {f1}"
if List.ofSeq g <> expectedOne then failwithf $"g failed: got {List.ofSeq g}"

printfn "yield vs yield! tests passed!"