// #Conformance #PatternMatching #ActivePatterns
// Regression test for https://github.com/dotnet/fsharp/issues/16856

let mutable count = 0
let (|T|) (f: _ -> _) = count <- count + 1

match id with
| T -> ()

if count <> 1 then failwith "match form: expected exactly one evaluation"

let (T) = id

if count <> 2 then failwith "let form: expected exactly one evaluation"
