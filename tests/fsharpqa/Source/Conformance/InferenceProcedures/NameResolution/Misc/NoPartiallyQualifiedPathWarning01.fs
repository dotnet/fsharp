// #Regression #Conformance #TypeInference #ReqNOMT 
// Regression test for FSHARP1.0:3274
// This file is going to be loaded by fsi.exe
// in the actual testcase.
#light

namespace S.M

type T = struct
            val private value:int
         end
