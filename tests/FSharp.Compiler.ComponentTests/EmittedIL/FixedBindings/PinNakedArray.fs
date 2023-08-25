module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed arr
    NativePtr.get ptr 0