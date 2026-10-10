module MixedRangeListTest01

let test = [-3; 1..10]
let expected = [-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"