
module M

let isNotNull (value: 'T) = 
    match value with 
    | null -> false
    | _ -> true

#if CHECKNULLS
let s: System.String | null = null
#else
let s: System.String = null 
#endif

let test1 (x: string) = ()
let test2 (x: string | null) = ()

let iRejectNulls (x:_|null) = 
    if (String.length x) > 5 then "big" else "small"

type GenericContainer<'T>(x:'T) =
    let innerVal = x

    member _.GetNull() : ('T|null) = null