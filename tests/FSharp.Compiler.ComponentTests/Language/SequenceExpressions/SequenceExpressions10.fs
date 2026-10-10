module MixedRangeVersionTest
        
let a = seq {
    0
    1..5
    10
    11..15
    20
    21..25
}

let b = [
    0
    1..5
    10
    11..15
    20
    21..25 
]
let c = [|
    0
    1..5
    10
    11..15
    20
    21..25
|]

let d = seq {
    0
    yield! [1..5]
    10
    yield! [11..15]
    20
    yield! [21..25]

}

let e = [
    0
    yield! [1..5]
    10
    yield! [11..15]
    20
    yield! [21..25]
]

let f = [|
    0
    yield! [1..5]
    10
    yield! [11..15]
    20
    yield! [21..25]
|]

if List.ofSeq a <> List.ofSeq d then failwithf $"a = d failed got {List.ofSeq a}"

if b <> e then failwithf $"b = e failed got {b}"

if c <> f then failwithf $"c = f failed got {c}"

printfn "All sequence tests passed!"