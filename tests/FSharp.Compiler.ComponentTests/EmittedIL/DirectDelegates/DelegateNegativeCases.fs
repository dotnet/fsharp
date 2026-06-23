module DelegateNegativeCases

open System

// first-class function value: there is no target method to point at, so a closure must remain
let firstClass (handler: int -> int -> unit) = Action<int, int>(handler)

// lambda body is not a single direct forwarding call to a known target (the argument is computed,
// not the delegate parameters forwarded as-is): a closure must remain
let private sink (x: int) : unit = ()
let notDirect (k: int) = Action<int, int>(fun a b -> sink (a + b + k))

// arguments reordered: not a transparent forwarding, so a closure must remain
let reordered (handler: int -> int -> unit) = Action<int, int>(fun a b -> handler b a)
