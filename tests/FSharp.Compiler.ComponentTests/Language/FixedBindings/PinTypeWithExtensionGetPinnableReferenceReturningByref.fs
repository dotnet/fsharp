module FixedBindings
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

type RefField<'T> = { mutable _value: 'T }

[<Extension>]
type RefFieldExtensions =
    [<Extension>]
    static member GetPinnableReference(refField: RefField<'T>) : byref<'T> = &refField._value 

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0