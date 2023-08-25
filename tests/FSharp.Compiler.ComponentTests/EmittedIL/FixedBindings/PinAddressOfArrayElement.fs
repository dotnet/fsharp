module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed &arr[0]
    NativePtr.get ptr 0
    
let x = [|'a';'b';'c'|]
let y = pinIt x
if y <> 'a' then failwithf "y did not equal first element of x"