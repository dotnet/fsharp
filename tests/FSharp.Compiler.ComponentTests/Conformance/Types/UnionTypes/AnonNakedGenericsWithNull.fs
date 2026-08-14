// Anonymous Union Types
// Anonymous union type consisting of naked generics

let decide<'T, 'U>(x: ('T | 'U | null)): int =
    match x with
    | :? 'T -> 0
    | :? 'U -> 1
    | null -> 2

if not (decide<int, string> 42 = 0) then failwith "Test failed"
if not (decide<int, string> "asd" = 1) then failwith "Test failed"
if not (decide<int, string> null = 2) then failwith "Test failed"
