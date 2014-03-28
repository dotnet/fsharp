// #NoMono #NoMT #CodeGen #EmittedIL 
#light

let TestFunction9b(x) =
    match x with 
    | [1;2] -> "three"
    | [3;4] -> "seven"
    | [a;b] when a+b = 4 -> "four"
    | _ -> "big"
