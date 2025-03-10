module M

type ToSeq =
    static member inline Invoke (source: 'FldT) : seq<'T>  =
        let inline call (mthd: ^M, input1: ^I) = ((^M or ^I) : (static member ToSeq : _*_ -> _) input1, mthd)
        call (Unchecked.defaultof<ToSeq>, source)

    static member inline ToSeq (x: 'Foldable                      , _: ToSeq) = (^Foldable: (static member ToSeq : _ -> _) x)
    static member inline ToSeq (_: 'T when 'T: null and 'T: struct, _: ToSeq) = ()

type Append =    
    static member inline Append (x: 'AltT        , y: 'AltT           , _: obj   ) = (^AltT : (static member Append : _*_ -> _) x, y) : 'AltT
    static member inline Append (_: ^t when ^t: null and ^t: struct, _, _: obj   ) = ()
    static member inline Append (x: Result<_,_>  , y                  , _: Append) = match x, y with Ok _, _ -> x | Error x, Error y -> Error (x + y) | _, _ -> y

    static member inline Invoke (x: 'AltT) (y: 'AltT) : 'AltT =
        let inline call (mthd: ^M, input1: ^I, input2: ^I) = ((^M or ^I) : (static member Append : _*_*_ -> _) input1, input2, mthd)
        call (Unchecked.defaultof<Append>, x, y)

    static member inline Append (x: 'R -> 'AltT  , y                  , _: Append) = fun r -> Append.Invoke (x r) (y r)

type Choice =
    static member inline Choice (x: ref<'RAltT>, _: obj) =
        let t = ToSeq.Invoke x.Value
        use e = t.GetEnumerator ()
        e.MoveNext() |> ignore
        let mutable res = e.Current
        while e.MoveNext() do res <- Append.Invoke res e.Current
        res

    static member inline Choice (x: ref<'FAltT> , _: Choice) = (^FAltT : (static member Choice : _ -> _) x.Value) : 'AltT
    static member inline Choice (_: ref< ^t> when ^t: null and ^t: struct, _: Choice) = ()

    static member inline Invoke (x: 'FAltT) : 'AltT =
        let inline call (mthd: ^M, input1: ^I) = ((^M or ^I) : (static member Choice : _*_ -> _) (ref input1, mthd))
        call (Unchecked.defaultof<Choice>, x)

[<NoComparison>]
type WrappedSeqE<'s> = WrappedSeqE of 's seq with static member ToSeq (WrappedSeqE x) = x

let v1 = [Ok 1; Error "a" ]
let v2 = Choice.Invoke (WrappedSeqE v1)