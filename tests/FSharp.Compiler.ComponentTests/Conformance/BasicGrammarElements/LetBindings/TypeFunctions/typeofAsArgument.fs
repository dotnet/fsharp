// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
#light

// Regression test for FSharp1.0:3654 - design change: allow foo<int> to be used as an argument without parentheses

let getString o = o.ToString()

if (getString typeof<int> <> "System.Int32") then failwith "Failed: 1"
