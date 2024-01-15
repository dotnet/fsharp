// #Conformance #DeclarationElements #LetBindings 
// Interesting pathological tests
// The code is odd, but parses and typechecks fine! :)
//<Expects status="success"></Expects>
module TestModule
let f _ _ _ _ _ _ _ _ _ _ = ()

f "B@"B@"B@"B@"B@"B@"B@"B@"B@"
