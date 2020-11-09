// #Regression #Conformance #DeclarationElements #LetBindings 
// Interesting pathological tests
// The code is odd. It used to parse, but not it is an error. See FSHARP1.0:4980, test since updated for triple quoted string support
//<Expects id="FS0514" span="(9,21-9,22)" status="error">End of file in string begun at or before here</Expects>

let f _ _ _ _ _ _ _ _ _ _ = ()

f """"""""""""""""""""
f """""""""""""""""""