// #Regression #Conformance #TypeInference #Attributes #ReqNOMT 
// Regression test for FSHARP1.0:2894
// Auto-open of my own namespace
// Case: no need to explicitly open XX.YY.ZZ
//      
#light 

module XX.YY.ZZ.MM
   // no need to explicitly open XX.YY.ZZ
   type t = C3
