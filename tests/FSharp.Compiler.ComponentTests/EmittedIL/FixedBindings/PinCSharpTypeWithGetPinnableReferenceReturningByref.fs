module FixedExpressions
open Microsoft.FSharp.NativeInterop
open System

let pinIt (thing: PinnableReference<int>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let x = PinnableReference(42)
    let y = pinIt x
    if y <> x.Value then failwith "y did not equal x value"
    0