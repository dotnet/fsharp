module TypeInferenceTests

let test1: int list = [1..5; 10]
let test2: int array = [|1..5; 10|]
let test3: seq<int> = seq { 1..5; 10 }

let inline mixedRange start finish extra =
    [start..finish; extra]

let test4 = mixedRange 1 5 10
let test5 = mixedRange 1.0 5.0 10.0
let test6 = mixedRange 'a' 'e' 'z'

if test4 <> [1; 2; 3; 4; 5; 10] then failwith "int range failed"
if test5 <> [1.0; 2.0; 3.0; 4.0; 5.0; 10.0] then failwith "float range failed"
if test6 <> ['a'; 'b'; 'c'; 'd'; 'e'; 'z'] then failwith "char range failed"

printfn "Type inference tests passed!"