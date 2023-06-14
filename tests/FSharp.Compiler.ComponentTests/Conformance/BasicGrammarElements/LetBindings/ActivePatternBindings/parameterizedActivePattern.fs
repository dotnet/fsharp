// #Conformance #DeclarationElements #LetBindings #ActivePatterns 
#light

let (|MulN|_|) (n:int) (inp:int) = if inp % 3 = 0 then Some(inp/n) else None

// Verify it works as exected.
let posCase =
    match 30 with
    | MulN 10 result -> if result <> 3 then failwith "Failed: 1"
    | _ -> failwith "Failed: 2"

let negCase =
    match 30 with
    | MulN 5 result -> if result <> 6 then failwith "Failed: 1"
    | _ -> failwith "Failed: 2"

printfn "Success!"
