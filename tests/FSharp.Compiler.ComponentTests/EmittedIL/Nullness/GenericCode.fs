module MyLibrary
let strictlyNotNull (x:obj) = ()

let myGenericFunction1 (p:_|null) = 
    match p with
    | null -> ()
    | p -> strictlyNotNull p

let myGenericFunction2 p = 
    match p with
    | Null -> ()
    | NonNull p -> strictlyNotNull p

let myGenericFunction3 p =
    match p with
    | null -> ()
    // By the time we typecheck `| null`, we assign T to be a nullable type. Imagine there could be plenty of code before this pattern match got to be typechecked.
    // As of now, the inference decision in the middle of a function cannot suddenly switch from (T which supports null) (T | null, where T is not nullable)
    | pnn -> strictlyNotNull (pnn |> Unchecked.nonNull)