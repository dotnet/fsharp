// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Regression test for FSharp1.0:4165
// Sweep all deprecation warnings and change to errors where reasonable [WAS: Inline values should be an error, not a warning]



let inline inc = (+) 1;;

let inline fs73 = (+) 1 >> (=) 0;;

