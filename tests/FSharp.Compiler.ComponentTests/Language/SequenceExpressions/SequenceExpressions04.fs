module MixedRangeSeqTests

let test1 = seq { 1..10; 19 }
let expected1 = seq { 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19 }
if List.ofSeq test1 <> List.ofSeq expected1 then failwith $"test1 failed got {test1}"

let test2 = seq { -3; 1..5; 10..12 }
let expected2 = seq { -3; 1; 2; 3; 4; 5; 10; 11; 12 }
if List.ofSeq test2 <>  List.ofSeq expected2 then failwith $"test2 failed got {test2}"

let test3 = seq { 0; 2..2..10; 15 }
let expected3 = seq { 0; 2; 4; 6; 8; 10; 15 }
if List.ofSeq test3 <> List.ofSeq expected3 then failwith $"test3 failed got {test3}"

let test4 = seq { 
    yield 1
    yield! seq { 2..5 }
    yield 10
    yield! seq { 15..2..20 }
}

let expected4 = seq { 1; 2; 3; 4; 5; 10; 15; 17; 19 }
if List.ofSeq test4 <> List.ofSeq expected4 then failwith $"test4 failed got {test4}"

printfn "All sequence tests passed!"