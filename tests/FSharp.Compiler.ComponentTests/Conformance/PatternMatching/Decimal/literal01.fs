// #Conformance #PatternMatching 
#light

// Pattern match decimal literals

[<Literal>]
let Decimal1 = 5m

[<Literal>]
let Decimal2 = 42.42m

let testDecimal x =
    match x with
    | Decimal1 -> 1
    | Decimal2 -> 2
    | _ -> 0

if testDecimal 1m <> 0 then exit 1

if testDecimal Decimal1 <> 1 then exit 1
if testDecimal 5m <> 1 then exit 1

if testDecimal Decimal2 <> 2 then exit 1
if testDecimal 42.42m <> 2 then exit 1

exit 0