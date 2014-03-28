// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:5673
//<Expects status="error" span="(7,19-7,20)" id="FS0001">The type 'CSharpTypes\.T' does not support a conversion to the type 'float32'$</Expects>

let t = new CSharpTypes.T()
let p = ( char t, double t, int t, byte t)
let q = ( float32 t)
