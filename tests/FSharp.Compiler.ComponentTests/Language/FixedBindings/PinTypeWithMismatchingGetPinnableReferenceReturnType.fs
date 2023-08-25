module FixedBindings
open Microsoft.FSharp.NativeInterop

type StrangeType() =
    let mutable _value = 42
    member this.GetPinnableReference() = &_value

let pinIt (thing: StrangeType) =
    use (ptr: nativeptr<char>) = fixed thing
    NativePtr.get ptr 0