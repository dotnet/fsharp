// #Conformance #DeclarationElements #LetBindings #ActivePatterns #RequiresPowerPack 
#light

let (|A|B|) x = if x < 0 then A else B

let _ =
    match -1 with
    | A -> printfn "So far so good..."
    | B -> exit 1

let _ =
    match 10 with
    | A -> exit 1
    | B -> printfn "Great!"
    
// Example from Expert F#
open Microsoft.FSharp.Math
let (|Rect|) (x:complex) = (x.RealPart, x.ImaginaryPart)

let addViaRect a b =
    match a, b with
    | Rect (ar, ai), Rect (br, bi) -> Complex.mkRect(ar+br, ai+bi)
    
let result = addViaRect Complex.One Complex.OneI
if result.r.CompareTo(1.0) <> result.i.CompareTo(1.0) then exit 1

// Looks like active patterns are working OK
exit 0







