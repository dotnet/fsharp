// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3175
//<Expects id="FS0039" span="(6,29-6,35)" status="error">The type 'matrix' is not defined$</Expects>

type S = struct
               val a : Math.matrix
         end
