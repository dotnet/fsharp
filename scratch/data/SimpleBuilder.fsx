
[<Struct>]
type OptionalBuilder =

    member __.Apply(fOpt : ('a -> 'b) option, xOpt : 'a option) : 'b option =
        match fOpt, xOpt with
        | Some f, Some x -> Some (f x)
        | _ -> None

    member __.Return(x) =
        Some x

let opt = OptionalBuilder()

let fOpt = Some (sprintf "f (x = %d) (y = %s) (z = %f)")
let xOpt = Some 1
let yOpt = Some "A"
let zOpt = Some 3.0

let foo =
    opt {
        let! f = fOpt
        and! x = xOpt
        and! y = yOpt
        and! z = zOpt
        return f x y z
    }

printfn "foo: \"%+A\"" foo 

let bar =
    opt {
        let! x = None
        and! y = Some 1
        and! z = Some 2
        return x + y + z + 1
    }

printfn "bar: %+A" bar 

type 'a SingleCaseDu = SingleCaseDu of 'a

let baz =
    opt {
        let! x                = Some 4
        and! (SingleCaseDu y) = Some (SingleCaseDu 30)
        and! (z,_)            = Some (200, "whatever")
        return (let w = 50000 in x + y + z + 1000)
    }

printfn "baz: %+A" baz 

(*
let foo' =
    opt {
        let! x' = x
        and! y' = y
        and! z' = z
        return sprintf "x = %d, y = %s, z = %f" x y z
    }
*)
