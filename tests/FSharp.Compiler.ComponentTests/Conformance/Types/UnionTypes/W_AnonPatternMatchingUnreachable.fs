// Anonymous Union Types
// Definitely unreachable pattern matching clause on an anonymous union type

let decide (x: (int|string)): int =
    match x with
    | :? System.Guid -> 0
    | _ -> 1

if not (decide 42 = 1) then failwith "Test failed"
