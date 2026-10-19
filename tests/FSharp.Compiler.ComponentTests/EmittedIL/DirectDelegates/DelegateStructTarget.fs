module DelegateStructTarget

open System

[<Struct>]
type S =
    member _.Add (x: int) (y: int) : int = x + y

// The target is an instance method on a value type. A delegate's Target is an 'object', so the receiver is
// boxed (a copy) at the construction site and the runtime binds the unboxing stub; this matches the closure
// form, which also captures the struct by value. (See DelegateInstanceMethod for the reference-type case.)
// 50. non-eta struct (value-type) receiver
let structInstanceNonEta (s: S) = Func<int, int, int>(s.Add)

// 51. eta struct (value-type) receiver
let structInstanceEta (s: S) = Func<int, int, int>(fun a b -> s.Add a b)
