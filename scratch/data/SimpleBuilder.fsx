// TODO Work towards defining an option applicative builder here

[<Struct>]
type OptionalBuilder =

    member __.Bind(opt, f) =
        match opt with
        | Some x -> f x
        | None -> None
    
    // TODO Make this actually get called when `let! ... and! ...` syntax is used (and automagically if RHSs allow it?)
    member __.Apply(x : 'a option, f : ('a -> 'b) option) : 'b option =
        match f, x with
        | Some f, Some x -> f x
        | _ -> None

    // TODO Not needed, but for maximum efficiency, we want to use this if it is defined
    member __.Map(x : 'a option, f : 'a -> 'b) : 'b option =
        match x with
        | Some x -> f x
        | None -> None

    member __.Return(x) =
        Some x
    
let opt = OptionalBuilder()

let x = Some 1
let y = Some "A"
let z : float option = None

let foo =
    opt {
        let! x' = x
        let! y' = y
        let! z' = z
        return sprintf "x = %d, y = %s, z = %f" x y z
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