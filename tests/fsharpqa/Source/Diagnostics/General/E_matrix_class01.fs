// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3175
//<Expects id="FS0039" span="(7,35-7,41)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>


type C = class
            member this.M(x: Math.matrix) = x
         end
