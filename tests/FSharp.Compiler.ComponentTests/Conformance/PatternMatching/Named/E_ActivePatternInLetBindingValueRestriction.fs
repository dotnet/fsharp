// #Conformance #PatternMatching #ActivePatterns
// Regression test for https://github.com/dotnet/fsharp/issues/16856
// A value bound through an active pattern is not generalized, exactly like
//     let g = match id with Id g -> g
// so leaving it unused reports the value restriction rather than an internal error.
//<Expects id="FS0030" status="error">Value restriction: The value 'g' has an inferred generic function type</Expects>

let (|Id|) f = f
let (Id g) = id
