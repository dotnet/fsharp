module MixedRangeListTests

let test1 = [-3; 1..10]
let expected1 = [-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10]
if test1 <> expected1 then failwith $"test1 failed: got {test1}"

let test2 = [-3; 1..10; 19]
let expected2 = [-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19]
if test2 <> expected2 then failwith $"test2 failed: got {test2}"

let test3 = [1..3; 5..7; 10]
let expected3 = [1; 2; 3; 5; 6; 7; 10]
if test3 <> expected3 then failwith $"test3 failed: got {test3}"

let test4 = [0; 2..2..10; 15]
let expected4 = [0; 2; 4; 6; 8; 10; 15]
if test4 <> expected4 then failwith $"test4 failed: got {test4}"

printfn "All list tests passed!"