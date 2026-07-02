// Anonymous Union Types
// Anonymous union type consisting of non-naked generics

let decide<'T, 'U>(x: (list<'T> | list<'U>)): int =
    match x with
    | :? list<'T> -> 0
    | :? list<'U> -> 1

if not (decide<int, string>([1]) = 0) then failwith "Test failed"
if not (decide<int, string>(["asd"]) = 1) then failwith "Test failed"