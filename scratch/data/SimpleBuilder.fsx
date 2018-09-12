// TODO Work towards defining an option applicative builder here

[<Struct>]
type OptionalBuilder =

    member __.Bind(opt, f) =
        match opt with
        | Some x -> f x
        | None -> None
    
    // TODO Make this actually get called when `let! ... and! ...` syntax is used (and automagically if RHSs allow it?)
    member __.Apply(fOpt : ('a -> 'b) option, xOpt : 'a option) : 'b option =
        match fOpt, xOpt with
        | Some f, Some x -> Some <| f x
        | _ -> None

    // TODO Not needed, but for maximum efficiency, we want to use this if it is defined
    member __.Map(f : 'a -> 'b, xOpt : 'a option) : 'b option =
        match xOpt with
        | Some x -> Some <| f x
        | None -> None

    member __.Return(x) =
        Some x
    
let opt = OptionalBuilder()

let xOpt = Some 1
let yOpt = Some "A"
let zOpt = None

let foo =
    opt {
        let! x = xOpt
        and! y = yOpt
        return sprintf "x = %d, y = %s" x y
    }

(*
let foo' =
    opt {
        let! x' = x
        and! y' = y
        and! z' = z
        return sprintf "x = %d, y = %s, z = %f" x y z
    }
*)
