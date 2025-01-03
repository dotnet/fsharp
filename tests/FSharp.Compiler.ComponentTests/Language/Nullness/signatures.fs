module M

let test1 (x: string) = ()
let test2 (x: string | null) = ()

let iRejectNulls (x:_|null) = 
    match x with
    | null -> "null"
    | s -> String.length s |> string

type GenericContainer<'T when 'T:not null and 'T:not struct>(x:'T) =
    let innerVal = x

    member _.GetNull() : ('T) = x
    member _.GetNotNull() : ('T|null) = null

let private GetString(key:string) : string = key + ""
let GetTextOpt(key:string) = GetString(key) |> Option.ofObj