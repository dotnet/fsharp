// #Regression #Conformance #TypeInference #Attributes #ReqNOMT 
// Regression test for FSHARP1.0:2894
// Auto-open of my own namespace
// Case: redundant open
//      
#light 

module XX.YY.ZZ.MM
   open XX.YY.ZZ      // redundant, but ok!
   type t = C3        // t is a type abbreviation for XX.YY.ZZ.MM
   
   
