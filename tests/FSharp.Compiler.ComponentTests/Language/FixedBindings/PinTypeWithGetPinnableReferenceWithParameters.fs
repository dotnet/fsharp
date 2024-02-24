module FixedBindings
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    member this.GetPinnableReference(someValue: string) : byref<'T> = &_value

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0