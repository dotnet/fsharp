module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (thing: byref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
    
let mutable x = 42
let xCopy = pinIt &x
if x <> xCopy then failwith "xCopy was not the same as x" 