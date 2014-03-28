// #Regression #Conformance #TypeInference #Attributes #ReqNOMT 
// Regression test for FSHARP1.0:2894
// Auto-open of my own namespace
// Case: parent namespace is not visible
//<Expects id="FS0039" span="(10,22-10,24)" status="error">The type 'C2' is not defined</Expects>
#light 

module XX.YY.ZZ.MM
   type t = | AAA of C3
            | BBB of C2        // error XX.YY.C2 is not "visible"

