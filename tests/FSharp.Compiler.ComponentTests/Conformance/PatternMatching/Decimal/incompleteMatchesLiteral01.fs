[<Literal>]
let One = 1m
[<Literal>]
let Two = 2m

let test() = 
    match 3m with
    | 0m -> false
    | One | Two -> false

exit 0