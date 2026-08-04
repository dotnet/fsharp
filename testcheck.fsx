type List<'t> with
    static member (|>>)(x: list<'t>, f: 't -> 'u) : list<'u> = List.map f x

type Option<'t> with
    static member (|>>)(x: option<'t>, f: 't -> 'u) : option<'u> = Option.map f x

type Microsoft.FSharp.Core.FSharpFunc<'t, 'u> with
    static member (|>>)(f: 't -> 'u, g: 'u -> 'v) : 't -> 'v = f >> g

let inline flip f x y = f y x

type List<'t> with
    static member inline (|>>>)(x: list<'MonadT>, f) = (flip (|>>) >> flip (|>>)) f x

type List<'t> with
    static member inline (|>>>>)(x: list<'Monad2T>, f) =
        (flip (|>>) >> flip (|>>) >> flip (|>>)) f x

let x07 = [ Some 1 ] |>>> string
let x08 = [ [ Some 1 ] ] |>>>> string
let x09 = [ Some [ 1 ] ] |>>>> string
printfn "x07=%A x08=%A x09=%A" x07 x08 x09
