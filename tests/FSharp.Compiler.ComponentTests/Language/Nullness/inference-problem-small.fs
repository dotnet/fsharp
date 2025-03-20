module M


type Choice =
    static member inline Choice (x: ref<'RAltT>, _: obj) =
        42

    static member inline Choice (x: ref<'FAltT> , _: Choice) = (^FAltT : (static member Choice : _ -> _) x.Value) : 'AltT
    //static member inline Choice (_: ref< ^t> when ^t: null and ^t: struct, _: Choice) = ()

    static member inline Invoke (x: 'FAltT) : 'AltT =
        let inline call (mthd: ^M, input1: ^I) = ((^M or ^I) : (static member Choice : _*_ -> _) (ref input1, mthd))
        call (Unchecked.defaultof<Choice>, x)

[<NoComparison>]
type WrappedSeqE<'s> = WrappedSeqE of 's seq with static member ToSeq (WrappedSeqE x) = x

let v1 = [Ok 1; Error "a" ]
let v2 = Choice.Invoke (WrappedSeqE v1)