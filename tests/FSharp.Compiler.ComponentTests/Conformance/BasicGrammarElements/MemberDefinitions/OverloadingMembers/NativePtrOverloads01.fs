module NativePtrOverloads01
open Microsoft.FSharp.NativeInterop
#nowarn "9"

type P =
    static member Do(p: nativeptr<int>) = 1
    static member Do(p: nativeptr<int64>) = 2

let _invoke (pi: nativeptr<int>) (pl: nativeptr<int64>) = P.Do pi + P.Do pl