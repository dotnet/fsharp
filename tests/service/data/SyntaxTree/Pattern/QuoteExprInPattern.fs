module QuoteExprInPattern

let f x =
    match x with
    | <@ 1 + 2 @> -> ()
