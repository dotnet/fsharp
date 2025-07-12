module MixedRangeSeqTests

let test1 = seq { 1..10; 19 }
let expected1 = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19]
if List.ofSeq test1 <> expected1 then failwith $"test1 failed"

let test2 = seq { 0; 2..2..10; 15 }
let expected2 = [|0; 2; 4; 6; 8; 10; 15|]
if Array.ofSeq test2 <> expected2 then failwith $"test2 failed"

let test3 = seq { 
    yield 1
    yield! [2..5]
    yield 10
    yield! [| 15..2..20 |]
}

let expected3 = [1; 2; 3; 4; 5; 10; 15; 17; 19]
if List.ofSeq test3 <> expected3 then failwith $"test3 failed"

printfn "All sequence tests passed!"