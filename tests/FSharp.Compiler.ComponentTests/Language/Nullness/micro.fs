
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
