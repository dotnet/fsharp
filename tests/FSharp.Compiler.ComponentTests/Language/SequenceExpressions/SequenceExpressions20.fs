module EmptyRangesTest

let test = [ 0; 1..0; 2..2; 3..1; 4 ]
let expected = [0;2;4]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"
