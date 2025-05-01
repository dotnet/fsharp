module FixedBindings
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    static member GetPinnableReference() : byref<'T> = Unchecked.defaultof<byref<'T>>

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0