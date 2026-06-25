module DelegateStructTarget

open System

[<Struct>]
type S =
    [<NoCompilerInlining>]
    member _.Add (x: int) (y: int) : int = x + y

// The target is an instance method on a value type. A delegate's Target is an object, so a direct delegate
// would have to box the receiver; until that is implemented a closure is kept. (See DelegateInstanceMethod
// for the reference-type case, which is emitted directly.)
// 48. non-eta struct (value-type) receiver
let structInstanceNonEta (s: S) = Func<int, int, int>(s.Add)

// 49. eta struct (value-type) receiver
let structInstanceEta (s: S) = Func<int, int, int>(fun a b -> s.Add a b)
