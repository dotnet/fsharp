// Regression test for FSharp1.0:3101 - trying to sign an assembly with an incorrect key yield a compiler exception
//<Expects id="FS2014" status="error">A problem occurred writing the binary 'E_BadKey01\.dll': A call to StrongNameGetPublicKey failed \(Value cannot be null\.</Expects>

#light 
open System
let _ = 100
exit 1
