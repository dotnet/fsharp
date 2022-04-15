// #Conformance #TypesAndModules #Exceptions 
#light

// Verify you can catch an F# exceptions can be caught via pattern matching (and not a dynamic type check).

exception NumIsOddException of int * string

let half x = 
    if x % 2 = 0 then
        x / 2
    else
        raise (NumIsOddException(x, "X is odd!"))

// Returns 1 if everything worked as expected.
let test() =
    try
        half 3 |> ignore
        -1
    with
    | NumIsOddException(4, _)               -> -1
    | NumIsOddException(_, "X is not odd.") -> -1
    | NumIsOddException(3, "X is odd!")     ->  1
    | _                                     -> -1

if test() <> 1 then failwith "Failed: 1"
