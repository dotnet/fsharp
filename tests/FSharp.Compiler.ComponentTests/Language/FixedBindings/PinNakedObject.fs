module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (thing: obj) =
    use ptr = fixed thing
    NativePtr.get ptr 0