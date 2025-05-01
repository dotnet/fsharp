// #Regression #Conformance #DeclarationElements #LetBindings 
// Interesting pathological tests
// The code is odd. It used to parse, but not it is an error. See FSHARP1.0:4980, since updated for triple quote strings


let f _ _ _ _ _ _ _ _ _ _ = ()

f """B@"B@"""""""""""""B@"B@"B@"B@"B@"B@"B@"
