module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (x: System.Tuple<int,int>) =
    use ptr = fixed x
    NativePtr.get ptr 0