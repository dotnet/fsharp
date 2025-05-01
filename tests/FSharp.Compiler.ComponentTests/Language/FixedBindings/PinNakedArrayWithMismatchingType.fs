module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (arr: int[]) =
    use (ptr: nativeptr<byte>) = fixed arr
    NativePtr.get ptr 0