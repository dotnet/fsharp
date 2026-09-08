// RFC FS-1043 "Extension members solve SRTP constraints" — end-to-end mini-library test.
//
// The library below is @gusty's `miniFSharpPlus` reproduction (Gustavo Leon, @gusty),
// kept close to verbatim so the feature is exercised the way it was designed against
// FSharpPlus-style code. Three faithful, documented adaptations were needed to build it
// as a net11 ComponentTest:
//   1. Three function-type extension headers `type ('a -> 'b) with ...` were rewritten to the
//      equivalent `type Microsoft.FSharp.Core.FSharpFunc<'a,'b> with ...` form that @gusty
//      already uses in the same file. This PR adds tuple-type extension sugar, not
//      function-type sugar, so the arrow spelling does not parse.
//   2. Eight `seq`/`IEnumerable`/`HashSet` type extensions are commented out. On .NET 9+ the
//      `IEnumerable<T>` type parameter is annotated `[<AllowByRefLike>]`, which F# type-extension
//      syntax cannot restate (FS0957/FS0341). Orthogonal BCL evolution, not a feature gap.
//   3. The deep Traversable tier (`traverse`/`sequence` over the mutually-recursive
//      Traverse/Sequence marker classes) is intentionally not invoked here: in the full-library
//      context it is not yet witnessed end-to-end and reaches the inline-SRTP dynamic fallback at
//      run time. Its curated working slice is covered by NestedTraverseSequenceSRTP.fs.
//
// What this proves (compile AND run, --langversion:preview): extension operators
// (++ >>= <*> |>>) and extension members solve SRTP constraints, and the
// Default1/Default2/Default3 return-type-directed mechanism (AllowOverloadOnReturnType) selects
// the right witness — end to end, with the expected runtime values.
module MiniFSharpPlusExtensionSRTP

#nowarn "42" // `retype` below uses (# "" x #) inline IL, by design in the mini-library

open System.Text

// #r @"c:\Repos\FSharpPlus\src\FSharpPlus\bin\Debug\netstandard2.1\FSharpPlus.dll"
open System
open FSharp.Core.CompilerServices

let inline retype (x: 'T) : 'U =  (# "" x: 'U #)

let addQuotes s = sprintf "'%s'" s

(*
module StructTupleExtensions =
    type Int32 with member this.Add x = this + x
    (5).Add 10

    (^t : (member Add : _ -> _) (5, 10)) // val it: int = -1571392838   ????
    *)

module MiniFSharpPlus =

    [<AutoOpen>]
    module Extensions =
        /// Additional operations on HashSet<'T>
        [<RequireQualifiedAccess>]
        module HashSet =
            open System.Collections.Generic

            /// <summary>Computes the union of the two sets.</summary>
            /// <param name="source1">The first input set.</param>
            /// <param name="source2">The second input set.</param>
            /// <returns>The union of <c>set1</c> and <c>set2</c>.</returns>
            [<CompiledName("Union")>]
            let union (source1: HashSet<'T>) (source2: HashSet<'T>) : HashSet<'T> =
                let source1 = nullArgCheck (nameof source1) source1
                let source2 = nullArgCheck (nameof source2) source2
                let union = HashSet<'T> (max source1.Count source2.Count)
                for item in source1 do union.Add item |> ignore
                for item in source2 do union.Add item |> ignore
                union

        /// Additional operations on Exception
        [<RequireQualifiedAccess>]
        module Exception =
            open System
            open System.Runtime.ExceptionServices

            /// Combines exceptions from 2 exceptions into a single AggregateException.
            /// Exceptions already present in the first argument won't be added.
            let add (exn1: exn) (exn2: exn) =
                let exn1 = nullArgCheck (nameof exn1) exn1
                let exn2 = nullArgCheck (nameof exn2) exn2
                let f (e: exn) =
                    match e with
                    :? AggregateException as a -> a.InnerExceptions :> seq<_>
                    | _ -> Seq.singleton e
                let left = f exn1
                new AggregateException (seq { yield! left; yield! Seq.except left (f exn2) })


    // Monoid

    [<AutoOpen>]
    module Monoid =
        type Zero = class end

        module Default4 =
            type Zero with
                [<AllowOverloadOnReturnType>]
                static member inline get_Zer0 () = printfn "Default4"; (^t : (static member Empty: ^t) ()) : 't

        module Default3 =
            type Zero with
                [<AllowOverloadOnReturnType>]
                static member inline get_Zer0 () = printfn "Default3"; LanguagePrimitives.GenericZero : 't


        [<AutoOpen>]
        module Invoker =
            open Default4
            open Default3
            // open Default2

            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.IEnumerable<'t> with static member get_Zer0 () = Seq.empty
            type List<'t>                            with static member get_Zer0 () = []
            type 'T``[]``                            with static member get_Zer0 () = [||]
            type Option<'T>                          with static member get_Zer0 () = None
            type ValueOption<'T>                     with static member get_Zer0 () = ValueNone
            type Unit                                with static member Zer0 = ()
            type Boolean                             with static member Zer0 = false
            type Set<'t when 't : comparison> with static member get_Zer0 () = Set.empty
            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.HashSet<'t> with static member get_Zer0 () = Collections.Generic.HashSet ()
            type String                               with static member Zer0 = ""
            type StringBuilder with static member Zer0 = StringBuilder ()
            type AggregateException with static member Zer0 = new AggregateException ()
            type Exception          with static member Zer0 = new Exception ()

            type Zero with static member inline get_Zer0 () = printfn "Default1"; (^t : (static member Zer0: ^t) ()) : 't
            and Zero  with
                static member inline Invoke () : 't =
                    let inline call_2 (a: ^a, b: ^b) = ((^a or ^b) : (static member Zer0 : ^b) ())
                    let inline call (a: 'a) = call_2 (a, Unchecked.defaultof<'r>) : 'r
                    call Unchecked.defaultof<Zero>

            and Tuple<'t>   with static member inline get_Zer0 () = Tuple<'T> (Zero.Invoke ()) : Tuple<'T>
            and ('t1 * 't2) with static member inline get_Zer0 () = (Zero.Invoke (), Zero.Invoke ())
            and ('t1 * 't2 * 't3) with static member inline get_Zer0 () = (Zero.Invoke (), Zero.Invoke (), Zero.Invoke ())
            and ('t1 * 't2 * 't3 * 't4) with static member inline get_Zer0 () = (Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke ())
            and ('t1 * 't2 * 't3 * 't4 * 't5) with static member inline get_Zer0 () = (Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke ())
            and ('t1 * 't2 * 't3 * 't4 * 't5 * 't6) with static member inline get_Zer0 () = (Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke ())
            and ('t1 * 't2 * 't3 * 't4 * 't5 * 't6 * 't7) with static member inline get_Zer0 () = (Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke ())
            and Tuple<'T1,'T2,'T3,'T4,'T5,'T6,'T7,'TR> with
                static member inline get_Zer0 () : 't =
                    let x = Unchecked.defaultof<'t>
                    let xr: 'tr = if true then Unchecked.defaultof<_> else (^t : (member Rest : 'tr) x)
                    let x7: 't7 = if true then Unchecked.defaultof<_> else (^t : (member Item7: 't7) x)
                    let x6: 't6 = if true then Unchecked.defaultof<_> else (^t : (member Item6: 't6) x)
                    let x5: 't5 = if true then Unchecked.defaultof<_> else (^t : (member Item5: 't5) x)
                    let x4: 't4 = if true then Unchecked.defaultof<_> else (^t : (member Item4: 't4) x)
                    let x3: 't3 = if true then Unchecked.defaultof<_> else (^t : (member Item3: 't3) x)
                    let x2: 't2 = if true then Unchecked.defaultof<_> else (^t : (member Item2: 't2) x)
                    let x1: 't1 = if true then Unchecked.defaultof<_> else (^t : (member Item1: 't1) x)
                    Tuple<'t1,'t2,'t3,'t4,'t5,'t6,'t7,'tr> (Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke (), Zero.Invoke ()) |> retype : 't

            and ValueTuple<'t> with static member inline get_Zer0 () = ValueTuple<'T> (Zero.Invoke ()) : ValueTuple<'T>
            and struct ('t1 * 't2) with static member inline get_Zer0 () = struct (Zero.Invoke (), Zero.Invoke ())

// open MiniFSharpPlus
// let z8: (string * string * string * string * string * string * string * string) = Zero.Invoke ()

    // Semigroups

    [<AutoOpen>]
    module Semigroup =
        type Plus = class end

        module Default3 = type Plus with static member inline ``+`` (x: 'Plus, y: 'Plus) = printfn "Default3"; (^Plus :  (static member (<|>) : _*_ -> _) x, y) : ^Plus
        module Default2 = type Plus with static member inline ``+`` (x: 'Plus, y: 'Plus) = printfn "Default2"; x + y : ^Plus

        [<AutoOpen>]
        module Invoker =
            open Default3
            open Default2

            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.IEnumerable<'t> with static member (++) (x: seq<'T>, y: seq<'T>) = Seq.append x y
            type List<'T> with static member (++) (x1: list<'T>, x2: list<'T>) = List.append x1 x2
            type 'T``[]`` with static member (++) (x1: 'T[], x2: 'T[]) = Array.append x1 x2
            type Unit     with static member (++) ((), ()) = ()
            type Boolean  with static member (++) (x: bool, y: bool) = x <> y
            type Set<'t when 't : comparison> with static member (++) (x: Set<_>, y) = Set.union x y
            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.HashSet<'t> with static member (++) (x: Collections.Generic.HashSet<_> , y) = HashSet.union x y
            type StringBuilder with static member (++) (x: StringBuilder     , y: StringBuilder     ) = StringBuilder().Append(x).Append(y)
            type AggregateException with static member (++) (x: AggregateException, y: AggregateException) = Exception.add x y
            type Exception          with static member (++) (x: exn               , y: exn               ) = Exception.add x y :> exn

            type Plus with static member inline ``+`` (x: 'Plus, y: 'Plus) = printfn "Default1"; x ++ y : ^Plus
            and Plus with
                static member inline Invoke (x: 'Plus) (y: 'Plus) : 'Plus =
                    let inline call (mthd : ^M, input1 : ^I, input2 : ^I) = ((^M or ^I) : (static member ``+`` : _*_ -> _) input1, input2)
                    call (Unchecked.defaultof<Plus>, x, y)

            and Option<'T> with
                static member inline (++) (x: option<'t>, y: option<'t>) =
                                match x, y with
                                | (Some a , Some b) -> Some (Plus.Invoke a b)
                                | (Some a , None  ) -> Some a
                                | (None   , Some b) -> Some b
                                | _                 -> None

            and ValueOption<'T> with
                static member inline (++) (x: voption<'t>, y: voption<'t>) =
                                match x, y with
                                | (ValueSome a , ValueSome b) -> ValueSome (Plus.Invoke a b)
                                | (ValueSome a , ValueNone  ) -> ValueSome a
                                | (ValueNone   , ValueSome b) -> ValueSome b
                                | _                 -> ValueNone

            and Result<'t,'e> with
                static member inline (++) (x: Result<'T,'E>, y: Result<'T,'E>) =
                                match x, y with
                                | Ok a   , Ok b    -> Ok (Plus.Invoke a b)
                                | Ok a   , Error _ -> Ok a
                                | Error _, Ok b    -> Ok b
                                | Error a, Error b -> Error (Plus.Invoke a b)

            and Choice<'t,'e> with
                static member inline (++) (x: Choice<'T,'E>, y: Choice<'T,'E>) =
                                match x, y with
                                | Choice1Of2 a   , Choice1Of2 b    -> Choice1Of2 (Plus.Invoke a b)
                                | Choice1Of2 a   , Choice2Of2 _ -> Choice1Of2 a
                                | Choice2Of2 _ , Choice1Of2 b    -> Choice1Of2 b
                                | Choice2Of2 a, Choice2Of2 b -> Choice2Of2 (Plus.Invoke a b)

            and Tuple<'t> with static member inline (++) (x: Tuple<'a>, y: Tuple<'a>) = Tuple<'a> (Plus.Invoke x.Item1 y.Item1) : Tuple<'a>
            and ('t1 * 't2)                               with static member inline (++) ((x1,x2               ), (y1,y2               )) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2)
            and ('t1 * 't2 * 't3)                         with static member inline (++) ((x1,x2,x3            ), (y1,y2,y3            )) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3)
            and ('t1 * 't2 * 't3 * 't4)                   with static member inline (++) ((x1,x2,x3,x4         ), (y1,y2,y3,y4         )) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4)
            and ('t1 * 't2 * 't3 * 't4 * 't5)             with static member inline (++) ((x1,x2,x3,x4,x5      ), (y1,y2,y3,y4,y5      )) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5)
            and ('t1 * 't2 * 't3 * 't4 * 't5 * 't6)       with static member inline (++) ((x1,x2,x3,x4,x5,x6   ), (y1,y2,y3,y4,y5,y6   )) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5, Plus.Invoke x6 y6)
            and ('t1 * 't2 * 't3 * 't4 * 't5 * 't6 * 't7) with static member inline (++) ((x1,x2,x3,x4,x5,x6,x7), (y1,y2,y3,y4,y5,y6,y7)) = (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5, Plus.Invoke x6 y6, Plus.Invoke x7 y7)

            and Tuple<'T1,'T2,'T3,'T4,'T5,'T6,'T7,'TR> with
                static member inline (++) (x, y) : 't =
                    let xr, yr = (^t : (member Rest : 'tr) x), (^t : (member Rest : 'tr) y)
                    let x7, y7 = (^t : (member Item7: 't7) x), (^t : (member Item7: 't7) y)
                    let x6, y6 = (^t : (member Item6: 't6) x), (^t : (member Item6: 't6) y)
                    let x5, y5 = (^t : (member Item5: 't5) x), (^t : (member Item5: 't5) y)
                    let x4, y4 = (^t : (member Item4: 't4) x), (^t : (member Item4: 't4) y)
                    let x3, y3 = (^t : (member Item3: 't3) x), (^t : (member Item3: 't3) y)
                    let x2, y2 = (^t : (member Item2: 't2) x), (^t : (member Item2: 't2) y)
                    let x1, y1 = (^t : (member Item1: 't1) x), (^t : (member Item1: 't1) y)
                    Tuple<_,_,_,_,_,_,_,_> (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5, Plus.Invoke x6 y6, Plus.Invoke x7 y7, Plus.Invoke xr yr) |> retype : 't

            and ValueTuple<'t> with static member inline (++) (x: ValueTuple<'a>, y: ValueTuple<'a>) = ValueTuple<'a> (Plus.Invoke x.Item1 y.Item1) : ValueTuple<'a>
            and struct ('t1 * 't2) with static member inline (++) (struct (x1,x2), struct (y1,y2)) = struct (Plus.Invoke x1 y1, Plus.Invoke x2 y2)
            and struct ('t1 * 't2 * 't3) with static member inline (++) (struct (x1,x2,x3), struct (y1,y2,y3)) = struct (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3)
            and struct ('t1 * 't2 * 't3 * 't4) with static member inline (++) (struct (x1,x2,x3,x4), struct (y1,y2,y3,y4)) = struct (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4)
            and struct ('t1 * 't2 * 't3 * 't4 * 't5) with static member inline (++) (struct (x1,x2,x3,x4,x5), struct (y1,y2,y3,y4,y5)) = struct (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5)
            and struct ('t1 * 't2 * 't3 * 't4 * 't5 * 't6) with static member inline (++) (struct (x1,x2,x3,x4,x5,x6), struct (y1,y2,y3,y4,y5,y6)) = struct (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5, Plus.Invoke x6 y6)
            and struct ('t1 * 't2 * 't3 * 't4 * 't5 * 't6 * 't7) with static member inline (++) (struct (x1,x2,x3,x4,x5,x6,x7), struct (y1,y2,y3,y4,y5,y6,y7)) = struct (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5, Plus.Invoke x6 y6, Plus.Invoke x7 y7)
            // and ValueTuple<'T1,'T2,'T3,'T4,'T5,'T6,'T7,'TR> with
            //     static member inline (++) (x, y) : 't =
            //         let xr, yr = (^t : (member Rest : 'tr) x), (^t : (member Rest : 'tr) y)
            //         let x7, y7 = (^t : (member Item7: 't7) x), (^t : (member Item7: 't7) y)
            //         let x6, y6 = (^t : (member Item6: 't6) x), (^t : (member Item6: 't6) y)
            //         let x5, y5 = (^t : (member Item5: 't5) x), (^t : (member Item5: 't5) y)
            //         let x4, y4 = (^t : (member Item4: 't4) x), (^t : (member Item4: 't4) y)
            //         let x3, y3 = (^t : (member Item3: 't3) x), (^t : (member Item3: 't3) y)
            //         let x2, y2 = (^t : (member Item2: 't2) x), (^t : (member Item2: 't2) y)
            //         let x1, y1 = (^t : (member Item1: 't1) x), (^t : (member Item1: 't1) y)
            //         ValueTuple<_,_,_,_,_,_,_,_> (Plus.Invoke x1 y1, Plus.Invoke x2 y2, Plus.Invoke x3 y3, Plus.Invoke x4 y4, Plus.Invoke x5 y5, Plus.Invoke x6 y6, Plus.Invoke x7 y7, Plus.Invoke xr yr) |> retype : 't

    // Pointed

    [<AutoOpen>]
    module Pointed =
        type Return = class end

        [<AutoOpen>]
        module Invoker =

            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.IEnumerable<'t> with static member Return (x: 'T) : seq<'T> = Seq.singleton x
            type List<'t> with static member Return (x: 'T) : list<'T> = [x]
            type 't``[]`` with static member Return (x: 'T) : 'T[] = [|x|]


            type Option<'t> with
                [<AllowOverloadOnReturnType>]
                static member Return (x: 'T) : option<'T> = Some x
            type ValueOption<'t> with static member Return (x: 'T) : ValueOption<'T> = ValueSome x
            type Result<'t, 'e> with static member Return (x: 'T) : Result<'T, 'E> = Ok x
            type Choice<'t, 'e> with static member Return (x: 'T) : Choice<'T, 'E> = Choice1Of2 x
            type ('t1 * 't2) with static member inline Return (x: 'T) : ('T1 * 'T) = (Zero.Invoke (), x)
            type struct ('t1 * 't2) with static member inline Return (x: 'T) : struct ('T1 * 'T) = struct (Zero.Invoke (), x)
            type Microsoft.FSharp.Core.FSharpFunc<'r, 'a> with static member Return (x: 'A) = fun (_: 'R) -> x
            type Set<'t when 't : comparison> with static member Return (x: 'T) : Set<'T> = Set.singleton x

            type Return with
                [<AllowOverloadOnReturnType>]
                static member inline Invoke x : '``Pointed<'T>`` = (^``Pointed<'T>`` : (static member Return : _ -> _) x)

    [<AutoOpen>]
    module Monad =
        type Bind = class end

        [<AutoOpen>]
        module Invoker =
            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.IEnumerable<'t> with static member (>>=) (x: seq<'T>, f: 'T -> seq<'U>) : seq<'U> = Seq.collect f x
            type List<'t> with static member (>>=) (x: list<'T>, f: 'T -> list<'U>) : list<'U> = List.collect f x
            type 't``[]`` with static member (>>=) (x: 'T[], f: 'T -> 'U[]) : 'U[] = x |> Array.collect f
            type Option<'t> with static member (>>=) (x: option<'T>, f: 'T -> option<'U>) : option<'U> = Option.bind f x
            type ('t1 * 't2) with static member inline (>>=) ((w: 'Monoid, a: 'T), k: 'T -> 'Monoid * 'U) = let m, b = k a in (Plus.Invoke w m, b) : 'Monoid*'U
            type struct ('t1 * 't2) with static member inline (>>=) (struct (w: 'Monoid, a: 'T), k: 'T -> struct ('Monoid * 'U)) = let struct (m, b) = k a in struct (Plus.Invoke w m, b) : struct ('Monoid * 'U)
            type Microsoft.FSharp.Core.FSharpFunc<'r, 'a> with static member (>>=) (m: 'R -> 'T, k: 'T -> 'R -> 'U) : 'R -> 'U = fun r -> let a = m r in k a r

            type Bind with static member inline Invoke (x: '``Monad<'T>``) (f: 'T -> '``Monad<'U>``) : '``Monad<'U>`` = (^``Monad<'T>`` : (static member (>>=) : _*_ -> _) x, f)


    [<AutoOpen>]
    module Applicative =
        type Apply = class end

        module Default3 = type Apply with static member inline (<*>) (f: '``Monad<'T -> 'U>`` , x: '``Monad<'T>``) = printfn "Default3"; Bind.Invoke f (fun f' -> Bind.Invoke x (fun x' -> Return.Invoke (f' x')))

        [<AutoOpen>]
        module Invoker =
            // open Default3

            type List<'t> with
                static member (<*>) (f: list<'T -> 'U>, x: list<'T>) : list<'U> =
                    let mutable coll = ListCollector<'U> ()
                    f |> List.iter (fun f ->
                        x |> List.iter (fun x ->
                            coll.Add (f x)))
                    coll.Close ()

            type 'T``[]`` with
                static member (<*>) (f: ('T -> 'U)[], x: 'T[]) : 'U[] =
                    let coll = ResizeArray<'U> ()
                    f |> Array.iter (fun f ->
                        x |> Array.iter (fun x ->
                            coll.Add (f x)))
                    coll.ToArray ()

            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.IEnumerable<'t> with static member (<*>) (f: seq<'T -> 'U>, x: seq<'T>) : seq<'U> = seq { for f in f do for x in x do yield f x }
            type Option<'t> with static member (<*>) (f: option<'T -> 'U>, x: option<'T>) : option<'U> = match f, x with (Some f, Some x) -> Some (f x) | _ -> None
            type ValueOption<'t> with static member (<*>) (f: voption<'T -> 'U>, x: voption<'T>) : voption<'U> = match f, x with (ValueSome f, ValueSome x) -> ValueSome (f x) | _ -> ValueNone

            type Microsoft.FSharp.Core.FSharpFunc<'t, 'u> with static member (<*>) (f: FSharpFunc<'T, 'T -> 'U>, g) : 'T -> 'U = fun x -> let f' = f x in f' (g x)

            // type Tuple<'T1, 'T2> with static member inline (<*>) (af: Tuple<_, _>, bx: Tuple<_, _>) = Tuple<_, _>(af.Item1 ++ bx.Item1, af.Item2 bx.Item2)

            type ('T1 * 'T2) with static member inline (<*>) ((a, f), (b, x)) = (Plus.Invoke a b, f x)
            type struct ('T1 * 'T2) with static member inline (<*>) (struct (a, f), struct (b, x)) = struct (Plus.Invoke a b, f x)


            // type ValueTuple<'T1, 'T2> with static member inline (<*>) (struct (a, f), struct (b, x)) = struct (a ++ b, f x)
            // [<struct]type ('T1 * 'T2) with static member inline (<*>) ((a, f), (b, x)) = (a ++ b, f x)

            type Apply with
                static member inline Invoke (f: '``Applicative<'T -> 'U>``) (x: '``Applicative<'T>``) : '``Applicative<'U>`` =
                    ((^``Applicative<'T -> 'U>`` or ^``Applicative<'T>`` or Apply) : (static member (<*>) : _*_ -> _) f, x)

    [<AutoOpen>]
    module Functor =
        type Map = class end

        module Default3 = type Map with static member inline (|>>) (x, f) = printfn "Default3"; Bind.Invoke x (fun a -> Return.Invoke (f a))
        module Default2 = type Map with static member inline (|>>) (x: '``Applicative<'T>``, f) = printfn "Default2"; ((^``Applicative<'T>`` or Apply) : (static member (<*>) : _ * _ -> _) (Return.Invoke f, x))

        [<AutoOpen>]
        module Invoker =
            open Default3
            open Default2

            // net9+ omitted (IEnumerable<T> genparam is AllowByRefLike; orthogonal BCL change): type Collections.Generic.IEnumerable<'t> with static member (|>>) (x, f) = Seq.map f x
            type List<'t>   with static member (|>>) (x, f) = List.map f x
            type 't``[]``   with static member (|>>) (x, f) = Array.map f x
            type Option<'t> with static member (|>>) (x, f) = Option.map f x
            type ValueOption<'t> with static member (|>>) (x, f) = ValueOption.map f x
            type Microsoft.FSharp.Core.FSharpFunc<'t, 'u> with static member (|>>) (f, g) = f >> g

            type Map with
                static member inline Invoke (f: 'T -> 'U) (x: '``Functor<'T>``) : '``Functor<'U>`` =
                    ((^``Functor<'T>`` or Map) : (static member (|>>) : _*_ -> _) x, f)


    [<AutoOpen>]
    module Traversable =
        type Traverse = class end

        module Default2 = type Traverse with static member inline Traverse (t: '``Traversable<'T>``, f: 'T -> '``Applicative<'U>>``) = printfn "Default2"; (^``Traversable<'T>`` : (static member Sequence : _ -> _) (Map.Invoke f t))

        [<AutoOpen>]
        module Invoker =
            open Default2

            type List<'t> with
                static member inline Traverse (t: list<'T>, f: 'T -> '``Applicative<'U>`` ) : '``Applicative<list<'U>>`` =
                    let cons head tail = head :: tail
                    let rec loop acc = function
                        | [] -> acc
                        | x::xs ->
                            let v = f x
                            loop (v::acc) xs
                    let cons_f x xs = Apply.Invoke (Map.Invoke cons xs) x
                    let r = (^``Applicative<'U>`` : (static member Return : _ -> _) [])
                    List.fold cons_f r (loop [] t)

            // type 't``[]`` with static member inline Traverse (x: 't``[]``, f) = let cons head tail = Map.Invoke (fun h t -> Array.append [|h|] t) head tail in Array.foldBack cons x (Return.Invoke [||])
            type Option<'t> with static member inline Traverse (t: option<'``Functor<'T>``>, f) = match t with Some x -> Map.Invoke Some (f x) | _ -> Return.Invoke None
            type ValueOption<'t> with static member inline Traverse (t: ValueOption<'``Functor<'T>``>, f) = match t with ValueSome x -> Map.Invoke ValueSome (f x) | _ -> Return.Invoke ValueNone

            type Traverse with
                static member inline Invoke (x: '``Traversable<'T>``) (f: 'T -> '``Applicative<'U>``) : '``Applicative<Traversable<'U>>`` =
                    ((^``Traversable<'T>`` or Traverse) : (static member Traverse : _*_ -> _) x, f)

    [<AutoOpen>]
    module Sequence =
        type Sequence = class end

        module Default2 = type Sequence with static member inline Sequence (t: '``Traversable<'Applicative<'T>>``) = printfn "Default2"; (^``Traversable<'Applicative<'T>>`` : (static member Traverse : _ * _ -> _) (t, id))

        [<AutoOpen>]
        module Invoker =
            open Default2

            type List<'t> with
                static member inline Sequence (t: list<'``Applicative<'T>``>) : '``Applicative<list<'T>>`` =
                    let cons head tail = head :: tail
                    let rec loop acc = function
                        | [] -> acc
                        | x::xs ->
                            loop (x::acc) xs
                    let cons_f x xs = Apply.Invoke (Map.Invoke cons xs) x
                    let r = (^``Applicative<'T>`` : (static member Return : _ -> _) [])
                    List.fold cons_f r (loop [] t)

            type Option<'t> with static member inline Sequence (t: option<'``Applicative<'T>``>) = match t with Some x -> Map.Invoke Some x | _ -> Return.Invoke None
            type ValueOption<'t> with static member inline Sequence (t: voption<'``Applicative<'T>``>) = match t with ValueSome x -> Map.Invoke ValueSome x | _ -> Return.Invoke ValueNone

            type Sequence with
                static member inline Invoke (x: '``Traversable<'Applicative<'T>>``) : '``Applicative<Traversable<'T>>`` =
                    ((^``Traversable<'Applicative<'T>>`` or Sequence) : (static member Sequence : _ -> _) x)

    // map squared

    let inline flip f (x: 'T) (y: 'V) : 'Result = f y x

    type List<'t>   with static member inline (|>>>) (x: list<'MonadT>, f) =  (flip (|>>) >> flip (|>>)) f x
    type Option<'t> with static member inline (|>>>) (x: option<'MonadT>, f) =  (flip (|>>) >> flip (|>>)) f x

    // map cubed

    type List<'t> with static member inline (|>>>>) (x: list<'Monad2T>, f) =  (flip (|>>) >> flip (|>>) >> flip (|>>)) f x


    // tests

    (*  fixed on b3ce0b6fc5638d11af0f2371444bca1b5d714b12
    stdin(65,16): error FS0043: A unique overload for method 'op_LessMultiplyGreater' could not be determined based on type information prior to this program point. A type annotation may be needed.

    Known return type: (int -> 'a) list

    Known type parameters: < ('b -> 'c -> 'd) list , int list >

    Candidates:
     - static member List.(<*>) : f: ('T -> 'U) list * x: 'T list -> 'U list
     - static member List.(<*>) : f: ('T -> 'U) list * x: 'T list -> 'U list
    *)
    let x01 = [(+)] <*> [1;10] <*> [2;3]

    // stdin(83,27): error FS0043: Expecting a type supporting the operator '|>>' but given a function type. You may be missing an argument to a function.
    let x02 = 5 |> (string |>> List.singleton )

    // stdin(84,29): error FS0001: Expecting a type supporting the operator '|>>>' but given a function type. You may be missing an argument to a function.
    let x03a = [string<int>] |>>> addQuotes
    let x03b = x03a |> List.map (fun f -> f 1)

    let x05 = Some 5 >>= fun x -> Some (string x)
    let x06 = [5; 6] >>= fun x -> [string x]
    let x07 = [Some 1] |>>> string
    let x08 = [[Some 1]] |>>>> string
    let x09 = [Some [1]] |>>>> string

    // stdin(76,33): error FS0043: Expecting a type supporting the operator '<*>' but given a tuple type
    let x10 = Tuple<_, _>(1, string) <*> Tuple<_, _>(2, 3)

    // stdin(81,22): error FS0043: Expecting a type supporting the operator '<*>' but given a tuple type
    let x11 = (1, string) <*> (2, 3)
    let x12 = struct (1, string) <*> struct (2, 3)

    let x13 = Result<int,string>.Ok 1 ++ Result<int,string>.Ok 3






module SimpleFSharpPlus =

    open MiniFSharpPlus

    let inline getZero () = Zero.Invoke ()
    let inline zero< .. > = Zero.Invoke () // otherwise it doesn't capture the defaults
    let inline result x = Return.Invoke x

    let inline (++) (x: 'T) (y: 'T) : 'T = Plus.Invoke x y //(^T : (static member (++) : ^T * ^T -> _) (x, y))

    let inline (|>>) x f : ^``Functor<'U>`` = Map.Invoke f x //  ((^``Functor<'T>``) : (static member (|>>) : ^``Functor<'T>`` * ('T -> 'U) -> ^``Functor<'U>``) (x, f))

    let inline (<*>) f x : ^``Applicative<'U>`` = Apply.Invoke f x // ((^``Applicative<'T -> 'U>``) : (static member (<*>) : ^``Applicative<'T -> 'U>`` * ^``Applicative<'T>`` -> ^``Applicative<'U>``) (f, x))
    let inline (>>=) x f : ^``Monad<'U>`` = Bind.Invoke x f // ((^``Monad<'T>``) : (static member (>>=) : ^``Monad<'T>`` * ('T -> ^``Monad<'U>``) -> ^``Monad<'U>``) (x, f))

    let inline traverse f x = Traverse.Invoke x f
    let inline sequence x = Sequence.Invoke x

    let inline (|>>>) x f : ^``Functor<Functor<'U>>`` = ((^``Functor<Functor<'T>>``) : (static member (|>>>) : ^``Functor<Functor<'T>>`` * ('T -> 'U) -> ^``Functor<Functor<'U>>``) (x, f))
    let inline (|>>>>) x f : ^``Functor<Functor<Functor<'U>>>`` = ((^``Functor<Functor<Functor<'T>>>``) : (static member (|>>>>) : ^``Functor<Functor<Functor<'T>>>`` * ('T -> 'U) -> ^``Functor<Functor<Functor<'U>>>``) (x, f))


open SimpleFSharpPlus

let x14 = Ok 1 ++ Ok 3 ++ Error ""

let y01 = [(+)] <*> [1;10] <*> [2;3]
let y02 = 5 |> (string |>> List.singleton )
let y03a = [string<int>] |>>> addQuotes
let y03b = y03a |> List.map (fun f -> f 1)
let y05 = Some 5 >>= fun x -> Some (string x)
let y06 = [5; 6] >>= fun x -> [string x]
let y07 = [Some 1] |>>> string
let y08 = [[Some 1]] |>>>> string
let y09 = [Some [1]] |>>>> string


// test default mechanism

type AA = AA of int with
    static member (<|>) (AA _, AA _) = AA 3
    static member (+) (AA _, AA _) = printfn "+"; AA 2
    static member (++) (AA _, AA _) = printfn "++"; AA 1

let y10 = AA 0 ++ AA 0

type BB = BB of int with
    static member (<|>) (BB _, BB _) = BB 3
    static member (+) (BB _, BB _) = printfn "+"; BB 2

let y11 = BB 0 ++ BB 0

type CC = CC of int with
    static member (<|>) (CC _, CC _) = printfn "<|>"; CC 3

let y12 = CC 0 ++ CC 0

type DD<'t> = DD of 't with
    static member Return x = DD x
    static member (>>=) (DD x, f) : DD<_> = f x

let y13 = DD 5 |>> string

type EE<'t> = EE of 't with
    static member Return x = EE x
    static member (<*>) (EE f, EE x) : EE<_> = EE (f x)

let y14 = EE 5 |>> string

// ================= assertions: witnessed operator/member tier (runtime values) =================
// x14/y01..y09: extension operators (++ >>= <*> |>>) + nested-functor operators resolve via SRTP.
if x14 <> Ok 4              then failwithf "x14 expected Ok 4, got %A" x14
if y01 <> [3; 4; 12; 13]   then failwithf "y01 expected [3;4;12;13], got %A" y01
if y02 <> ["5"]            then failwithf "y02 expected [\"5\"], got %A" y02
if y03b <> ["'1'"]         then failwithf "y03 expected [\"'1'\"], got %A" y03b
if y05 <> Some "5"         then failwithf "y05 expected Some \"5\", got %A" y05
if y06 <> ["5"; "6"]       then failwithf "y06 expected [\"5\";\"6\"], got %A" y06
if y07 <> [Some "1"]       then failwithf "y07 expected [Some \"1\"], got %A" y07
if y08 <> [[Some "1"]]     then failwithf "y08 expected [[Some \"1\"]], got %A" y08
if y09 <> [Some ["1"]]     then failwithf "y09 expected [Some [\"1\"]], got %A" y09
// y10..y14: the Default1/Default2/Default3 return-type-directed mechanism selects the witness.
if y10 <> AA 1  then failwithf "y10 expected AA 1, got %A" y10
if y11 <> BB 2  then failwithf "y11 expected BB 2, got %A" y11
if y12 <> CC 3  then failwithf "y12 expected CC 3, got %A" y12
if y13 <> DD "5" then failwithf "y13 expected DD \"5\", got %A" y13
if y14 <> EE "5" then failwithf "y14 expected EE \"5\", got %A" y14

// ============ user-written GENERIC SRTP code consuming the extension operators ============
// Each function is `inline` with only a structural constraint; the extension members declared
// in the library above supply the witnesses. Invoking at specific types shows the feature end
// to end: a caller's own generic code becomes usable purely because extensions are in scope.
let inline concatAll (xs: 'T list) : 'T = List.reduce (++) xs
let inline mapF (f: 'a -> 'b) (x: '``Functor<'a>``) : '``Functor<'b>`` = x |>> f

if concatAll [AA 0; AA 0; AA 0] <> AA 1        then failwith "concatAll AA"        // AA's (++)
if concatAll [ [1]; [2;3]; [4] ] <> [1;2;3;4]  then failwith "concatAll list"      // List (++)
if concatAll [ (1,[10]); (2,[20]) ] <> (3,[10;20]) then failwith "concatAll tuple" // tuple (++) elementwise
if mapF string [1;2;3] <> ["1";"2";"3"]        then failwith "mapF list"           // List (|>>)
if mapF ((+) 1) (Some 4) <> Some 5             then failwith "mapF option"         // Option (|>>)

printfn "MiniFSharpPlusExtensionSRTP: all assertions passed"
