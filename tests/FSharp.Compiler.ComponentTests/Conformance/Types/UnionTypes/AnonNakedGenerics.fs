// Anonymous Union Types
// Anonymous union type consisting of naked generics

let decide<'T, 'U>(x: ('T | 'U)): int =
    match x with
    | :? 'T -> 0
    | :? 'U -> 1

if not (decide<int, string>(42) = 0) then failwith "Test failed"
if not (decide<int, string>("asd") = 1) then failwith "Test failed"
