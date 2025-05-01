module FixedBindings
open Microsoft.FSharp.NativeInterop

type Point = { mutable X: int; mutable Y: int }

let pinIt (thing: Point) =
    use ptr = fixed &thing.X
    NativePtr.get ptr 0
    
let p = { X = 10; Y = 20 }
let xCopy = pinIt p
if xCopy <> p.X then failwith "xCopy was not equal to X"