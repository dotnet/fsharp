module NativePtrOverloads03
open Microsoft.FSharp.NativeInterop
#nowarn "9"
// Regression test for issue #7428 

type R =
    static member F(p: nativeptr<uint16>) = 0us
    static member F(p: nativeptr<int64>) = 0L