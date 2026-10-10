module MixedRangeListTest02

let test = [-3; 1..10; 19]
let expected = [-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"