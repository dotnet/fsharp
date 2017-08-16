
module Test

type Evaluator<'t,'ret> = abstract member Eval : ('t -> 'u) -> 'u list -> 'ret
and 't Crate = abstract member Eval : Evaluator<'t,'ret> -> 'ret

module Bar =
    let foo (x : 't Crate) =
        x.Eval { new Evaluator<_,_> with
            member __.Eval (x, y) = failwith ""
        }