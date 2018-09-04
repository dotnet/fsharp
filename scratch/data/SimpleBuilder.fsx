// TODO Work towards defining an option applicative builder here

[<Struct>]
type OptionalBuilder =
  member __.Bind(opt, binder) =
    match opt with
    | Some value -> binder value
    | None -> None
  member __.Return(value) =
    Some value
    
let optional = OptionalBuilder()

let x = Some 1
let y = Some "A"
let z : float option = None

let foo =
    optional
        {
            let! x' = x
            let! y' = y
            let! z' = z
            return sprintf "x = %d, y = %s, z = %f" x y z
        }

(*
let foo' =
    optional
        {
            let! x' = x
            and! y' = y
            and! z' = z
            return sprintf "x = %d, y = %s, z = %f" x y z
        }
*)