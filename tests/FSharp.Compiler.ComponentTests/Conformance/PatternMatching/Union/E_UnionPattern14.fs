module DUs =
    type Countries = 
    | CA
    | US
    | GB
let x = DUs.CA

let output = 
    match x with 
    | uSA -> "US"
    | AAA -> "CA"
    | uAA -> "U"