module FixedBindings
open Microsoft.FSharp.NativeInterop
open System

let pinIt (thing: Span<char>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    
[<EntryPoint>]
let main _ =
    let span = Span("The quick brown fox jumped over the lazy dog".ToCharArray())
    let x = pinIt span
    if x <> 'T' then failwith "x did not equal the first char of the span"
    0