module DUs =
    type Countries = 
    | CA
    | US
    | GB
let x = DUs.CA

open DUs

let output = 
    match x with 
    | US -> "US"
    | CA -> "CA"
    | UUU -> "U"