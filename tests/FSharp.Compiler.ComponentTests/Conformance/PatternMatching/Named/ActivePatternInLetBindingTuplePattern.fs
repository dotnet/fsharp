// #Conformance #PatternMatching #ActivePatterns
// Regression test for https://github.com/dotnet/fsharp/issues/16856

let (|T|) (f: _ -> _) = ()
let (T), x = id, 1

if x <> 1 then failwith "expected the tuple partner value to bind"
