// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Regression test for FSharp1.0:1943 - Better error message when unknown unicode characters are used - in particular the � character
//<Expects id="FS0010" span="(7,13-7,14)" status="error">Unexpected reserved keyword in pattern</Expects>

let foo(x : `a) = ([] : `a list)
