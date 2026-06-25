module DelegateExtensionMethod

open System
open System.Runtime.CompilerServices

type Holder() =
    class
    end

[<Extension>]
type HolderExtensions =
    [<Extension; NoCompilerInlining>]
    static member Combine (h: Holder, x: int, y: int) : int = x + y

// An extension member compiles to a static method whose first parameter is the receiver, so using it as a
// delegate target binds that receiver as a leading argument - a partial application, which has no closed
// direct-delegate form. A closure must remain regardless of langversion.
// 50. extension member (receiver is a leading static arg)
let extensionEta (h: Holder) = Func<int, int, int>(fun a b -> h.Combine(a, b))
