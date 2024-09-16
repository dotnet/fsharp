// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSHARP1.0:1537
// The warning is emitted _and_ the code compiles just fine.
// As of 1/16/2009, the warning is now an error - so this code does not compile!


module M
 type R = { private i : int }   // err
 let r = { i = 10 }
