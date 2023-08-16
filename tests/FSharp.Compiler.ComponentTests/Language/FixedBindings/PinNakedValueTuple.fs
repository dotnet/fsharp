module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt (x: System.ValueTuple<int,int>) =
    use ptr = fixed x
    NativePtr.get ptr 0