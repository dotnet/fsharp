module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (thing: outref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0