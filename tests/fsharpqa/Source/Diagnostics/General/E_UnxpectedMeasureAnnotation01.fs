// #Regression #Diagnostics #RequiresPowerPack 
// Regression test for FSHARP1.0:2312 (ICE when providing unnamed parameter to static member)
//<Expects id="FS0712" span="(12,27)" status="error">Type parameter cannot be used as type constructor</Expects>
// The error is obscure, but ok for now.
#light
open Microsoft.FSharp.Math

type Matrix<'t> with
    /// Matrix-scalar multiplication.   
    /// ******* NOTICE THE INVALID PARAMETER SYNTAX *****
    [<OverloadID("MultiplyMatrixScalar")>]
    static member ( * ) : Matrix<'a> 'a = (Microsoft.FSharp.Math.Matrix.Generic.zero 1 1) : Matrix<'a>

type intMtx = Matrix<int>
let sqr (x:Matrix<'a>) : Matrix<'a> = x * x
let cube (x:Matrix<'a>) : Matrix<'a> = x * (sqr x)
