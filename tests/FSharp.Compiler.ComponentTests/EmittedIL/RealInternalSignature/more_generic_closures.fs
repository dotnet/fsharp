namespace more_generic_closures 

open System.Collections
open System.Collections.Generic

type ConcatEnumerator<'T,'U when 'U :> seq<'T>>(sources : seq<'U>) as x =

    [<DefaultValue>]
    val mutable internal outerEnum : IEnumerator<'U>

    [<DefaultValue>]
    val mutable internal innerEnum : IEnumerator<'T>

    [<DefaultValue>]
    val mutable public CurrentElement : 'T

    do
        x.outerEnum <- sources.GetEnumerator()
        x.innerEnum <- Seq.empty<'T>.GetEnumerator()
        x.CurrentElement <- Unchecked.defaultof<'T>

    member x.MoveNext() : bool =
        let rec takeInner () : bool =
            if x.innerEnum.MoveNext() then
                x.CurrentElement <- x.innerEnum.Current
                true
            else
                takeOuter ()

        and takeOuter () : bool =
            if x.outerEnum.MoveNext() then
                let ie : 'U = x.outerEnum.Current
                x.innerEnum <- ie.GetEnumerator()
                takeInner ()
            else
                false

        takeInner ()

module Run =
    let test() =
        let data : seq<seq<int>> =
            seq {
                yield seq { 1; 2 }
                yield seq { }
                yield seq { 3 }
            }

        let c = ConcatEnumerator<int, seq<int>>(data)
        let r1 = c.MoveNext()
        let r2 = c.MoveNext()
        let r3 = c.MoveNext()

        printfn "MoveNext1 = %A, CurrentElement = %A" r1 c.CurrentElement
        printfn "MoveNext2 = %A, CurrentElement = %A" r2 c.CurrentElement
        printfn "MoveNext3 = %A, CurrentElement = %A" r3 c.CurrentElement

    test()
