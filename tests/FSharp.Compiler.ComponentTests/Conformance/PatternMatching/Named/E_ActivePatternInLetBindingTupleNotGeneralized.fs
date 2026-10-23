// #Conformance #PatternMatching #ActivePatterns
// Regression test for https://github.com/dotnet/fsharp/issues/16856
// An active pattern anywhere in the pattern de-generalizes the whole binding,
// exactly like the equivalent 'match': neither 'g' nor its tuple partner 'h' is generalized.

let (|Id|) f = f

let (Id g), h = id, id

let g2, h2 = match id, id with Id g, h -> g, h
