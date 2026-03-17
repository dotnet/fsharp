// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3175
//<Expects id="FS0039" span="(8,34-8,40)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>
//<Expects id="FS0039" span="(8,49-8,55)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>


type I = interface
               abstract i : Math.matrix -> Math.matrix
         end

