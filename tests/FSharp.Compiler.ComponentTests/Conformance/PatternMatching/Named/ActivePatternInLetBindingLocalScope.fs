// #Conformance #PatternMatching #ActivePatterns
// Regression test for https://github.com/dotnet/fsharp/issues/16856

let mutable count = 0
let (|T|) (f: _ -> _) = count <- count + 1

let apply () =
    let (T) = id
    ()

apply ()

if count <> 1 then failwith "local let form: expected exactly one evaluation"
