module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (thing: byref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0