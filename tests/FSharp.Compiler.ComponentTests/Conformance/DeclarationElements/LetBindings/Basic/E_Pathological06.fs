// #Regression #Conformance #DeclarationElements #LetBindings 
// Interesting pathological tests
// The code is odd. It used to parse, but not it is an error. See FSHARP1.0:4980, since updated for triple quote strings
//<Expects id="FS1232" span="(8,20-8,23)" status="error">End of file in triple-quote string begun at or before here</Expects>

let f _ _ _ _ _ _ _ _ _ _ = ()

f """B@"B@"""""""""""""B@"B@"B@"B@"B@"B@"B@"
