module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (thing: int) =
    use ptr = fixed thing
    NativePtr.get ptr 0