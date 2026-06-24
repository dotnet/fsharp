module DelegateNonInlinable

open System

// Section D, bullet 1: the optimizer inlines small function bodies before IlxGen runs. If a delegate
// target is inlined away in release/Optimize+ builds, the forwarding call no longer survives and the
// direct-delegate recognizer has nothing to fire on. To prove the release path is actually exercised we
// need targets the optimizer will NOT inline. [<NoCompilerInlining>] forces ValInline.Never, so the
// forwarding call is guaranteed to reach IlxGen regardless of how small the body is (see
// ComputeInlineFlag in CheckExpressions.fs). Bodies are arithmetic only (TFM-stable, no BCL calls) and
// Func<_,_,_> is used so the result is observable.

[<NoCompilerInlining>]
let accCurried (x: int) (y: int) : int = x + y

[<NoCompilerInlining>]
let accTupled (x: int, y: int) : int = x + y

// non-inlinable known target, non-eta-expanded
let niNonEta () = Func<int, int, int>(accCurried)

// non-inlinable known target, eta-expanded, curried application
let niEtaCurried () = Func<int, int, int>(fun a b -> accCurried a b)

// non-inlinable known target, eta-expanded, tupled application
let niEtaTupled () = Func<int, int, int>(fun a b -> accTupled (a, b))

type S =
    [<NoCompilerInlining>]
    static member AccS (x: int) (y: int) : int = x + y

// non-inlinable static target, non-eta-expanded
let niStaticNonEta () = Func<int, int, int>(S.AccS)

// non-inlinable static target, eta-expanded
let niStaticEta () = Func<int, int, int>(fun a b -> S.AccS a b)

type C(k: int) =
    [<NoCompilerInlining>]
    member _.AccC (x: int) (y: int) : int = x + y + k

    [<NoCompilerInlining>]
    member _.GPick<'T> (x: 'T) (y: 'T) : 'T = x

// non-inlinable instance target, non-eta-expanded
let niInstanceNonEta (o: C) = Func<int, int, int>(o.AccC)

// non-inlinable instance target, eta-expanded
let niInstanceEta (o: C) = Func<int, int, int>(fun a b -> o.AccC a b)

// non-inlinable generic instance target, non-eta-expanded
let niGenericInstanceNonEta (o: C) = Func<int, int, int>(o.GPick<int>)

// non-inlinable generic instance target, eta-expanded
let niGenericInstanceEta (o: C) = Func<int, int, int>(fun a b -> o.GPick<int> a b)

// Section D, bullet 1 (the contrast case): a trivial, inlinable target kept on purpose to document and
// guard the inline-vs-direct interaction. In release the optimizer may inline the body before the
// recognizer runs; the recognizer must therefore act in the optimizer (before the head call is inlined)
// for this to still become a direct delegate.
let trivial (x: int) (y: int) : int = x + y

let niTrivialNonEta () = Func<int, int, int>(trivial)

let niTrivialEta () = Func<int, int, int>(fun a b -> trivial a b)
