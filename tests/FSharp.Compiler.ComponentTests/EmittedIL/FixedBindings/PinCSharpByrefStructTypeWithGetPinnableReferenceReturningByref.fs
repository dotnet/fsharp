module FixedBindings
open Microsoft.FSharp.NativeInterop
open CsLib

let pinIt (refField: RefField<int>) =
    use ptr = fixed refField
    NativePtr.get ptr 0

[<EntryPoint>]
let main _ =
    let mutable x = 42
    let refToX = new RefField<_>(&x)
    let y = pinIt refToX
    if y <> x then failwith "y did not equal x"
    0