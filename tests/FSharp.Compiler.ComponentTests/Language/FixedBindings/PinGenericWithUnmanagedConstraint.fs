module FixedBindings
open Microsoft.FSharp.NativeInterop

let pinIt<'a when 'a : unmanaged> (thing: 'a) =
    use ptr = fixed thing
    NativePtr.get ptr 0