module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (thing: 'a) =
    use ptr = fixed thing
    NativePtr.get ptr 0