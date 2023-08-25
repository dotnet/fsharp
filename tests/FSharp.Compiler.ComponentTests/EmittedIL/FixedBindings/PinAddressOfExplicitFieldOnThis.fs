module FixedBindings
open Microsoft.FSharp.NativeInterop

type Point =
    val mutable X: int
    val mutable Y: int
    
    new(x: int, y: int) = { X = x; Y = y }
    
    member this.PinIt() =
        use ptr = fixed &this.X
        NativePtr.get ptr 0
        
let p = Point(10,20)
let xCopy = p.PinIt()
if xCopy <> p.X then failwith "xCopy was not equal to X"