module FixedBindings
open Microsoft.FSharp.NativeInterop

type Point() =
    let mutable value = 42
    
    member this.PinIt() =
        let ptr = fixed &value
        NativePtr.get ptr 0