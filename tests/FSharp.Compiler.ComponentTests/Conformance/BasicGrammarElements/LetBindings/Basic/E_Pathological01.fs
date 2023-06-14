// #Regression #Conformance #DeclarationElements #LetBindings 
// Interesting pathological tests
// The code is odd. It used to parse, but not it is an error. See FSHARP1.0:4980, test since updated for triple quoted string support
//<Expects id="FS1232" span="(8,3-8,6)" status="error">End of file in triple-quote string begun at or before here</Expects>

let f _ _ _ _ _ _ _ _ _ _ = ()

f """"
