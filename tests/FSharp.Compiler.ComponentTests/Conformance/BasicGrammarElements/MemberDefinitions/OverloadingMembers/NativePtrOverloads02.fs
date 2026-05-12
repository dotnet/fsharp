module NativePtrOverloads02
open Microsoft.FSharp.NativeInterop
#nowarn "9"

type Q =
    static member M(p: nativeptr<int>) = 0
    static member M(p: nativeptr<int>) = 1  // expect duplicate member error