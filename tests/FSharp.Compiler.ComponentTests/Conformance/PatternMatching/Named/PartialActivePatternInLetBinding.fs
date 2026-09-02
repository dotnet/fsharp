// #Conformance #PatternMatching #ActivePatterns
// Regression test for https://github.com/dotnet/fsharp/issues/16856

let (|P|_|) (f: _ -> _) = Some()
let (P) = id
