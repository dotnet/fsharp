module FixedBindings
open Microsoft.FSharp.NativeInterop

type RefField<'T>(_value) =
    let mutable _value = _value 
    member this.GetPinnableReference () : inref<'T> = &_value

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0