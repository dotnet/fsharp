// Regression test for https://github.com/dotnet/fsharp/issues/15254
#nowarn "52"

open System

let default_voidptr = Unchecked.defaultof<voidptr>
let intptr_voidptr = IntPtr.Zero.ToPointer()
let isSame = intptr_voidptr = default_voidptr

if not (isSame) then raise (new Exception "default_voidptr and intptr_voidptr compare is incorrect")
