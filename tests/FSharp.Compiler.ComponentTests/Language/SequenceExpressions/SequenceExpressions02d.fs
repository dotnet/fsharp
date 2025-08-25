module MixedRangeListTest04

let test = [0; 2..2..10; 15]
let expected = [0; 2; 4; 6; 8; 10; 15]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"