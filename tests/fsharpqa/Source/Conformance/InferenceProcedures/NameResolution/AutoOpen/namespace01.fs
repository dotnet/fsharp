// #Regression #Conformance #TypeInference #Attributes #ReqNOMT 
// Regression test for FSHARP1.0:2894
// Auto-open of my own namespace
// Case: namespace
//      
#light 

namespace XX.YY.ZZ
   type t = G3<int>        // t is a type abbreviation for XX.YY.ZZ.G3<'a>
