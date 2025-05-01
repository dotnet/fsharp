// Regression test for https://github.com/dotnet/fsharp/issues/15254
#nowarn "52"

open System

let default_nativeint = Unchecked.defaultof<nativeint>
let intptr_nativeint = IntPtr.Zero
let isSame = intptr_nativeint = default_nativeint

if not (isSame) then raise (new Exception "default_nativeint and intptr_nativeint compare is incorrect")
