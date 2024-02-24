module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System.Runtime.CompilerServices
open System

type RefField<'T>(_value) =
    let mutable _value = _value
    member this.Value = _value
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.GetPinnableReference () : byref<'T> = &_value

let pinIt (thing: RefField<int>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let x = RefField(42)
    let y = pinIt x
    if y <> x.Value then failwith "y did not equal x value"
    0