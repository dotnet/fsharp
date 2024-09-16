// #Regression #Conformance #DeclarationElements #LetBindings 
// Interesting pathological tests
// The code is odd. It used to parse, but not it is an error. See FSHARP1.0:4980, test since updated for triple quoted string support


let f _ _ _ _ _ _ _ _ _ _ = ()

f """"""""""""""""""""
f """""""""""""""""""