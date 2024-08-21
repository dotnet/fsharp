// #Conformance #DeclarationElements #LetBindings #ActivePatterns 
#light

let (|A|B|) x = if x < 0 then A else B

let _ =
    match -1 with
    | A -> printfn "So far so good..."
    | B -> failwith "Failed: 1"

let _ =
    match 10 with
    | A -> failwith "Failed: 2"
    | B -> printfn "Great!"
    


let (|DT|) (x:System.DateTime) = (x.Date, x.TimeOfDay)

let maxit a b =
    match a, b with
    | DT (ar, ai), DT (br, bi) -> max ar br
    
let result = maxit System.DateTime.Now System.DateTime.Now
if result.Date <> System.DateTime.Now.Date then failwith "Failed: 3"
