module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed &arr[1]
    NativePtr.get ptr 0