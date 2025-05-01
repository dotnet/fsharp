module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (thing: string) =
    use (ptr: nativeptr<byte>) = fixed thing
    NativePtr.get ptr 0