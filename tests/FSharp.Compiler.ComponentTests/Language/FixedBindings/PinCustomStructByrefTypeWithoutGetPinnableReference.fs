module FixedBindings
open System
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

[<Struct; IsByRefLike>]
type BoringRefField<'T> = { Value: 'T }

let pinIt (thing: BoringRefField<char>) =
    use ptr = fixed thing
    NativePtr.get ptr 0