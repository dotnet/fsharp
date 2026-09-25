// Regression test reduced from @gusty's miniFSharpPlus repro for RFC FS-1043.
//
// `sequence (FF (EE 2))` dispatches through a chain of extension members on marker
// classes (Sequence.Default2 -> Traverse -> Map -> Apply), i.e. return-type-directed
// SRTP resolved via the extension-members-solve-SRTP feature. Before the codegen fix
// this ICE'd with FS0073 "internal error: Undefined or unsolved type variable" because
// a phantom unsolved SRTP typar reached GenGenericArgs on the non-witness closure path.
// It must now compile and evaluate to EE (FF 2).
module NestedTraverseSequenceSRTP

module Mini =
    [<AutoOpen>]
    module Pointed =
        type Return = class end
        [<AutoOpen>]
        module Invoker =
            type Return with
                static member inline Invoke x : '``Pointed<'T>`` = (^``Pointed<'T>`` : (static member Return : _ -> _) x)

    [<AutoOpen>]
    module Monad =
        type Bind = class end
        [<AutoOpen>]
        module Invoker =
            type Bind with static member inline Invoke (x: '``Monad<'T>``) (f: 'T -> '``Monad<'U>``) : '``Monad<'U>`` = (^``Monad<'T>`` : (static member (>>=) : _*_ -> _) x, f)

    [<AutoOpen>]
    module Applicative =
        type Apply = class end
        module Default3 = type Apply with static member inline (<*>) (f: '``Monad<'T -> 'U>``, x: '``Monad<'T>``) = Bind.Invoke f (fun f' -> Bind.Invoke x (fun x' -> Return.Invoke (f' x')))
        [<AutoOpen>]
        module Invoker =
            open Default3
            type Apply with
                static member inline Invoke (f: '``Applicative<'T -> 'U>``) (x: '``Applicative<'T>``) : '``Applicative<'U>`` =
                    ((^``Applicative<'T -> 'U>`` or ^``Applicative<'T>`` or Apply) : (static member (<*>) : _*_ -> _) f, x)

    [<AutoOpen>]
    module Functor =
        type Map = class end
        module Default3 = type Map with static member inline (|>>) (x, f) = Bind.Invoke x (fun a -> Return.Invoke (f a))
        module Default2 = type Map with static member inline (|>>) (x: '``Applicative<'T>``, f) = ((^``Applicative<'T>`` or Apply) : (static member (<*>) : _ * _ -> _) (Return.Invoke f, x))
        [<AutoOpen>]
        module Invoker =
            open Default3
            open Default2
            type Map with
                static member inline Invoke (f: 'T -> 'U) (x: '``Functor<'T>``) : '``Functor<'U>`` =
                    ((^``Functor<'T>`` or Map) : (static member (|>>) : _*_ -> _) x, f)

    [<AutoOpen>]
    module Traversable =
        type Traverse = class end
        module Default2 = type Traverse with static member inline Traverse (t: '``Traversable<'T>``, f: 'T -> '``Applicative<'U>>``) = (^``Traversable<'T>`` : (static member Sequence : _ -> _) (Map.Invoke f t))
        [<AutoOpen>]
        module Invoker =
            open Default2
            type Traverse with
                static member inline Invoke (x: '``Traversable<'T>``) (f: 'T -> '``Applicative<'U>``) : '``Applicative<Traversable<'U>>`` =
                    ((^``Traversable<'T>`` or Traverse) : (static member Traverse : _*_ -> _) x, f)

    [<AutoOpen>]
    module Sequence =
        type Sequence = class end
        module Default2 = type Sequence with static member inline Sequence (t: '``Traversable<'Applicative<'T>>``) = (^``Traversable<'Applicative<'T>>`` : (static member Traverse : _ * _ -> _) (t, id))
        [<AutoOpen>]
        module Invoker =
            open Default2
            type Sequence with
                static member inline Invoke (x: '``Traversable<'Applicative<'T>>``) : '``Applicative<Traversable<'T>>`` =
                    ((^``Traversable<'Applicative<'T>>`` or Sequence) : (static member Sequence : _ -> _) x)

module SimpleFSharpPlus =
    open Mini
    let inline sequence x = Sequence.Invoke x
    let inline (|>>) x f = Map.Invoke f x

open Mini
open SimpleFSharpPlus

type EE<'t> = EE of 't with
    static member Return x = EE x
    static member (<*>) (EE f, EE x) : EE<_> = EE (f x)

type FF<'t> = FF of 't with
    static member inline Traverse (FF x, f) = (f x) |>> FF

let y17 = sequence (FF (EE 2))
match y17 with
| EE (FF v) when v = 2 -> ()
| other -> failwithf "Expected EE (FF 2), got %A" other

// A live (deferred then forced) closure on the same phantom-typar path must also evaluate
// correctly, guarding against future reasoning that treats the defaulted-typar branch as dead code.
let inline sequenceDelayed x : unit -> _ = fun () -> sequence x
match (sequenceDelayed (FF (EE 2))) () with
| EE (FF v) when v = 2 -> ()
| other -> failwithf "Expected EE (FF 2) from delayed sequence, got %A" other
