module FixedBindings
open Microsoft.FSharp.NativeInterop

[<Struct>]
type Fruit = Apple | Banana

let pinIt (x: Fruit) =
    use ptr = fixed x
    NativePtr.get ptr 0