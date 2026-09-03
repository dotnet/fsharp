// #Conformance #PatternMatching #ActivePatterns
// Regression test for https://github.com/dotnet/fsharp/issues/16856

let (|Id|) f = f
let (Id g) = id

if g 1 <> 1 then failwith "expected g to be usable at a concrete type"
