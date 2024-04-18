
module M

let isNotNull (value: 'T) = 
    match value with 
    | null -> false
    | _ -> true

let s: System.String = null 