// #Regression #Conformance #DeclarationElements #LetBindings 
// Interesting pathological tests
// The code is odd. It used to parse, but not it is an error. See FSHARP1.0:4980, since updated for triple quoted string support
//<Expects id="FS0515" span="(8,33-8,35)" status="error">End of file in verbatim string begun at or before here</Expects>

let f _ _ _ _ _ _ _ _ _ _ = ()

f """B@"B@"""B@"B@"B@"B@"B@"B@"B@"
