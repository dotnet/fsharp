module FixedBindings
open Microsoft.FSharp.NativeInterop

type Fruit = Apple | Banana

let pinIt (x: Fruit) =
    use ptr = fixed x
    NativePtr.get ptr 0