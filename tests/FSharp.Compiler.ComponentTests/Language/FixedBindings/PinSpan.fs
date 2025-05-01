module FixedBindings
open System
open Microsoft.FSharp.NativeInterop

let pinIt (thing: Span<char>) =
    use ptr = fixed thing
    NativePtr.get ptr 0