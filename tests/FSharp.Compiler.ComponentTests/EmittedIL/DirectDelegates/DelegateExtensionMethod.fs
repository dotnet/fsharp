module DelegateExtensionMethod

open System
open System.Runtime.CompilerServices

type Holder() =
    class
    end

[<Extension>]
type HolderExtensions =
    [<Extension>]
    static member Combine (h: Holder, x: int, y: int) : int = x + y

// An extension member compiles to a static method whose first parameter is the receiver. Using it as a
// delegate target binds that receiver as a leading argument, which the CLR's "closed over the first argument"
// delegate stores as the Target while the function pointer points at the static method - a direct delegate.
// (The member here is tupled, 'Combine(h, x, y)'; the recognizer de-tuples the forwarding call by the target's
// arity, exactly as the code generator does when emitting the call.) As an eta-expanded delegate it is direct
// only in optimized builds, where the user's lambda need not survive for debugging.
// 52. extension member (receiver is a leading static arg, bound as Target)
let extensionEta (h: Holder) = Func<int, int, int>(fun a b -> h.Combine(a, b))
