module DelegateNonInlinable

open System

[<NoCompilerInlining>]
let accCurried (x: int) (y: int) : int = x + y

[<NoCompilerInlining>]
let accTupled (x: int, y: int) : int = x + y

// 23. non-eta module function, non-inlinable
let niNonEta () = Func<int, int, int>(accCurried)

// 6. eta module function, non-inlinable (curried application)
let niEtaCurried () = Func<int, int, int>(fun a b -> accCurried a b)

// 38. eta module function, non-inlinable, tupled application
let niEtaTupled () = Func<int, int, int>(fun a b -> accTupled (a, b))

type S =
    [<NoCompilerInlining>]
    static member AccS (x: int) (y: int) : int = x + y

// 24. non-eta static method, non-inlinable
let niStaticNonEta () = Func<int, int, int>(S.AccS)

// 7. eta static method, non-inlinable
let niStaticEta () = Func<int, int, int>(fun a b -> S.AccS a b)

type C(k: int) =
    [<NoCompilerInlining>]
    member _.AccC (x: int) (y: int) : int = x + y + k

    [<NoCompilerInlining>]
    member _.GPick<'T> (x: 'T) (y: 'T) : 'T = x

// 25. non-eta instance method, non-inlinable
let niInstanceNonEta (o: C) = Func<int, int, int>(o.AccC)

// 8. eta instance method, non-inlinable
let niInstanceEta (o: C) = Func<int, int, int>(fun a b -> o.AccC a b)

// 26. non-eta generic instance method, non-inlinable
let niGenericInstanceNonEta (o: C) = Func<int, int, int>(o.GPick<int>)

// 9. eta generic instance method, non-inlinable
let niGenericInstanceEta (o: C) = Func<int, int, int>(fun a b -> o.GPick<int> a b)

// A trivial, inlinable target kept on purpose to document and
// guard the inline-vs-direct interaction. In release the optimizer may inline the body before the
// recognizer runs; the recognizer must therefore act in the optimizer (before the head call is inlined)
// for this to still become a direct delegate.
let trivial (x: int) (y: int) : int = x + y

// 27. non-eta inlinable trivial target (inline-race contrast)
let niTrivialNonEta () = Func<int, int, int>(trivial)

// 10. eta inlinable trivial target (inline-race)
let niTrivialEta () = Func<int, int, int>(fun a b -> trivial a b)
