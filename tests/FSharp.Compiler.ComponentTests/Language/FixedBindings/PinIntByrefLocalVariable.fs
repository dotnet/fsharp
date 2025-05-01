module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt () =
    let mutable thing = 42
    use ptr = fixed &thing
    NativePtr.get ptr 0