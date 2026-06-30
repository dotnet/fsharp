module DelegateNegativeCases

open System
open System.Runtime.CompilerServices

// 42. first-class function value: there is no target method to point at, so a closure must remain
let firstClass (handler: int -> int -> unit) = Action<int, int>(handler)

// 43. lambda body is not a single direct forwarding call to a known target (the argument is computed,
// not the delegate parameters forwarded as-is): a closure must remain
let private sink (x: int) : unit = ()
let notDirect (k: int) = Action<int, int>(fun a b -> sink (a + b + k))

// 44. arguments reordered: not a transparent forwarding, so a closure must remain
let reordered (handler: int -> int -> unit) = Action<int, int>(fun a b -> handler b a)

type Holder() =
    [<NoCompilerInlining>]
    member _.TakesObj (x: obj) : int = 1

// 45. Reference-parameter contravariance: the delegate's Invoke is (string):int and the target is (object):int.
// The CLR would accept this binding directly (a delegate may bind a method whose parameter is a supertype),
// but it stays a closure: F# elaborates the 'string -> obj' argument upcast as a coercion, so the forwarded
// argument is no longer a verbatim Invoke parameter and the direct-delegate recognizer does not match. (The
// signature check is not involved - it never even runs here.)
let contra (h: Holder) = System.Func<string, int>(fun s -> h.TakesObj s)

[<Extension>]
type Extensions =
    [<Extension; NoCompilerInlining>]
    static member Echo<'T> (x: 'T, y: int, z: int) : 'T = x

// 54. extension member on a VALUE-TYPE receiver: an extension member compiles to a static method whose first
// parameter is the receiver, which the closed-delegate mechanism would store as the 'object' Target and pass
// straight into that first by-value parameter with no unboxing. A value-type receiver therefore has no closed
// form (unlike a value-type *instance* receiver, which is reached through the method's unboxing stub), so a
// closure must remain.
let valueTypeExtension () = Func<int, int, int>(fun a b -> (3).Echo(a, b))
