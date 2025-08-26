module BigIntRangesTest
open System.Numerics

let test = [ 0I..2I..6I; 10I ]
let expected = [0I;2I;4I;6I;10I]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"
