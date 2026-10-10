module CustomCEWithRanges

type ArrayBuilder() =
    member _.Yield(x) = [| x |]
    member _.YieldFrom(xs: seq<_>) = Seq.toArray xs
    member _.Zero() = [||]
    member _.Combine(a: int array, b: unit -> int array) = Array.append a (b())
    member _.Delay(f) = f
    member _.Run(f) = f()

let array = ArrayBuilder()

let test1 = array { -3; 1..10; 19 }
let expected1 = [| -3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19 |]
if test1 <> expected1 then failwith $"test1 failed: got {test1}"

let test2 = array { 1..3; 5..7; 10 }
let expected2 = [| 1; 2; 3; 5; 6; 7; 10 |]
if test2 <> expected2 then failwith $"test2 failed: got {test2}"

let test3 = array { 0; 2..2..10; 15 }
let expected3 = [| 0; 2; 4; 6; 8; 10; 15 |]
if test3 <> expected3 then failwith $"test3 failed: got {test3}"

printfn "Custom CE with ranges tests passed!"