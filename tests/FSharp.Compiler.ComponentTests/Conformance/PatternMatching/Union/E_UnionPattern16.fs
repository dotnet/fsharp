module DUs =
    type Countries = 
    | CA
    | US
    | GB
let x = DUs.CA

let output = 
    match x with 
    | US -> "US"
    | CA -> "CA"
    | uuu -> "U"