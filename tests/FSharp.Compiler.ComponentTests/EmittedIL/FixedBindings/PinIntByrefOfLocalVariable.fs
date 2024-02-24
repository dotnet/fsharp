module FixedBindings
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

[<MethodImpl(MethodImplOptions.NoInlining)>]
let fail () =
    failwith "thingCopy was not the same as thing"

let pinIt (x: int) =
    let mutable thing = x + 1
    use ptr = fixed &thing
    let thingCopy = NativePtr.get ptr 0
    if thingCopy <> thing then fail ()
    
pinIt 100