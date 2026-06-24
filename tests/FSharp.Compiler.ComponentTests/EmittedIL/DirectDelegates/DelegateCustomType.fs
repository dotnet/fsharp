module DelegateCustomType

open System

// Custom, F#-declared delegate types exercise construction with delegates defined in the *compiled* assembly
// (local scope, unlike imported BCL Func/Action) and with Invoke signatures the Func/Action tests do not
// cover: a multi-argument (tupled) signature, a generic delegate, and a byref parameter. (F# forbids curried
// delegate signatures — FS0950 — so every F# delegate has a single tupled Invoke parameter group.) Targets
// are [<NoCompilerInlining>] so the optimized path genuinely emits the direct form rather than inlining the
// target away (no inline-race).

type DTupled = delegate of int * int -> int
type DGen<'T> = delegate of 'T -> 'T
type DByref = delegate of byref<int> -> unit

[<NoCompilerInlining>]
let acc (x: int) (y: int) : int = x + y

[<NoCompilerInlining>]
let ident (x: 'T) : 'T = x

type C() =
    [<NoCompilerInlining>]
    member _.M (x: int) (y: int) : int = x * y

// Tupled-signature custom delegate: Invoke(int, int).
let tupledNonEta () = DTupled(acc)
let tupledEta () = DTupled(fun a b -> acc a b)

// Instance member through a custom delegate: the receiver becomes the delegate's Target.
let instanceNonEta (c: C) = DTupled(c.M)
let instanceEta (c: C) = DTupled(fun a b -> c.M a b)

// Generic custom delegate instantiated at int: Invoke(int):int over the generic target.
let genNonEta () = DGen<int>(ident)
let genEta () = DGen<int>(fun x -> ident x)

// byref-parameter custom delegate: the body mutates through the byref, so it is not a transparent forwarding
// call and stays a closure. Documents that a byref Invoke parameter does not break the recognizer.
let byrefMutate () = DByref(fun x -> x <- x + 1)
