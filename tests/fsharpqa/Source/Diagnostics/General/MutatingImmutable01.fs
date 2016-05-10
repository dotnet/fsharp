// #Regression #Diagnostics 
// Regression test for FSHARP1.0:6135
//<Expects status="error" span="(11,5-11,43)" id="FS0027">This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable m_cts = expression'.</Expects>


open System.Threading

let private m_cts = null

let test() =
    m_cts <- new CancellationTokenSource()
    ()

