module Prelude

#nowarn "3370"

let test s b = if b then () else failwith s 

let out r (s:string) = r := !r @ [s]

let check s actual expected = 
    if actual = expected then printfn "%s: OK" s
    else failwithf "%s: FAILED, expected %A, got %A" s expected actual

let check2 s expected actual = check s actual expected