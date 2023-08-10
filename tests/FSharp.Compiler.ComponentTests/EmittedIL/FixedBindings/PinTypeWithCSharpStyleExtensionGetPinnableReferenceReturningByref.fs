module FixedBindings
open Microsoft.FSharp.NativeInterop
open System.Runtime.CompilerServices

type RefField<'T> = { mutable _value: 'T }

[<Extension>]
type RefFieldExtensions =
    [<Extension; MethodImpl(MethodImplOptions.NoInlining)>]
    static member GetPinnableReference(refField: RefField<'T>) : byref<'T> = &refField._value 

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let mutable x = 42
    let refToX = { _value = x }
    let y = pinIt refToX
    if y <> x then failwith "y did not equal x"
    0