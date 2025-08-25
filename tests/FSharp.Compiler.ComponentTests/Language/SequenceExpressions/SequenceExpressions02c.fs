module MixedRangeListTest03

let test = [1..3; 5..7; 10]
let expected = [1; 2; 3; 5; 6; 7; 10]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"