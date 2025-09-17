module NativePtrOverloads04
open Microsoft.FSharp.NativeInterop
#nowarn "9"
[<Measure>] type kg
[<Measure>] type m

type S =
    static member H(p: nativeptr<int<kg>>) = 1
    static member H(p: nativeptr<int<m>>) = 2  // expect duplicate member error