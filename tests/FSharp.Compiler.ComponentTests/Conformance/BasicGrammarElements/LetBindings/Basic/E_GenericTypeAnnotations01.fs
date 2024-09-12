// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Regression test for FSharp1.0:1943 - Better error message when unknown unicode characters are used - in particular the � character


let foo(x : `a) = ([] : `a list)
